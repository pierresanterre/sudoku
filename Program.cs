namespace sudoku
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Puzzle puzzle;
            string[] solution;
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

            solution =
                [
                    "624135",
                    "513246",
                    "451362",
                    "236451",
                    "362514",
                    "145623",
                ];

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

            // Seattle Times Comics 8/10/2025
            puzzle = new Puzzle(3, 2);
            puzzle.Solve(
                [
                    "  5  3",
                    " 4  2 ",
                    " 13   ",
                    "   31 ",
                    " 3  4 ",
                    "5  2  ",
                ]);

            solution =
                [
                    "125463",
                    "346125",
                    "413652",
                    "652314",
                    "231546",
                    "564231",
                ];

            // Seattle Times 8/10/2025
            puzzle = new Puzzle(3, 3);
            puzzle.Solve(
                [
                    "   6    5",
                    "   41  9 ",
                    "      764",
                    "6  32   9",
                    " 9  8  5 ",
                    "8   59  2",
                    "982      ",
                    " 6  71   ",
                    "7    2   ",
                ]);
*/

            // Seattle Times Comics 8/17/2025
            puzzle = new Puzzle(3, 2);
            puzzle.Solve(
                [
                    "     1",
                    "31 6  ",
                    "   5  ",
                    "  2   ",
                    "  3 14",
                    "2     ",
                ]);

            solution =
                [
                    "625431",
                    "314625",
                    "136542",
                    "452163",
                    "563214",
                    "241356",
                ];


            // Seattle Times 8/17/2025
            puzzle = new Puzzle(3, 3);
            puzzle.Solve(
                [
                    " 1  62 7 ",
                    "6   7    ",
                    " 85     3",
                    "  9 317  ",
                    "2   5   9",
                    "  829 1  ",
                    "1     23 ",
                    "    4   5",
                    " 3 62  1 ",
                ]);

            solution =
                [
                    "913562478",
                    "642378591",
                    "785914623",
                    "469831752",
                    "271456389",
                    "358297146",
                    "196785234",
                    "827143965",
                    "534629817",
                ];

            return 0;
        }
    }
}
