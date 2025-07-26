namespace sudoku
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Core core = new Core(3, 2);
            Core.InitialCellDigit[] initialCellDigits =
            [
                new Core.InitialCellDigit(1, 2, '5'),
                new Core.InitialCellDigit(4, 1, '2'),
            ];

            core.Solve(initialCellDigits);
            return 0;
        }
    }
}
