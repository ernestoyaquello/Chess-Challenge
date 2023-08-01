using ChessChallenge.API;
using Timer = ChessChallenge.API.Timer;

public class MyBotTest
{
    [Fact]
    public void TestBotCheckmatesInOne()
    {
        var bot = new MyBot();
        var board = Board.CreateBoardFromFEN("8/7k/5K2/6Q1/8/8/8/8 b - - 0 1");
        var exepectedMoves = new Move[] {
            new("h7h8", board),
            new("g5g7", board),
        };

        foreach (var exepectedMove in exepectedMoves)
        {
            var move = bot.Think(board, new Timer(30000)); // might need to be bigger in some machines

            Assert.Equal(move.ToString(), exepectedMove.ToString());

            board.MakeMove(move);
        }

        Assert.True(board.IsInCheckmate());
    }

    [Fact]
    public void TestBotCheckmatesInTwo()
    {
        var bot = new MyBot();
        var board = Board.CreateBoardFromFEN("kbK5/pp6/1P6/8/8/8/8/R7 w - - 0 1");
        var exepectedMoves = new Move[] {
            new("a1a6", board),
            new("b7a6", board),
            new("b6b7", board),
        };

        foreach (var exepectedMove in exepectedMoves)
        {
            var move = bot.Think(board, new Timer(60000)); // might need to be bigger in some machines

            Assert.Equal(move.ToString(), exepectedMove.ToString());

            board.MakeMove(move);
        }

        Assert.True(board.IsInCheckmate());
    }

    [Fact]
    public void TestBotCheckmatesInThree()
    {
        var bot = new MyBot();
        var board = Board.CreateBoardFromFEN("1r2r3/1q1n1p1k/p1b1pp2/3pP3/1b6/2N1BBQ1/1PP3PP/3R3K w - - 0 2");
        var exepectedMoves = new Move[] {
            new("d1d4", board),
            new("b4f8", board),
            new("d4h4", board),
            new("f8h6", board),
            new("h4h6", board),
        };

        foreach (var exepectedMove in exepectedMoves)
        {
            var move = bot.Think(board, new Timer(120000)); // might need to be bigger in some machines

            Assert.Equal(move.ToString(), exepectedMove.ToString());

            board.MakeMove(move);
        }

        Assert.True(board.IsInCheckmate());
    }

    [Fact]
    public void TestHeuristicScoresAreSameButInvertedForBothColors()
    {
        var bot = new MyBot();
        var boards = new[] {
            Board.CreateBoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"),
            Board.CreateBoardFromFEN("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR b KQkq - 0 1"),
            Board.CreateBoardFromFEN("1r2r3/1q1n1p1k/p1b1pp2/3pP3/1b6/2N1BBQ1/1PP3PP/3R3K w - - 0 2"),
            Board.CreateBoardFromFEN("1r1r4/Rp2np2/3k4/3P3p/2Q2p2/2P4q/1P1N1P1P/6RK w - - 1 0"),
            Board.CreateBoardFromFEN("5rk1/p3br1p/8/2p3p1/N2pPB2/1P1P1qPb/P2Q3P/R3R1K1 b - - 0 1"),
            Board.CreateBoardFromFEN("3r3r/ppk2B2/2n1Q1pp/5p2/3p4/q5P1/5P1P/1RR3K1 w - - 1 0"),
            Board.CreateBoardFromFEN("5rk1/1R4b1/3p4/1P1P4/4Pp2/3B1Pnb/PqRK1Q2/8 b - - 0 1"),
        };

        foreach (var board in boards)
        {
            Assert.Equal(
                bot.CalculateHeuristicScore(board, isWhite: true),
                -bot.CalculateHeuristicScore(board, isWhite: false)
            );
        }
    }

    [Fact]
    public void TestPawnPieceValuesMiddleGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: false,
        pieceType: PieceType.Pawn,
        expectedIntrinsicValue: 82,
        expectedModifiers: new int[][]
        {
            new int[] {   0,   0,   0,   0,   0,   0,  0,   0, },
            new int[] {  98, 127,  61,  95,  68, 126, 34, -11, },
            new int[] {  -6,   7,  26,  31,  65,  56, 25, -20, },
            new int[] { -14,  13,   6,  21,  23,  12, 17, -23, },
            new int[] { -27,  -2,  -5,  12,  17,   6, 10, -25, },
            new int[] { -26,  -4,  -4, -10,   3,   3, 33, -12, },
            new int[] { -35,  -1, -20, -23, -15,  24, 38, -22, },
            new int[] {   0,   0,   0,   0,   0,   0,  0,   0, },
        }
    );

    [Fact]
    public void TestPawnPieceValuesEndGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: true,
        pieceType: PieceType.Pawn,
        expectedIntrinsicValue: 94,
        expectedModifiers: new int[][]
        {
            new int[] {   0,   0,   0,   0,   0,   0,   0,   0, },
            new int[] { 127, 127, 127, 127, 127, 127, 127, 127, },
            new int[] {  94, 100,  85,  67,  56,  53,  82,  84, },
            new int[] {  32,  24,  13,   5,  -2,   4,  17,  17, },
            new int[] {  13,   9,  -3,  -7,  -7,  -8,   3,  -1, },
            new int[] {   4,   7,  -6,   1,   0,  -5,  -1,  -8, },
            new int[] {  13,   8,   8,  10,  13,   0,   2,  -7, },
            new int[] {   0,   0,   0,   0,   0,   0,   0,   0, },
        }
    );

    [Fact]
    public void TestKnightPieceValuesMiddleGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: false,
        pieceType: PieceType.Knight,
        expectedIntrinsicValue: 337,
        expectedModifiers: new int[][]
        {
            new int[] { -127, -89, -34, -49,  61, -97, -15, -107, },
            new int[] {  -73, -41,  72,  36,  23,  62,   7,  -17, },
            new int[] {  -47,  60,  37,  65,  84, 127,  73,   44, },
            new int[] {   -9,  17,  19,  53,  37,  69,  18,   22, },
            new int[] {  -13,   4,  16,  13,  28,  19,  21,   -8, },
            new int[] {  -23,  -9,  12,  10,  19,  17,  25,  -16, },
            new int[] {  -29, -53, -12,  -3,  -1,  18, -14,  -19, },
            new int[] { -105, -21, -58, -33, -17, -28, -19,  -23, },
        }
    );

    [Fact]
    public void TestKnightPieceValuesEndGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: true,
        pieceType: PieceType.Knight,
        expectedIntrinsicValue: 281,
        expectedModifiers: new int[][]
        {
            new int[] { -58, -38, -13, -28, -31, -27, -63, -99, },
            new int[] { -25,  -8, -25,  -2,  -9, -25, -24, -52, },
            new int[] { -24, -20,  10,   9,  -1,  -9, -19, -41, },
            new int[] { -17,   3,  22,  22,  22,  11,   8, -18, },
            new int[] { -18,  -6,  16,  25,  16,  17,   4, -18, },
            new int[] { -23,  -3,  -1,  15,  10,  -3, -20, -22, },
            new int[] { -42, -20, -10,  -5,  -2, -20, -23, -44, },
            new int[] { -29, -51, -23, -15, -22, -18, -50, -64, },
        }
    );

    [Fact]
    public void TestBishopPieceValuesMiddleGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: false,
        pieceType: PieceType.Bishop,
        expectedIntrinsicValue: 365,
        expectedModifiers: new int[][]
        {
            new int[] { -29,   4, -82, -37, -25, -42,   7,  -8, },
            new int[] { -26,  16, -18, -13,  30,  59,  18, -47, },
            new int[] { -16,  37,  43,  40,  35,  50,  37,  -2, },
            new int[] {  -4,   5,  19,  50,  37,  37,   7,  -2, },
            new int[] {  -6,  13,  13,  26,  34,  12,  10,   4, },
            new int[] {   0,  15,  15,  15,  14,  27,  18,  10, },
            new int[] {   4,  15,  16,   0,   7,  21,  33,   1, },
            new int[] { -33,  -3, -14, -21, -13, -12, -39, -21, },
        }
    );

    [Fact]
    public void TestBishopPieceValuesEndGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: true,
        pieceType: PieceType.Bishop,
        expectedIntrinsicValue: 297,
        expectedModifiers: new int[][]
        {
            new int[] { -14, -21, -11,  -8, -7,  -9, -17, -24, },
            new int[] {  -8,  -4,   7, -12, -3, -13,  -4, -14, },
            new int[] {   2,  -8,   0,  -1, -2,   6,   0,   4, },
            new int[] {  -3,   9,  12,   9, 14,  10,   3,   2, },
            new int[] {  -6,   3,  13,  19,  7,  10,  -3,  -9, },
            new int[] { -12,  -3,   8,  10, 13,   3,  -7, -15, },
            new int[] { -14, -18,  -7,  -1,  4,  -9, -15, -27, },
            new int[] { -23,  -9, -23,  -5, -9, -16,  -5, -17, },
        }
    );

    [Fact]
    public void TestRookPieceValuesMiddleGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: false,
        pieceType: PieceType.Rook,
        expectedIntrinsicValue: 477,
        expectedModifiers: new int[][]
        {
            new int[] {  32,  42,  32,  51, 63,  9,  31,  43, },
            new int[] {  27,  32,  58,  62, 80, 67,  26,  44, },
            new int[] {  -5,  19,  26,  36, 17, 45,  61,  16, },
            new int[] { -24, -11,   7,  26, 24, 35,  -8, -20, },
            new int[] { -36, -26, -12,  -1,  9, -7,   6, -23, },
            new int[] { -45, -25, -16, -17,  3,  0,  -5, -33, },
            new int[] { -44, -16, -20,  -9, -1, 11,  -6, -71, },
            new int[] { -19, -13,   1,  17, 16,  7, -37, -26, },
        }
    );

    [Fact]
    public void TestRookPieceValuesEndGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: true,
        pieceType: PieceType.Rook,
        expectedIntrinsicValue: 512,
        expectedModifiers: new int[][]
        {
            new int[] { 13, 10, 18, 15, 12,  12,   8,   5, },
            new int[] { 11, 13, 13, 11, -3,   3,   8,   3, },
            new int[] {  7,  7,  7,  5,  4,  -3,  -5,  -3, },
            new int[] {  4,  3, 13,  1,  2,   1,  -1,   2, },
            new int[] {  3,  5,  8,  4, -5,  -6,  -8, -11, },
            new int[] { -4,  0, -5, -1, -7, -12,  -8, -16, },
            new int[] { -6, -6,  0,  2, -9,  -9, -11,  -3, },
            new int[] { -9,  2,  3, -1, -5, -13,   4, -20, },
        }
    );

    [Fact]
    public void TestQueenPieceValuesMiddleGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: false,
        pieceType: PieceType.Queen,
        expectedIntrinsicValue: 1025,
        expectedModifiers: new int[][]
        {
            new int[] { -28,   0,  29,  12,  59,  44,  43,  45, },
            new int[] { -24, -39,  -5,   1, -16,  57,  28,  54, },
            new int[] { -13, -17,   7,   8,  29,  56,  47,  57, },
            new int[] { -27, -27, -16, -16,  -1,  17,  -2,   1, },
            new int[] {  -9, -26,  -9, -10,  -2,  -4,   3,  -3, },
            new int[] { -14,   2, -11,  -2,  -5,   2,  14,   5, },
            new int[] { -35,  -8,  11,   2,   8,  15,  -3,   1, },
            new int[] {  -1, -18,  -9,  10, -15, -25, -31, -50, },
        }
    );

    [Fact]
    public void TestQueenPieceValuesEndGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: true,
        pieceType: PieceType.Queen,
        expectedIntrinsicValue: 936,
        expectedModifiers: new int[][]
        {
            new int[] {  -9,  22,  22,  27,  27,  19,  10,  20, },
            new int[] { -17,  20,  32,  41,  58,  25,  30,   0, },
            new int[] { -20,   6,   9,  49,  47,  35,  19,   9, },
            new int[] {   3,  22,  24,  45,  57,  40,  57,  36, },
            new int[] { -18,  28,  19,  47,  31,  34,  39,  23, },
            new int[] { -16, -27,  15,   6,   9,  17,  10,   5, },
            new int[] { -22, -23, -30, -16, -16, -23, -36, -32, },
            new int[] { -33, -28, -22, -43,  -5, -32, -20, -41, },
        }
    );

    [Fact]
    public void TestKingPieceValuesMiddleGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: false,
        pieceType: PieceType.King,
        expectedIntrinsicValue: 0,
        expectedModifiers: new int[][]
        {
            new int[] { -65,  23,  16, -15, -56, -34,   2,  13, },
            new int[] {  29,  -1, -20,  -7,  -8,  -4, -38, -29, },
            new int[] {  -9,  24,   2, -16, -20,   6,  22, -22, },
            new int[] { -17, -20, -12, -27, -30, -25, -14, -36, },
            new int[] { -49,  -1, -27, -39, -46, -44, -33, -51, },
            new int[] { -14, -14, -22, -46, -44, -30, -15, -27, },
            new int[] {   1,   7,  -8, -64, -43, -16,   9,   8, },
            new int[] { -15,  36,  12, -54,   8, -28,  24,  14, },
        }
    );

    [Fact]
    public void TestKingPieceValuesEndGame() => AssertPieceValueIsCorrect(
        bot: new MyBot(),
        isEndGame: true,
        pieceType: PieceType.King,
        expectedIntrinsicValue: 0,
        expectedModifiers: new int[][]
        {
            new int[] { -74, -35, -18, -18, -11,  15,   4, -17, },
            new int[] { -12,  17,  14,  17,  17,  38,  23,  11, },
            new int[] {  10,  17,  23,  15,  20,  45,  44,  13, },
            new int[] {  -8,  22,  24,  27,  26,  33,  26,   3, },
            new int[] { -18,  -4,  21,  24,  27,  23,   9, -11, },
            new int[] { -19,  -3,  11,  21,  23,  16,   7,  -9, },
            new int[] { -27, -11,   4,  13,  14,   4,  -5, -17, },
            new int[] { -53, -34, -21, -11, -28, -14, -24, -43, },
        }
    );

    private static void AssertPieceValueIsCorrect(MyBot bot, bool isEndGame, PieceType pieceType, int expectedIntrinsicValue, int[][] expectedModifiers)
    {
        foreach (var isWhite in new[] { false, true })
        {
            for (var rankIndex = 0; rankIndex < 8; rankIndex++)
            {
                for (var fileIndex = 0; fileIndex < 8; fileIndex++)
                {
                    var expectedResult = expectedIntrinsicValue + expectedModifiers[isWhite ? rankIndex : (7 - rankIndex)][isWhite ? fileIndex : (7 - fileIndex)];

                    var square = new Square(fileIndex, 7 - rankIndex);
                    var result = bot.CalculatePieceScore(pieceType, square, isWhite, isEndGame);

                    Assert.Equal(result, expectedResult);
                }
            }
        }
    }
}
