using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

// Current bot version: 1.1
//
// ---
//
// Stats
//
// 1.1 vs 1.0:
//   * Total games:   640
//   * Victories:   + 83
//   * Draw:        = 522
//   * Defeats:     - 34
public class MyBot : IChessBot
{
    // https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
    private readonly ulong[] _pieceValues = new ulong[]
    {
        // PAWN
        //
        // Intrinsic value (middle game, end game)
        82, 94,

        // Position values in the middle game (board representation with the player at the bottom)
        //   0,   0,   0,   0,   0,   0,  0,   0,
        //  98, 127,  61,  95,  68, 126, 34, -11,
        //  -6,   7,  26,  31,  65,  56, 25, -20,
        // -14,  13,   6,  21,  23,  12, 17, -23,
        // -27,  -2,  -5,  12,  17,   6, 10, -25,
        // -26,  -4,  -4, -10,   3,   3, 33, -12,
        // -35,  -1, -20, -23, -15,  24, 38, -22,
        //   0,   0,   0,   0,   0,   0,  0,   0,
        0,
        7097459017139495669,
        18016397555859462636,
        17441603619526545897,
        16572959708013791975,
        16644456456397201908,
        15996764890959718122,
        0,

        // Position values in the end game (board representation with the player at the bottom)
        //   0,   0,   0,   0,   0,   0,   0,   0,
        // 127, 127, 127, 127, 127, 127, 127, 127,
        //  94, 100,  85,  67,  56,  53,  82,  84,
        //  32,  24,  13,   5,  -2,   4,  17,  17,
        //  13,   9,  -3,  -7,  -7,  -8,   3,  -1,
        //   4,   7,  -6,   1,   0,  -5,  -1,  -8,
        //  13,   8,   8,  10,  13,   0,   2,  -7,
        //   0,   0,   0,   0,   0,   0,   0,   0,
        0,
        9187201950435737471,
        6801655084430479956,
        2312612728042426641,
        939561247365923839,
        290475583207112696,
        939009361567548153,
        0,


        // KNIGHT
        //
        // Intrinsic value (middle game, end game)
        337, 281,

        // Position values in the middle game (board representation with the player at the bottom)
        // -127, -89, -34, -49,  61, -97, -15, -107,
        //  -73, -41,  72,  36,  23,  62,   7,  -17,
        //  -47,  60,  37,  65,  84, 127,  73,   44,
        //   -9,  17,  19,  53,  37,  69,  18,   22,
        //  -13,   4,  16,  13,  28,  19,  21,   -8,
        //  -23,  -9,  12,  10,  19,  17,  25,  -16,
        //  -29, -53, -12,  -3,  -1,  18, -14,  -19,
        // -105, -21, -58, -33, -17, -28, -19,  -23,
        9342680933676872085,
        13247136148779567087,
        15076966615050307884,
        17803031920951759382,
        17511138899614963192,
        16858956967493835248,
        16414482638625239789,
        10947061984358886889,

        // Position values in the endgame (board representation with the player at the bottom)
        // -58, -38, -13, -28, -31, -27, -63, -99,
        // -25,  -8, -25,  -2,  -9, -25, -24, -52,
        // -24, -20,  10,   9,  -1,  -9, -19, -41,
        // -17,   3,  22,  22,  22,  11,   8, -18,
        // -18,  -6,  16,  25,  16,  17,   4, -18,
        // -23,  -3,  -1,  15,  10,  -3, -20, -22,
        // -42, -20, -10,  -5,  -2, -20, -23, -44,
        // -29, -51, -23, -15, -22, -18, -50, -64,
        14329033328800678301,
        16715365099252476108,
        16783800949368417751,
        17222633684109822190,
        17220093825034290414,
        16860913020019010794,
        15487024780794456532,
        16415033442073235136,

        // BISHOP
        //
        // Intrinsic value (middle game, end game)
        365, 297,

        // Position values in the middle game (board representation with the player at the bottom)
        // -29,   4, -82, -37, -25, -42,   7,  -8,
        // -26,  16, -18, -13,  30,  59,  18, -47,
        // -16,  37,  43,  40,  35,  50,  37,  -2,
        //  -4,   5,  19,  50,  37,  37,   7,  -2,
        //  -6,  13,  13,  26,  34,  12,  10,   4,
        //   0,  15,  15,  15,  14,  27,  18,  10,
        //   4,  15,  16,   0,   7,  21,  33,   1,
        // -33,  -3, -14, -21, -13, -12, -39, -21,
        16358392006027118584,
        16578012956302447313,
        17304284594630174206,
        18159942178533869566,
        18018072090070747652,
        4238681986241034,
        292470093107241217,
        16140323734789872107,

        // Position values in the endgame (board representation with the player at the bottom)
        // -14, -21, -11,  -8, -7,  -9, -17, -24,
        //  -8,  -4,   7, -12, -3, -13,  -4, -14,
        //   2,  -8,   0,  -1, -2,   6,   0,   4,
        //  -3,   9,  12,   9, 14,  10,   3,   2,
        //  -6,   3,  13,  19,  7,  10,  -3,  -9,
        // -12,  -3,   8,  10, 13,   3,  -7, -15,
        // -14, -18,  -7,  -1,  4,  -9, -15, -27,
        // -23,  -9, -23,  -5, -9, -16,  -5, -17,
        17504354826400034792,
        17941223764351253746,
        213922081778565124,
        18233117799415939842,
        18015257309785816567,
        17653274953623271921,
        17505203675331031525,
        16859200998490569711,

        // ROOK
        //
        // Intrinsic value (middle game, end game)
        477, 512,

        // Position values in the middle game (board representation with the player at the bottom)
        //  32,  42,  32,  51, 63,  9,  31,  43,
        //  27,  32,  58,  62, 80, 67,  26,  44,
        //  -5,  19,  26,  36, 17, 45,  61,  16,
        // -24, -11,   7,  26, 24, 35,  -8, -20,
        // -36, -26, -12,  -1,  9, -7,   6, -23,
        // -45, -25, -16, -17,  3,  0,  -5, -33,
        // -44, -16, -20,  -9, -1, 11,  -6, -71,
        // -19, -13,   1,  17, 16,  7, -37, -26,
        2317700362708524843,
        1954626277587753516,
        18091832870286736656,
        16786330994748946668,
        15917679309208749801,
        15269437970961202143,
        15344024480331332281,
        17146049379124632550,

        // Position values in the endgame (board representation with the player at the bottom)
        // 13, 10, 18, 15, 12,  12,   8,   5,
        // 11, 13, 13, 11, -3,   3,   8,   3,
        //  7,  7,  7,  5,  4,  -3,  -5,  -3,
        //  4,  3, 13,  1,  2,   1,  -1,   2,
        //  3,  5,  8,  4, -5,  -6,  -8, -11,
        // -4,  0, -5, -1, -7, -12,  -8, -16,
        // -6, -6,  0,  2, -9,  -9, -11,  -3,
        // -9,  2,  3, -1, -5, -13,   4, -20,
        939583328096094213,
        796307054255081475,
        506381201242455037,
        289089099061657346,
        217588974497757429,
        18158790774386653424,
        18084767266409805309,
        17798793075300173036,

        // QUEEN
        //
        // Intrinsic value (middle game, end game)
        1025, 936,

        // Position values in the middle game (board representation with the player at the bottom)
        // -28,   0,  29,  12,  59,  44,  43,  45,
        // -24, -39,  -5,   1, -16,  57,  28,  54,
        // -13, -17,   7,   8,  29,  56,  47,  57,
        // -27, -27, -16, -16,  -1,  17,  -2,   1,
        //  -9, -26,  -9, -10,  -2,  -4,   3,  -3,
        // -14,   2, -11,  -2,  -5,   2,  14,   5,
        // -35,  -8,  11,   2,   8,  15,  -3,   1,
        //  -1, -18,  -9,  10, -15, -25, -31, -50,
        16429163379017132845,
        16778717872489307190,
        17577275602081689401,
        16565911722214424065,
        17863237612223595517,
        17438771182613696005,
        15994546179959422209,
        18441949150508999118,

        // Position values in the endgame (board representation with the player at the bottom)
        //  -9,  22,  22,  27,  27,  19,  10,  20,
        // -17,  20,  32,  41,  58,  25,  30,   0,
        // -20,   6,   9,  49,  47,  35,  19,   9,
        //   3,  22,  24,  45,  57,  40,  57,  36,
        // -18,  28,  19,  47,  31,  34,  39,  23,
        // -16, -27,  15,   6,   9,  17,  10,   5,
        // -22, -23, -30, -16, -16, -23, -36, -32,
        // -33, -28, -22, -43,  -5, -32, -20, -41,
        17804442482529995284,
        17227429836039462400,
        17007291149660132105,
        222391814112950564,
        17157609773481469719,
        17358296857365776901,
        16927310198910606560,
        16133277969922714839,

        // KING
        //
        // Intrinsic value (middle game, end game)
        0, 0,

        // Position values in the middle game (board representation with the player at the bottom)
        // -65,  23,  16, -15, -56, -34,   2,  13,
        //  29,  -1, -20,  -7,  -8,  -4, -38, -29,
        //  -9,  24,   2, -16, -20,   6,  22, -22,
        // -17, -20, -12, -27, -30, -25, -14, -36,
        // -49,  -1, -27, -39, -46, -44, -33, -51,
        // -14, -14, -22, -46, -44, -30, -15, -27,
        //   1,   7,  -8, -64, -43, -16,   9,   8,
        // -15,  36,  12, -54,   8, -28,  24,  14,
        13769493016351736333,
        2161706904529459939,
        17804984360584484586,
        17288462337760031452,
        14987950808620130253,
        17506312892778213861,
        74301425981589768,
        17376027324174309390,

        // Position values in the endgame (board representation with the player at the bottom)
        // -74, -35, -18, -18, -11,  15,   4, -17,
        // -12,  17,  14,  17,  17,  38,  23,  11,
        //  10,  17,  23,  15,  20,  45,  44,  13,
        //  -8,  22,  24,  27,  26,  33,  26,   3,
        // -18,  -4,  21,  24,  27,  23,   9, -11,
        // -19,  -3,  11,  21,  23,  16,   7,  -9,
        // -27, -11,   4,  13,  14,   4,  -5, -17,
        // -53, -34, -21, -11, -28, -14, -24, -43,
        13176950794836968687,
        17586853486323439371,
        725386368513813517,
        17876502275575323139,
        17220662268435827189,
        17148875141305862135,
        16570154858095901679,
        14690438475869776085,
    };

    // TODO Check if this takes too much memory and do something to stop it if that's the case
    private readonly Dictionary<ulong, (int, ScoredMove)> _cachedBestMoves = new()
    {
        // rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1 
        //{ 13227872743731781434, (999999, new("d2d4")) },

        // rnbqkbnr/pppppppp/8/8/3P4/8/PPP1PPPP/RNBQKBNR b KQkq d3 0 1 
        //{ 13920910881790336478, (999999, new("d7d5")) },

        // rnbqkbnr/ppp1pppp/8/3p4/3P4/8/PPP1PPPP/RNBQKBNR w KQkq d6 0 2 
        //{ 9628964799347499318, (999999, new("c2c4")) },
    };

    private struct ScoredMove
    {
        public ScoredMove() {}

        public ScoredMove(string name) => this.name = name;

        public ScoredMove(Move? move, int potentialScore) => (_move, this.potentialScore) = (move, potentialScore);

        private Move? _move = null;
        public int potentialScore = -999999; // Based only on the piece position change and the potential capture
        public int gameScore = -999999; // To be set later by the function CalculateBestMove(...)
        public string? name = null; // Will be null except for moves prerecorded inside _cachedBestMoves

        public readonly Move GetMove(Board board) => _move ?? new(name!, board);
    }

    public Move Think(Board board, Timer timer)
    {
        // Get an initial result by searching with a maximum depth of 4 without a time limit to ensure the search completes
        var (_, bestScoredMove) = SearchForBestScoredMove(board, timer, int.MaxValue, 4);

        // Calculate the maximum time to elapse on this turn (let's consider that victory is always at least 30 moves away; and, at most, 50)
        int turnTimeLimit = timer.MillisecondsElapsedThisTurn + (timer.MillisecondsRemaining / Math.Max(30, 50 - board.GameMoveHistory.Length));

        // Search for the best move over and over with a bigger maximum depth on every iteration until we run out of time
        for (int maxDepth = 5; maxDepth < 30 && timer.MillisecondsElapsedThisTurn < turnTimeLimit; maxDepth++)
        {
            // * If the search was fully completed, we just take the result as the new best, most-accurately scored move.
            // * If the search was only partially completed because it ran out of time, we take the result, but only if it's
            //   better than the current one. This is because there is a chance that the move, despite having been fully
            //   studied, is not actually the best possible one (not all moves are studied in a partially completed search).
            var (searchStatus, newBestScoredMove) = SearchForBestScoredMove(board, timer, turnTimeLimit, maxDepth);
            if (searchStatus == 0 || (searchStatus == 1 && newBestScoredMove.gameScore > bestScoredMove.gameScore))
                bestScoredMove = newBestScoredMove;
        }
        
        return bestScoredMove.GetMove(board);
    }

    // Simple negamax algorithm to search for the best possible move.
    //
    // The first parameter indicates:
    //  * 0 = The search completed, so the returned move is valid
    //  * 1 = The returned move is valid, but the search didn't have time to complete, so not all possible moves were studied (meaning that the returned one might not actually be the best one)
    //  * 2 = The returned move is not valid and must be discarded because the search didn't have time to fully study any of the possible moves
    private (int, ScoredMove) SearchForBestScoredMove(Board board, Timer timer, int turnTimeLimit, int maxDepth, int depth = 1, int bestOpponentScoreSoFar = -999999, bool onlyCaptures = false)
    {
        int searchAccuracy = 1 + Math.Max(0, maxDepth - depth);

        if (!onlyCaptures && _cachedBestMoves.TryGetValue(board.ZobristKey, out (int, ScoredMove) cachedScoredMove) && cachedScoredMove.Item1 >= searchAccuracy)
            return (0, cachedScoredMove.Item2);

        var numberOfPiecesLeft = BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard);
        var isEndGame = numberOfPiecesLeft <= 12; // not ideal, but simpler to calculate and write than more complex approaches
        var scoredMoves = board
            .GetLegalMoves(onlyCaptures)
            .Select(move => new ScoredMove(move, CalculateMovePotentialScore(move, board.IsWhiteToMove, isEndGame)))
            .OrderByDescending(m => m.potentialScore) // sort moves by potential to cut off branches early
            .ToArray();
        var bestMove = scoredMoves.FirstOrDefault(new ScoredMove());
        var numberOfFullyStudiedMoves = 0;
        var ranOutOfTimeToContinue = false;

        while (numberOfFullyStudiedMoves < scoredMoves.Length)
        {
            var scoredCandidateMove = scoredMoves[numberOfFullyStudiedMoves];
            var candidateMove = scoredCandidateMove.GetMove(board);
            var recaptureDetected = candidateMove.IsCapture && board.SquareIsAttackedByOpponent(candidateMove.TargetSquare);

            board.MakeMove(candidateMove);

            if (board.IsInCheckmate())
                scoredCandidateMove.gameScore = 999999;
            else if (board.IsDraw())
                scoredCandidateMove.gameScore = 0;
            else
            {
                // If the move didn't score high enough, it's likely a loser move that isn't worth studying for too long, so we reduce the maximum depth
                var newMaxDepth = Math.Max(4, maxDepth - (numberOfFullyStudiedMoves / 10));

                // We keep searching down the tree, as long as we haven't reached the maximum depth yet.
                // To try to avoid the horizon effect, we keep searching regardless of the maximum depth if:
                // * We have detected a recapture (in this case, we only keep going while there are recaptures).
                // * The player is in check.
                if (depth < newMaxDepth || recaptureDetected || board.IsInCheck())
                {
                    var (searchStatus, opponentBestMove) = SearchForBestScoredMove(board, timer, turnTimeLimit, newMaxDepth, depth + 1, bestMove.gameScore, recaptureDetected && depth >= newMaxDepth);
                    if (searchStatus == 0)
                        // If the returned move is valid, we take it into account with a negative value (what's good for the opponent is bad for us)
                        scoredCandidateMove.gameScore = -opponentBestMove.gameScore;
                    else
                        // If the inner search we just invoked didn't fully complete, we need to cancel this search too
                        ranOutOfTimeToContinue = true;
                }
                else
                    // We cannot search further down the tree, time to calculate the score with heuristics
                    scoredCandidateMove.gameScore = CalculateHeuristicScore(board, !board.IsWhiteToMove);
            }

            board.UndoMove(candidateMove);

            if (!ranOutOfTimeToContinue)
            {
                // This candidate move was fully studied because the search wasn't cancelled, so we need to see if it is actually the new best move
                numberOfFullyStudiedMoves++;
                if (scoredCandidateMove.gameScore > bestMove.gameScore)
                    bestMove = scoredCandidateMove;

                // If we have reached the time limit after this iteration without having studied all the possible moves, we must cancel the search
                ranOutOfTimeToContinue = timer.MillisecondsElapsedThisTurn >= turnTimeLimit && numberOfFullyStudiedMoves < scoredMoves.Length;

            }

            // There is no need to continue with the search on this branch if one of these is true:
            // * The ideal move (a checkmate) has already been found.
            // * The parent node has already found a move that would make us perform worse in this turn than we are performing now.
            //   Thus, our opponent will choose that move and not the one that would lead to the hypothetical situation we are analysing here.
            // * The search has been cancelled because it ran out of time.
            if (bestMove.gameScore == 999999 || bestOpponentScoreSoFar >= -bestMove.gameScore || ranOutOfTimeToContinue)
                break;
        }

        if (!ranOutOfTimeToContinue)
        {
            // No legal moves were found, but we still have to return a score for the search to work
            if (numberOfFullyStudiedMoves == 0)
                bestMove.gameScore = CalculateHeuristicScore(board, board.IsWhiteToMove);

            // Don't save anything to the cache unless we did a full search that included all possible moves
            if (!onlyCaptures)
                _cachedBestMoves[board.ZobristKey] = (searchAccuracy, bestMove);

            return (0, bestMove);
        }

        // The search was cancelled, so we just return the best move, indicating whether it is valid or not. For it to be valid,
        // it needs to have been fully studied, meaning that the moves loop finished at least one iteration without cancellations.
        return (numberOfFullyStudiedMoves > 0 ? 1 : 2, bestMove);
    }

    // Calculations:
    // + The total score of all our pieces combined
    // - The total score of all the opponent's pieces combined
    private int CalculateHeuristicScore(Board board, bool isWhite)
    {
        var heuristicScore = 0;
        var numberOfPiecesLeft = BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard);
        var isEndGame = numberOfPiecesLeft <= 12; // not ideal, but simpler to calculate and write than more complex approaches

        for (int squareIndex = 0, numberOfPiecesFound = 0; numberOfPiecesFound < numberOfPiecesLeft; squareIndex++)
        {
            Square square = new(squareIndex);
            var pieceInSquare = board.GetPiece(square);
            if (pieceInSquare.IsNull)
                continue;

            heuristicScore += (pieceInSquare.IsWhite == isWhite ? 1 : -1) * CalculatePieceScore(pieceInSquare.PieceType, square, pieceInSquare.IsWhite, isEndGame);
            numberOfPiecesFound++;
        }

        return heuristicScore;
    }

    // Calculations:
    // + Score of the piece after moving
    // - Score of the piece before moving
    // + Score of the captured piece multiplied by 100 (if any)
    private int CalculateMovePotentialScore(Move move, bool isWhite, bool isEndGame) =>
        CalculatePieceScore(move.IsPromotion ? move.PromotionPieceType : move.MovePieceType, move.TargetSquare, isWhite, isEndGame)
        - CalculatePieceScore(move.MovePieceType, move.StartSquare, isWhite, isEndGame)
        + (move.IsCapture ? (100 * CalculatePieceScore(move.CapturePieceType, move.TargetSquare, !isWhite, isEndGame)) : 0);

    // Calculations:
    // + The intrinsic value of the piece (will be different during the end game)
    // + Its position value (will also be different during the end game)
    private int CalculatePieceScore(PieceType piece, Square position, bool isWhite, bool isEndGame)
    {
        var pieceOffset = 18 * (((int)piece) - 1);
        var endGameOffset = isEndGame ? 1 : 0;
        return (int)_pieceValues[pieceOffset + endGameOffset]
            + ((sbyte)(_pieceValues[2 + pieceOffset + (endGameOffset * 8) + (isWhite ? (7 - position.Rank) : position.Rank)] >> (8 * (isWhite ? (7 - position.File) : position.File))));
    }
}