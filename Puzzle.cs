using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace sudoku
{
    public partial class Puzzle
    {
        public abstract class UI
        {
            protected Puzzle puzzle;
            public abstract void Log(string text, ConsoleColor consoleColor);
            public abstract void FullUI(Cell[] cells);
            protected UI(Puzzle puzzle)
            {
                this.puzzle = puzzle;
            }
        }

        const int maxDigits = 62; // Assumed <= 64 for long Mask (and avoid base64 last 2 characters decision);
        const string digitsAsCharacters = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        public readonly int boxWidth;
        public readonly int boxHeight;
        public readonly int numDigits;
        public readonly Digit[] digits;
        public readonly UI ui;
        public readonly int countGroupTypes;
        public Cell[] cells;
        public Group[,] groups;
        public int digitsUnknown;
        public bool? success = null;

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
        /// Given an enum type E, return its number of elements
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <returns></returns>
        public static int CountOfEnum<E>()
        {
            int result = Enum.GetValues(typeof(E)).Length;
            return result;
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
            private Puzzle puzzle;
            private long bitmap;
            private int bitsInMask;

            private void RecomputeBitsInMask()
            {
                bitsInMask = 0;
                for (int i = 0; i < puzzle.numDigits; i++)
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
                for (int i = 0; i < puzzle.numDigits; i++)
                {
                    if ((bitmap & (1 << i)) != 0)
                    {
                        return puzzle.digits[i];
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
                    result = RepeatChar('X', puzzle.numDigits);
                }
                else if (IsFixed())
                {
                    Digit digit = FixedDigit();
                    result = RepeatChar(digit.display, puzzle.digits.Length);
                }
                else
                {
                    foreach (Digit digit in puzzle.digits)
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

            public Mask(Puzzle puzzle, int ordinal)
            {
                this.puzzle = puzzle;
                bitmap = 1 << ordinal;
                bitsInMask = 1;
            }

            public Mask(Puzzle puzzle)
            {
                this.puzzle = puzzle;
                if (puzzle.numDigits > 64)
                {
                    throw new Exception("Maximum is 64 digits");
                }

                if (puzzle.numDigits == 64)
                {
                    bitmap = Int64.MaxValue;
                }
                else
                {
                    bitmap = (1 << puzzle.numDigits) - 1;
                }
                bitsInMask = puzzle.numDigits;
            }
        }

        /// <summary>
        /// A digit is the abstraction for the symbol to display. It also has the mask
        /// that represent that single digit.
        /// </summary>
        public class Digit
        {
            private Puzzle puzzle;
            public char display;
            public Mask mask;

            public Digit(Puzzle puzzle, int ordinal)
            {
                this.puzzle = puzzle;
                display = puzzle.DigitToDisplay(ordinal);
                mask = new Mask(puzzle, ordinal);
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
            Box
        };

        public class GroupElement
        {
            public GroupType groupType;
            public int groupOrdinal;
            public int inGroupOrdinal;

            public GroupElement(GroupType groupType, int groupOrdinal, int inGroupOrdinal)
            {
                this.groupType = groupType;
                this.groupOrdinal = groupOrdinal;
                this.inGroupOrdinal = inGroupOrdinal;
            }

            public override string ToString()
            {
                string result = $"{groupType}[{groupOrdinal},{inGroupOrdinal}]";
                return result;
            }
        }

        /// <summary>
        /// A group is the array of cells that are joined either
        /// horizontally, vertically or in a box.
        /// </summary>
        public class Group
        {
            public Puzzle puzzle;
            public GroupType groupType;
            public int ordinal;
            public Cell[] cells;

            public Group(Puzzle puzzle, GroupType groupType, int ordinal)
            {
                this.puzzle = puzzle;
                this.groupType = groupType;
                this.ordinal = ordinal;
                cells = new Cell[puzzle.numDigits];
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
            public Puzzle puzzle;
            public Mask mask;
            public int ordinal;
            public GroupElement[] groupOrdinals = new GroupElement[sizeof(GroupType)];

            public void SetCell(Digit digit)
            {
                mask.MaskForOnly(digit);
            }

            public Cell(Puzzle puzzle, int ordinal)
            {
                this.puzzle = puzzle;
                this.ordinal = ordinal;
                mask = new Mask(puzzle);

                int x = ordinal % puzzle.numDigits;
                int y = ordinal / puzzle.numDigits;
                groupOrdinals[(int)GroupType.Row] = new GroupElement(GroupType.Row, y, x);
                groupOrdinals[(int)GroupType.Column] = new GroupElement(GroupType.Column, x, y);
                int boxX = (int)(x / puzzle.boxWidth);
                int boxY = (int)(y / puzzle.boxHeight);
                int boxXOffset = x % puzzle.boxWidth;
                int boxYOffset = y % puzzle.boxHeight;
                groupOrdinals[(int)GroupType.Box] = new GroupElement(GroupType.Box, boxX + boxY * puzzle.boxHeight, boxXOffset + boxYOffset * puzzle.boxWidth);

                for (int i = 0; i < puzzle.countGroupTypes; i++)
                {
                    GroupElement groupElement = groupOrdinals[i];
                    puzzle.groups[i, groupElement.groupOrdinal].cells[groupElement.inGroupOrdinal] = this;
                }
            }

            public string At()
            {
                int x = ordinal % puzzle.numDigits;
                int y = ordinal / puzzle.numDigits;
                string result = $"[{x + 1},{y + 1}]";
                return result;
            }

            public override string ToString()
            {
                string result = $"{ordinal.ToString()}=";
                result += mask.ToString();
                for (int i = 0; i < puzzle.countGroupTypes; i++)
                {
                    GroupType groupType = (GroupType)i;
                    result += $", {groupType}[{groupOrdinals[i].groupOrdinal},{groupOrdinals[i].inGroupOrdinal}]";
                }
                return result;
            }
        }

        public Puzzle(int boxWidthParam, int boxHeightParam)
        {
            Puzzle puzzle = this;
            boxWidth = boxWidthParam;
            boxHeight = boxHeightParam;
            numDigits = boxHeight * boxWidth;
            digitsUnknown = numDigits * numDigits;

            digits = new Digit[numDigits];
            for (int i = 0; i < numDigits; i++)
            {
                digits[i] = new Digit(puzzle, i);
            }

            //
            // WARNING: Groups' construction must be done before cell.
            // Cell constructor will set the cell values in the group.
            //
            countGroupTypes = CountOfEnum<GroupType>();
            groups = new Group[countGroupTypes, numDigits];
            for (int i = 0; i < countGroupTypes; i++)
            {
                for (int j = 0; j < numDigits; j++)
                {
                    groups[i, j] = new Group(puzzle, (GroupType)i, j);
                }
            }

            cells = new Cell[numDigits * numDigits];
            for (int i = 0; i < numDigits * numDigits; i++)
            {
                cells[i] = new Cell(puzzle, i);
            }

            ui = new UIConsole(puzzle);
        }
        public override string ToString()
        {
            string result = $"{boxWidth}x{boxHeight} with {digitsUnknown} unknown";
            return result;
        }
    }
}
