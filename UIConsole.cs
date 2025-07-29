using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static sudoku.Core;
using static System.Net.Mime.MediaTypeNames;

namespace sudoku
{
    public class UIConsole : UI
    {
        private void WriteInColor(string text, ConsoleColor consoleColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            Console.Write(text);
            Console.ForegroundColor = oldColor;
        }

        private void DrawDigit(Digit digit, bool present)
        {
            if (present)
            {
                Console.Write(digit.display);
            }
            else
            {
                Console.Write(" ");
            }
        }

        private void DrawCell(Cell cell)
        {
            Mask mask = cell.mask;
            if (mask.IsImpossible())
            {
                WriteInColor(RepeatChar('X', core.digits.Length), ConsoleColor.DarkRed);
            }
            else if (mask.IsFixed())
            {
                Digit digit = mask.FixedDigit();
                WriteInColor(RepeatChar(digit.display, core.digits.Length), ConsoleColor.DarkGreen);
            }
            else
            {
                foreach (Digit digit in core.digits)
                {
                    DrawDigit(digit, mask.DigitPresent(digit));
                }
            }
        }

        public override void Log(string text, ConsoleColor consoleColor)
        {
            WriteInColor(text, consoleColor);
            Console.WriteLine();
        }

        public override void FullUI(Cell[] cells)
        {
            int cellOrdinal = 0;

            string horizontalLine = string.Empty;
            for (int box = 0; box < core.boxHeight; box++)
            {
                if (box != 0)
                {
                    horizontalLine += " + ";
                }
                horizontalLine += RepeatChar('-', (core.numDigits * core.boxWidth) + (core.boxWidth -1));
            }

            for (int yOuter = 0; yOuter < core.boxWidth; yOuter++)
            {
                if (yOuter != 0)
                {
                    Console.WriteLine(horizontalLine);
                }
                for (int yInner = 0; yInner < core.boxHeight; yInner++)
                {
                    for (int xOuter = 0; xOuter < core.boxHeight; xOuter++)
                    {
                        if (xOuter != 0)
                        {
                            Console.Write(" | ");
                        }
                        for (int xInner = 0; xInner < core.boxWidth; xInner++)
                        {
                            if (xInner != 0)
                            {
                                Console.Write("|");
                            }
                            DrawCell(cells[cellOrdinal]);
                            cellOrdinal++;
                        }
                    }
                    Console.WriteLine("");
                }
            }
        }
        public UIConsole(Core core) : base(core)
        {

        }
    }
}
