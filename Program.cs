namespace sudoku
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            // Seattle Times Comics 7/27/2025
            Core core;
            
            core = new Core(3, 2);
            Core.InitialCellDigit[] initialCellDigits =
            [
                new Core.InitialCellDigit(1, 1, '6'),
                new Core.InitialCellDigit(6, 1, '5'),
                new Core.InitialCellDigit(3, 2, '3'),
                new Core.InitialCellDigit(4, 2, '2'),
                new Core.InitialCellDigit(3, 3, '1'),
                new Core.InitialCellDigit(4, 4, '4'),
                new Core.InitialCellDigit(3, 5, '2'),
                new Core.InitialCellDigit(4, 5, '5'),
                new Core.InitialCellDigit(1, 6, '1'),
                new Core.InitialCellDigit(6, 6, '3'),
            ];
            core.Solve(initialCellDigits);

            core = new Core(3, 2);
            core.Solve(
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
