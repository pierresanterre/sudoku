using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static sudoku.Puzzle;

namespace sudoku
{
    public partial class Puzzle
    {
        public bool SetCell(int cellOrdinal, Digit digit)
        {
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
                                }
                            }
                        }
                    }
                }
                return true;
            }
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

        private bool SolveWithExceptions(InternalInitialCellDigit[] internalInitialCellDigits)
        {
            ui.Log("Starting", ConsoleColor.Green);
            ui.FullUI(cells);
            foreach (InternalInitialCellDigit internalInitialCellDigit in internalInitialCellDigits)
            {
                ui.Log($"From initial condition, cell{cells[internalInitialCellDigit.ordinal].At()} is {internalInitialCellDigit.digit}", ConsoleColor.Green);
                SetCell(internalInitialCellDigit.ordinal, internalInitialCellDigit.digit);
            }
            ui.Log("After initial cells", ConsoleColor.Green);
            ui.FullUI(cells);
            if (!success.HasValue)
            {
                ui.Log("Not solved", ConsoleColor.Red);
                return false;
            }
            else
            {
                int ordinal = 0;
                Console.Write($"Final solution, success={success.Value}");
                for (int y = 0; y < numDigits; y++)
                {
                    Console.WriteLine("");
                    for (int x = 0; x < numDigits; x++)
                    {
                        Mask mask = cells[ordinal].mask;
                        if (mask.IsImpossible())
                        {
                            Console.Write("!");
                        }
                        else if (mask.IsFixed())
                        {
                            Digit digit = mask.FixedDigit();
                            Console.Write(digit.display);
                        }
                        else
                        {
                            Console.Write("?");
                        }
                        ordinal++;
                    }
                }
                return success.Value;
            }
        }

        public bool Solve(InitialCellDigit[] initialCellDigits)
        {
            bool result = BreakWithDebugger(() => {
                InternalInitialCellDigit[] internalInitialCellDigits = initialCellDigits.Select(x => x.ToInternalInitialCellDigit(this)).ToArray();
                bool exceptionResult = SolveWithExceptions(internalInitialCellDigits);
                return exceptionResult;
            }
            , (exception) =>
            {
                ui.Log($"Exception: {exception.Message}", ConsoleColor.DarkRed);
                return false;
            });

            return result;
        }

        public bool Solve(string[] lines)
        {
            bool result = BreakWithDebugger(() => {
                InternalInitialCellDigit[] internalInitialCellDigits = StringsToInternalInitialCellDigits(this, lines);
                bool exceptionResult = SolveWithExceptions(internalInitialCellDigits);
                return exceptionResult;
            }
            , (exception) =>
            {
                ui.Log($"Exception: {exception.Message}", ConsoleColor.DarkRed);
                return false;
            });

            return result;
        }
    }
}
