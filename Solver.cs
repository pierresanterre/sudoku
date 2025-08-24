using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace sudoku
{
    public partial class Puzzle
    {
        public class Solution : IEquatable<Solution>
        {
            private bool? success;
            public readonly string exception;
            public readonly int numDigits;
            private string[] solutionLines;

            bool Equals(Solution solution)
            {
                if (!string.IsNullOrEmpty(exception))
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(solution.exception))
                {
                    return false;
                }

                if (success != solution.success)
                {
                    return false;
                }

                if (numDigits != solution.numDigits)
                {
                    return false;
                }

                if (solution.solutionLines.Length != solutionLines.Length)
                {
                    return false;
                }

                if ((solution.solutionLines.Length != numDigits) || (solutionLines.Length != solution.numDigits))
                {
                    throw new Exception("Inconsistent solution size");
                }

                bool result = true;
                for (int i = 0; i < numDigits; i++)
                {
                    if (string.Compare(solutionLines[i], solution.solutionLines[i], false) != 0)
                    {
                        result = false;
                        break;
                    }
                }

                return result;
            }

            bool IEquatable<Solution>.Equals(Solution? solution)
            {
                if (solution == null)
                {
                    return false;
                }

                bool result = Equals(solution);
                return result;
            }

            public void Verify(string title, Solution expectedSolution)
            {
                bool compared = Equals(expectedSolution);
                if (compared)
                {
                    Console.WriteLine($"{title} has expected solution");
                }
                else
                {
                    Console.WriteLine($"{title} does not have expected solution. Computed:");
                    Display();
                    Console.WriteLine($"Expected:");
                    expectedSolution.Display();
                }
            }

            public void SetSolution(bool? success, string[] solutionLines)
            {
                this.success = success;
                this.solutionLines = solutionLines;
            }

            public Solution(string exception)
            {
                success = null;
                this.exception = exception;
                numDigits = 0;
                solutionLines = Array.Empty<string>();
            }

            public Solution(bool? success, string[] solutionLines)
            {
                this.success = success;
                exception = string.Empty;
                numDigits = solutionLines.Length;
                this.solutionLines = solutionLines;
            }

            private string[] ToStringLines()
            {
                string[] result = new string[numDigits + 1];
                if (success.HasValue)
                {
                    if (success.Value)
                    {
                        result[0] = "Success";
                    }
                    else
                    {
                        result[0] = "Failure";
                    }
                }
                else
                {
                    result[0] = "Incomplete";
                }

                if (!string.IsNullOrEmpty(exception))
                {
                    result[0] += $": Exception {exception}";
                    result = result[0..];
                }
                else
                {
                    for (int i = 0; i < numDigits; i++)
                    {
                        result[i + 1] += solutionLines[i];
                    }
                }

                return result;
            }

            public override string ToString()
            {
                string[] lines = ToStringLines();
                string result = lines.Aggregate((accumulator, item) => accumulator + "|" + item);
                return result;
            }

            public void Display()
            {
                string[] lines = ToStringLines();
                foreach (var line in lines)
                {
                    Console.WriteLine(line);
                }
                // string result = lines.Aggregate((accumulator, item) => accumulator + item);
            }
        }

        /// <summary>
        /// We are give a cellOrdinal with its now know digit.
        /// 1) Set the digit in the puzzle and display the new puzzle
        /// 2) Process all groups this cell is in with masking off the digit. Recurse if that makes cells with unique mask.
        /// </summary>
        /// <param name="cellOrdinal"></param>
        /// <param name="digit"></param>
        /// <returns>true if and only if we set a cell. False if no change or the puzzle is now complete</returns>
        private bool SetCell(int cellOrdinal, Digit digit)
        {
            bool result = false;
            Cell cell = cells[cellOrdinal];
            if (cell.mask == digit.mask)
            {
                return false;
            }
            else
            {
                cell.SetCell(digit);
                digitsUnknown--;
                if (digitsUnknown == 0)
                {
                    success = true;
                    return false;
                }
                ui.FullUI(cells);
                for (int groupTypeIndex = 0; groupTypeIndex  < countGroupTypes; groupTypeIndex++)
                {
                    GroupElement groupElement = cell.groupOrdinals[groupTypeIndex];
                    Group group = groups[groupTypeIndex, groupElement.groupOrdinal];
                    for (int i = 0; i < numDigits; i++)
                    {
                        if (i != groupElement.inGroupOrdinal)
                        {
                            if (success != null)
                            {
                                return false;
                            }

                            Cell newCell = group.cells[i];
                            Mask newMask = newCell.mask;
                            if (newMask.MaskOff(digit))
                            {
                                group.maskLeft.MaskOff(digit);
                                if (newMask.IsImpossible())
                                {
                                    success = false;
                                    return false;
                                }
                                else if (newMask.IsFixed())
                                {
                                    Digit newFixedDigit = newMask.FixedDigit();
                                    ui.Log($"From cell{cell.At()} being {digit}, we can now deduce cell{newCell.At()} is {newFixedDigit}", ConsoleColor.Green);
                                    SetCell(newCell.ordinal, newFixedDigit);
                                    result = true;
                                }
                            }
                        }
                    }
                }
                return result;
            }
        }

        private bool SimplifyGroup(Group group)
        {
            return false;
        }

        private bool Simplify()
        {
            bool result = false;
            for (int groupTypeIndex = 0; groupTypeIndex < countGroupTypes; groupTypeIndex++)
            {
                for (int i = 0; i < numDigits; i++)
                {
                    result |= SimplifyGroup(groups[groupTypeIndex, i]);
                }
            }

            return result;
        }

        public class InternalInitialCellDigit
        {
            public int ordinal;
            public Digit digit;

            public InternalInitialCellDigit(Puzzle puzzle, int cellOrdinal, char display)
            {
                ordinal = cellOrdinal;
                int digitOrdinal = puzzle.DisplayToOrdinal(display);
                Digit digit = puzzle.digits[digitOrdinal];
                this.digit = digit;
            }

            public override string ToString()
            {
                string result = $"{ordinal}={digit}";
                return result;
            }
        }

        private Solution SolveWithExceptions(InternalInitialCellDigit[] internalInitialCellDigits)
        {
            ui.Log("Starting", ConsoleColor.Green);
            ui.FullUI(cells);
            foreach (InternalInitialCellDigit internalInitialCellDigit in internalInitialCellDigits)
            {
                ui.Log($"From initial condition, cell{cells[internalInitialCellDigit.ordinal].At()} is {internalInitialCellDigit.digit}", ConsoleColor.Green);
                SetCell(internalInitialCellDigit.ordinal, internalInitialCellDigit.digit);
            }
            ui.Log("", ConsoleColor.Green);
            ui.Log("", ConsoleColor.Green);
            ui.Log("", ConsoleColor.Green);
            ui.Log("After initial cells", ConsoleColor.Green);
            ui.FullUI(cells);
            if (!success.HasValue)
            {
                Simplify();
            }

            string[] solutionLines = new string[numDigits];
            int ordinal = 0;
            for (int y = 0; y < numDigits; y++)
            {
                solutionLines[y] = string.Empty;
                for (int x = 0; x < numDigits; x++)
                {
                    Mask mask = cells[ordinal].mask;
                    if (mask.IsImpossible())
                    {
                        solutionLines[y] += "!";
                    }
                    else if (mask.IsFixed())
                    {
                        Digit digit = mask.FixedDigit();
                        solutionLines[y] += digit.display;
                    }
                    else
                    {
                        solutionLines[y] += "?";
                    }
                    ordinal++;
                }
            }

            Solution solution = new Solution(success, solutionLines);
            if (!success.HasValue)
            {
                ui.Log("Not solved", ConsoleColor.Red);
            }
            else
            {
                Console.Write($"Final solution, success={success.Value}");
                solution.Display();
            }

            return solution;
        }

        public Solution Solve(InitialCellDigit[] initialCellDigits)
        {
            Solution solution = BreakWithDebugger<Solution>(() => {
                InternalInitialCellDigit[] internalInitialCellDigits = initialCellDigits.Select(x => x.ToInternalInitialCellDigit(this)).ToArray();
                Solution exceptionResult = SolveWithExceptions(internalInitialCellDigits);
                return exceptionResult;
            }
            , (exception) =>
            {
                ui.Log($"Exception: {exception.Message}", ConsoleColor.DarkRed);
                return new Solution(exception.Message);
            });

            return solution;
        }

        public Solution Solve(string[] lines)
        {
            Solution result = BreakWithDebugger<Solution>(() => {
                InternalInitialCellDigit[] internalInitialCellDigits = StringsToInternalInitialCellDigits(this, lines);
                Solution exceptionResult = SolveWithExceptions(internalInitialCellDigits);
                return exceptionResult;
            }
            , (exception) =>
            {
                ui.Log($"Exception: {exception.Message}", ConsoleColor.DarkRed);
                return new Solution(exception.Message);
            });

            return result;
        }
    }
}
