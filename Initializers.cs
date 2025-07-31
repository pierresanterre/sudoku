using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sudoku
{
    public partial class Puzzle
    {
        public class InitialCellDigit
        {
            public int x;
            public int y;
            public char display;

            public InternalInitialCellDigit ToInternalInitialCellDigit(Puzzle puzzle)
            {
                x = x - 1;
                if (x < 0 || x >= puzzle.numDigits)
                {
                    throw new Exception($"Invalid initial cell: {this}");
                }
                y = y - 1;
                if (y < 0 || y >= puzzle.numDigits)
                {
                    throw new Exception($"Invalid initial cell: {this}");
                }
                int cellOrdinal = x + (y * puzzle.numDigits);
                InternalInitialCellDigit result = new InternalInitialCellDigit(puzzle, cellOrdinal, display);
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

        internal InternalInitialCellDigit[] StringsToInternalInitialCellDigits(Puzzle puzzle, string[] lines)
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
                        internalInitialCellDigits.Add(new InternalInitialCellDigit(puzzle, (i * numDigits) + j, line[j]));
                    }
                }
            }

            InternalInitialCellDigit[] result = internalInitialCellDigits.ToArray();
            return result;
        }
    }
}
