namespace sudoku
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // Seattle Times Comics 7/27/2025
            Puzzle puzzle;
            
            puzzle = new Puzzle(3, 2);
            Puzzle.InitialCellDigit[] initialCellDigits =
            [
                new Puzzle.InitialCellDigit(1, 1, '6'),
                new Puzzle.InitialCellDigit(6, 1, '5'),
                new Puzzle.InitialCellDigit(3, 2, '3'),
                new Puzzle.InitialCellDigit(4, 2, '2'),
                new Puzzle.InitialCellDigit(3, 3, '1'),
                new Puzzle.InitialCellDigit(4, 4, '4'),
                new Puzzle.InitialCellDigit(3, 5, '2'),
                new Puzzle.InitialCellDigit(4, 5, '5'),
                new Puzzle.InitialCellDigit(1, 6, '1'),
                new Puzzle.InitialCellDigit(6, 6, '3'),
            ];
            puzzle.Solve(initialCellDigits);

            puzzle = new Puzzle(3, 2);
            puzzle.Solve(
                [
                    "6    5",
                    "  32  ",
                    "  1   ",
                    "   4  ",
                    "  25  ",
                    "1    3",
                ]);

            return 0;
        }
    }
}
