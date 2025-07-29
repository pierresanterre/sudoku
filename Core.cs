using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static sudoku.Core;

namespace sudoku
{
    public class Core
    {
        public abstract class UI
        {
            protected Core core;
            public abstract void Log(string text, ConsoleColor consoleColor);
            public abstract void FullUI(Cell[] cells);
            protected UI(Core core)
            {
                this.core = core;
            }
        }

        const int maxDigits = 62; // Assumed <= 64 for long Mask (and avoid base64 last 2 characters decision);
        const string digitsAsCharacters = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public readonly int boxWidth;
        public readonly int boxHeight;
        public readonly int numDigits;
        public readonly Digit[] digits;
        public readonly UI ui;
        public Cell[] cells;

        public static string RepeatChar(char character, int length)
        {
            string result = "".PadLeft(length, character);
            return result;
        }

        public static string ArrayToString<E>(E[] elememts) where E : class
        {
            string result = "[";
            bool first = true;
            foreach (E e in elememts)
            {
                if (!first)
                {
                    result += ",";
                }
                first = false;
                result += e.ToString();
            }
            result += "]";
            return result;
        }

        char DigitToDisplay(int ordinal)
        {
            char result = digitsAsCharacters[ordinal];
            return result;
        }

        int DisplayToOrdinal(char display)
        {
            int ordinal = digitsAsCharacters.IndexOf(display);
            if (ordinal == -1)
            {
                throw new Exception($"Character {display} is not a valid digit");
            }
            if (ordinal >= numDigits)
            {
                throw new Exception($"Character {display} is ordinal {ordinal} which is too big for sudoku size {numDigits}");
            }

            return ordinal;
        }

        /// <summary>
        /// A mask is the set of possible digits.
        /// The key usage is by the cell structure, but we abstract the operations here.
        /// </summary>
        public class Mask
        {
            private Core core;
            private long bitmap;
            private int bitsInMask;

            private void RecomputeBitsInMask()
            {
                bitsInMask = 0;
                for (int i = 0; i < core.numDigits; i++)
                {
                    if ((bitmap & (1 << i)) != 0)
                    {
                        bitsInMask += 1;
                    }
                }
            }

            public bool IsImpossible()
            {
                bool result = (bitsInMask == 0);
                return result;
            }

            public bool IsFixed()
            {
                bool result = (bitsInMask == 1);
                return result;
            }

            public Digit FixedDigit()
            {
                for (int i = 0; i < core.numDigits; i++)
                {
                    if ((bitmap & (1 << i)) != 0)
                    {
                        return core.digits[i];
                    }
                }

                throw new Exception("No fixed digit");
            }

            public bool DigitPresent(Digit digit)
            {
                bool result = ((bitmap & digit.mask.bitmap) != 0);
                return result;
            }

            public void And(Mask mask)
            {
                bitmap = bitmap & mask.bitmap;
                RecomputeBitsInMask();
            }

            public bool MaskOff(Mask mask)
            {
                long newBitmap = bitmap & ~mask.bitmap;
                if (bitmap == newBitmap)
                {
                    return false;
                }
                else
                {
                    bitmap = newBitmap;
                    RecomputeBitsInMask();
                    return true;
                }
            }

            public bool MaskOff(Digit digit)
            {
                bool result = MaskOff(digit.mask);
                return result;
            }

            public bool MaskForOnly(Digit digit)
            {
                if (bitmap == digit.mask.bitmap)
                {
                    return false;
                }
                else
                {
                    bitmap = digit.mask.bitmap;
                    RecomputeBitsInMask();
                    return true;
                }
            }

            public int NumPossibleDigits()
            {
                return bitsInMask;
            }

            public override string ToString()
            {
                string result = string.Empty;
                if (IsImpossible())
                {
                    result = RepeatChar('X', core.numDigits);
                }
                else if (IsFixed())
                {
                    Digit digit = FixedDigit();
                    result = RepeatChar(digit.display, core.digits.Length);
                }
                else
                {
                    foreach (Digit digit in core.digits)
                    {
                        if (DigitPresent(digit))
                        {
                            result += digit.display;
                        }
                        else
                        {
                            result += " ";
                        }
                    }
                }
                return result;
            }

            public Mask(Core core, int ordinal)
            {
                this.core = core;
                bitmap = 1 << ordinal;
                bitsInMask = 1;
            }

            public Mask(Core core)
            {
                this.core = core;
                if (core.numDigits > 64)
                {
                    throw new Exception("Maximum is 64 digits");
                }

                if (core.numDigits == 64)
                {
                    bitmap = Int64.MaxValue;
                }
                else
                {
                    bitmap = (1 << core.numDigits) - 1;
                }
                bitsInMask = core.numDigits;
            }
        }

        /// <summary>
        /// A digit is the abstraction for the symbol to display. It also has the mask
        /// that represent that single digit.
        /// </summary>
        public class Digit
        {
            private Core core;
            public char display;
            public Mask mask;

            public Digit(Core core, int ordinal)
            {
                this.core = core;
                display = core.DigitToDisplay(ordinal);
                mask = new Mask(core, ordinal);
            }

            public override string ToString()
            {
                string result = display.ToString();
                return result;
            }
        };

        public enum GroupType
        {
            Row,
            Column,
            Box,
        };

        /// <summary>
        /// A group is the array of cells that are joined either
        /// horizontally, vertically or in a box.
        /// </summary>
        public class Group
        {
            public GroupType groupType;
            public int ordinal;
            public Cell[] cells;

            public Group(GroupType groupType, int ordinal, Cell[] cells)
            {
                this.groupType = groupType;
                this.ordinal = ordinal;
                this.cells = cells;
            }

            public override string ToString()
            {
                string result = $"{groupType.ToString()}({ordinal})=";
                Mask[] masks = cells.Select(x => x.mask).ToArray();
                result += ArrayToString(masks);
                return result;
            }
        }

        public class InitialCellDigit
        {
            public int x;
            public int y;
            public char display;
            public InitialCellDigit(int x, int y, char display)
            {
                this.x = x;
                this.y = y;
                this.display = display;
            }

            public override string ToString()
            {
                string result = $"{x},{y} = {display}";
                return result;
            }
        }

        public class Cell
        {
            public Mask mask;
            public int ordinal;

            public void SetCell(Digit digit)
            {
                mask.MaskForOnly(digit);
            }

            public Cell(Core core, int ordinal)
            {
                mask = new Mask(core);
                this.ordinal = ordinal;
            }

            public override string ToString()
            {
                string result = $"{ordinal.ToString()}=";
                result += mask.ToString();
                return result;
            }
        }

        private void SolveWithExceptions(InitialCellDigit[] initialCellDigits)
        {
            foreach (InitialCellDigit initialCellDigit in initialCellDigits)
            {
                int x = initialCellDigit.x - 1;
                if (x < 0 || x >= numDigits)
                {
                    throw new Exception($"Invalid initial cell: {initialCellDigit}");
                }
                int y = initialCellDigit.y - 1;
                if (y < 0 || y >= numDigits)
                {
                    throw new Exception($"Invalid initial cell: {initialCellDigit}");
                }
                int cellOrdinal = x + (y * numDigits);
                int digitOrdinal = DisplayToOrdinal(initialCellDigit.display);
                Digit digit = digits[digitOrdinal];
                cells[cellOrdinal].SetCell(digit);
            }
            ui.Log("After initial cells", ConsoleColor.Green);
            ui.FullUI(cells);
        }

        public void Solve(InitialCellDigit[] initialCellDigits)
        {
            ui.Log("Starting", ConsoleColor.Green);
            ui.FullUI(cells);
            if (Debugger.IsAttached)
            {
                SolveWithExceptions(initialCellDigits);
            }
            else
            {
                try
                {
                    SolveWithExceptions(initialCellDigits);
                }
                catch (Exception exception)
                {
                    ui.Log($"Exception: {exception.Message}", ConsoleColor.DarkRed);
                }
            }
        }

        public Core(int boxWidthParam, int boxHeightParam)
        {
            Core core = this;
            boxWidth = boxWidthParam;
            boxHeight = boxHeightParam;
            numDigits = boxHeight * boxWidth;

            digits = new Digit[numDigits];
            for (int i = 0; i < numDigits; i++)
            {
                digits[i] = new Digit(core, i);
            }

            cells = new Cell[numDigits * numDigits];
            for (int i = 0; i < numDigits * numDigits; i++)
            {
                cells[i] = new Cell(core, i);
            }

            ui = new UIConsole(core);
        }
    }
}
