using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                ui.FullUI(cells);
                for (int groupTypeIndex = 0; groupTypeIndex  < countGroupTypes; groupTypeIndex++)
                {
                    GroupElement groupElement = cell.groupOrdinals[groupTypeIndex];
                    Group group = groups[groupTypeIndex, groupElement.groupOrdinal];
                    for (int i = 0; i < numDigits; i++)
                    {
                        if (i != groupElement.inGroupOrdinal)
                        {
                            group.cells[i].mask.MaskOff(digit);
                        }
                    }
                }
                ui.FullUI(cells);
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
                SetCell(internalInitialCellDigit.ordinal, internalInitialCellDigit.digit);
            }
            ui.Log("After initial cells", ConsoleColor.Green);
            ui.FullUI(cells);
            return true;
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
