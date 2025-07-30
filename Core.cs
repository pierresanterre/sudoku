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

        /// <summary>
        /// Class used to be able to pass a void method as a Func
        /// </summary>
        public class VoidClass
        {
            /// <summary>
            /// For showing in the debugger
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return "VoidClass";
            }
        }

        /// <summary>
        /// Return a string of length * the same character
        /// </summary>
        /// <param name="character"></param>
        /// <param name="length"></param>
        /// <returns>The string of length of the same character</returns>
        public static string RepeatChar(char character, int length)
        {
            string result = "".PadLeft(length, character);
            return result;
        }

        public delegate E ExceptionCallback<E>(Exception exception);

        public static E? BreakWithDebugger<E>(Func<E?> func, ExceptionCallback<E?> exceptionCallback)
        {
            E? result = default(E);
            if (Debugger.IsAttached)
            {
                result = func();
            }
            else
            {
                try
                {
                    result = func();
                }
                catch (Exception exception)
                {
                    result = exceptionCallback(exception);
                }
            }

            return result;
        }

        public static string IEnumerableToString<E>(IEnumerable<E> elements) where E : class
        {
            string result = "[";
            bool first = true;
            foreach (E e in elements)
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
                IEnumerable<Mask> masks = cells.Select(x => x.mask);
                result += IEnumerableToString(masks);
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

        public class InternalInitialCellDigit
        {
            public int ordinal;
            public Digit digit;

            public InternalInitialCellDigit(Core core, int cellOrdinal, char display)
            {
                ordinal = cellOrdinal;
                int digitOrdinal = core.DisplayToOrdinal(display);
                Digit digit = core.digits[digitOrdinal];
                this.digit = digit;
            }

            public override string ToString()
            {
                string result = $"{ordinal}={digit}";
                return result;
            }
        }

        public class InitialCellDigit
        {
            public int x;
            public int y;
            public char display;

            public InternalInitialCellDigit ToInternalInitialCellDigit(Core core)
            {
                x = x - 1;
                if (x < 0 || x >= core.numDigits)
                {
                    throw new Exception($"Invalid initial cell: {this}");
                }
                y = y - 1;
                if (y < 0 || y >= core.numDigits)
                {
                    throw new Exception($"Invalid initial cell: {this}");
                }
                int cellOrdinal = x + (y * core.numDigits);
                InternalInitialCellDigit result = new InternalInitialCellDigit(core, cellOrdinal, display);
                return result;
            }

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

        internal InternalInitialCellDigit[] StringsToInternalInitialCellDigits(Core core, string[] lines)
        {
            List<InternalInitialCellDigit> internalInitialCellDigits = new List<InternalInitialCellDigit>();
            if (lines.Length != numDigits)
            {
                throw new Exception($"We must have {numDigits} initialization strings, not {lines.Length}");
            }

            for (int i = 0; i < numDigits; i++)
            {
                string line = lines[i];
                if (line.Length != numDigits)
                {
                    throw new Exception($"Initialization strings must have length {numDigits}, not {line.Length} on line {i}: {line}");
                }
                for (int j = 0; j < numDigits; j++)
                {
                    if (line[j] != ' ')
                    {
                        internalInitialCellDigits.Add(new InternalInitialCellDigit(core, (i * numDigits) + j, line[j]));
                    }
                }
            }

            InternalInitialCellDigit[] result = internalInitialCellDigits.ToArray();
            return result;
        }

        private bool SolveWithExceptions(InternalInitialCellDigit[] internalInitialCellDigits)
        {
            ui.Log("Starting", ConsoleColor.Green);
            ui.FullUI(cells);
            foreach (InternalInitialCellDigit internalInitialCellDigit in internalInitialCellDigits)
            { 
                cells[internalInitialCellDigit.ordinal].SetCell(internalInitialCellDigit.digit);
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
