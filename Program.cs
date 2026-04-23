namespace sudoku
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            string puzzleName;
            Puzzle puzzle;
            Puzzle.Solution solution;
            Puzzle.Solution expectedSolution;

            //puzzleName = "Seattle Times Comics 7/27/2025";
            //puzzle = new Puzzle(3, 2);
            //solution = puzzle.Solve(
            //    [
            //        "6    5",
            //        "  32  ",
            //        "  1   ",
            //        "   4  ",
            //        "  25  ",
            //        "1    3",
            //    ]);
            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "624135",
            //        "513246",
            //        "451362",
            //        "236451",
            //        "362514",
            //        "145623",
            //    ]);
            //solution.Verify(puzzleName, expectedSolution);

            //puzzleName = "Seattle Times 7/27/2025";
            //puzzle = new Puzzle(3, 3);
            ///*
            //puzzle.Solve(
            //    [
            //        "6    51  ",
            //        "   8149  ",
            //        "       5 ",
            //        " 92 6 4  ",
            //        "1   4   5",
            //        "  5 3 27 ",
            //        " 2       ",
            //        "  6257   ",
            //        "  43    2",
            //    ]);
            //*/
            //// Simplified
            //solution = puzzle.Solve(
            //    [
            //        "6  9 51  ",
            //        "   8149  ",
            //        "   6 3 5 ",
            //        "7925684  ",
            //        "1  742 95",
            //        "  513927 ",
            //        " 2 4     ",
            //        "  6257   ",
            //        "  43    2",
            //    ]);
            //// TO SOLVE


            //puzzleName = "Seattle Times Comics 8/10/2025";
            //puzzle = new Puzzle(3, 2);
            //solution = puzzle.Solve(
            //    [
            //        "  5  3",
            //        " 4  2 ",
            //        " 13   ",
            //        "   31 ",
            //        " 3  4 ",
            //        "5  2  ",
            //    ]);
            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "125463",
            //        "346125",
            //        "413652",
            //        "652314",
            //        "231546",
            //        "564231",
            //    ]);
            //solution.Verify(puzzleName, expectedSolution);

            //puzzleName = "Seattle Times 8/10/2025";
            //puzzle = new Puzzle(3, 3);
            //puzzle.Solve(
            //    [
            //        "   6    5",
            //        "   41  9 ",
            //        "      764",
            //        "6  32   9",
            //        " 9  8  5 ",
            //        "8   59  2",
            //        "982      ",
            //        " 6  71   ",
            //        "7    2   ",
            //    ]);
            //// TO SOLVE

            //puzzleName = "Seattle Times Comics 8/17/2025";
            //puzzle = new Puzzle(3, 2);
            //solution = puzzle.Solve(
            //    [
            //        "     1",
            //        "31 6  ",
            //        "   5  ",
            //        "  2   ",
            //        "  3 14",
            //        "2     ",
            //    ]);

            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "625431",
            //        "314625",
            //        "136542",
            //        "452163",
            //        "563214",
            //        "241356",
            //    ]);

            //solution.Verify(puzzleName, expectedSolution);

            //puzzleName = "Seattle Times 8/17/2025";
            //puzzle = new Puzzle(3, 3);
            //puzzle.Solve(
            //    [
            //        " 1  62 7 ",
            //        "6   7    ",
            //        " 85     3",
            //        "  9 317  ",
            //        "2   5   9",
            //        "  829 1  ",
            //        "1     23 ",
            //        "    4   5",
            //        " 3 62  1 ",
            //    ]);
            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "913562478",
            //        "642378591",
            //        "785914623",
            //        "469831752",
            //        "271456389",
            //        "358297146",
            //        "196785234",
            //        "827143965",
            //        "534629817",
            //    ]);
            //solution.Verify(puzzleName, expectedSolution);

            //puzzleName = "Seattle Times 8/24/2025";
            //puzzle = new Puzzle(3, 3);
            //puzzle.Solve(
            //    [
            //        "        8",
            //        "8   53   ",
            //        " 5 19  74",
            //        "    2539 ",
            //        "    6    ",
            //        " 1234    ",
            //        "96  14 5 ",
            //        "   53   7",
            //        "         ",
            //    ]);
            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "691472538",
            //        "874653129",
            //        "253198674",
            //        "746823391",
            //        "389761245",
            //        "512349786",
            //        "967214853",
            //        "428536917",
            //        "135987462",
            //    ]);
            //solution.Verify(puzzleName, expectedSolution);

            //puzzleName = "Seattle Times Comics 8/24/2025";
            //puzzle = new Puzzle(3, 2);
            //solution = puzzle.Solve(
            //    [
            //        "  35  ",
            //        "    43",
            //        "   2 6",
            //        "6 1   ",
            //        "16    ",
            //        "  26  ",
            //    ]);

            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "413562",
            //        "256143",
            //        "534216",
            //        "621435",
            //        "165324",
            //        "342651",
            //    ]);
            //solution.Verify(puzzleName, expectedSolution);

            //puzzleName = "Seattle Times 8/31/2025";
            //puzzle = new Puzzle(3, 3);
            //puzzle.Solve(
            //    [
            //        " 3 4     ",
            //        " 94      ",
            //        " 6  83   ",
            //        "25  9    ",
            //        " 19 3 84 ",
            //        "    5  69",
            //        "   51  86",
            //        "      12 ",
            //        "     6 3 ",
            //    ]);
            //expectedSolution = new Puzzle.Solution(true,
            //    [
            //        "832475691",
            //        "794261358",
            //        "165983472",
            //        "258694713",
            //        "619732845",
            //        "347158269",
            //        "423517986",
            //        "586349127",
            //        "971826534",
            //    ]);
            //solution.Verify(puzzleName, expectedSolution);

            puzzleName = "Seattle Times Comic Jason 11/16/2025";
            puzzle = new Puzzle(4, 4);
            solution = puzzle.Solve(
                [
                    "91C63 B2DE407 5A",
                    "8 E  4709A C2B 1",
                    "D 0 F 918B 6C E4",
                    "BA   DEC371F8690",
                    "8  0EA52D914CB6 ",
                    "02DCB61 AFE  973",
                    "659 2F 0CB A18D ",
                    "1B 48 D9 563E0F2",
                    "C0 BE 5F1  9 4 8",
                    "3 18C907 6A BD25",
                    "4D  A1 6B8C5073F",
                    "52A4 8 F0371E 9 ",
                    "961 7 E5 F D 0C ",
                    "C8   4AE1029    ",
                    "2 B095C8637 FA1 ",
                    "5EFD  6B 9 A3247",
                ]);
            expectedSolution = new Puzzle.Solution(true,
                [
                    "91C638B2DE407F5A",
                    "83EF64709A5C2BD1",
                    "D705FA918B26C3E4",
                    "BA425DEC371F8690",
                    "F8370EA52D914CB6",
                    "02DCB614AFE85973",
                    "E65972F30CB4A18D",
                    "1BA48CD97563E0F2",
                    "C07BE35F12D964A8",
                    "3F18C90746AEBD25",
                    "4D9EA126B8C5073F",
                    "652A4B8DF0371EC9",
                    "A961273E54FBD80C",
                    "7C83DF4AE102956B",
                    "24B095C8637DFA1E",
                    "5EFD106BC98A3247",
                ]);
            solution.Verify(puzzleName, expectedSolution);

            return 0;
        }
    }
}
