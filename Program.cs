namespace sudoku
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Puzzle puzzle;

/*
            // Seattle Times Comics 7/27/2025
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

            string[] solution =
                [
                    "624135",
                    "513246",
                    "451362",
                    "236451",
                    "362514",
                    "145623",
                ];
*/

            // Seattle Times 7/27/2025
            puzzle = new Puzzle(3, 3);
            puzzle.Solve(
                [
                    "6    51  ",
                    "   8149  ",
                    "       5 ",
                    " 92 6 4  ",
                    "1   4   5",
                    "  5 3 27 ",
                    " 2       ",
                    "  6257   ",
                    "  43    2",
                ]);

            puzzle = new Puzzle(3, 3);
            puzzle.Solve(
                [
                    "6  9 51  ",
                    "   8149  ",
                    "   6 3 5 ",
                    "7925684  ",
                    "1  742 95",
                    "  513927 ",
                    " 2 4     ",
                    "  6257   ",
                    "  43    2",
                ]);

            return 0;
        }
    }
}
