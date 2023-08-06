using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    // https://www.chessprogramming.org/PeSTO%27s_Evaluation_Function
    private readonly ulong[] _pieceValues = new ulong[]
    {
        // PAWN
        //
        // Intrinsic value (middle game, end game)
        82, 94,

        // Game phase weight
        0,

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

        // Game phase weight
        1,

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

        // Game phase weight
        1,

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

        // Game phase weight
        2,

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

        // Game phase weight
        4,

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

        // Game phase weight
        0,

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

    // TODO Convert into a normal array? Might be more efficient
    private readonly Dictionary<ulong, ScoredMove> _cachedBestMoves = new();

    public struct ScoredMove
    {
        public ScoredMove(Move wrappedMove) => move = wrappedMove;

        public Move move;

        public int searchScore = -10000000; // To be set later by the function SearchForBestScoredMove(...)
        public int searchAccuracy = 0; // To be set later by the function SearchForBestScoredMove(...)
        public int searchResultType = 0; // To be set later by the function SearchForBestScoredMove(...)
    }

    public Move Think(Board board, Timer timer)
    {
        // Get an initial result by searching with a maximum depth of 3 and no time limit (this is to ensure the search completes)
        var maxDepth = 3;
        var (_, bestScoredMove) = SearchForBestScoredMove(board, timer, int.MaxValue, maxDepth);

        // Calculate the maximum time we can elapse this turn, assuming that there will be less remaining moves the fewer pieces there are left
        var turnTimeLimit = timer.MillisecondsElapsedThisTurn + (timer.MillisecondsRemaining / (20 + BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard)));

        // Search for the best move over and over with a bigger maximum depth on every iteration until we run out of time
        while (++maxDepth < 30 && timer.MillisecondsElapsedThisTurn < turnTimeLimit)
        {
            // * If the search was fully completed, we just take the result as the new best, most-accurately scored move.
            // * If the search was only partially completed because it ran out of time, we take the result, but only if it's
            //   better than the current one. This is because there is a chance that the move, despite having been fully
            //   studied, is not actually the best possible one (not all moves are studied in a partially completed search).
            var (searchStatus, newBestScoredMove) = SearchForBestScoredMove(board, timer, turnTimeLimit, maxDepth);
            if (searchStatus == 0 || (searchStatus == 1 && newBestScoredMove.searchScore > bestScoredMove.searchScore))
//            {
                bestScoredMove = newBestScoredMove;
//                Console.WriteLine($"Move score = {bestScoredMove.gameScore}\nMax depth  : {maxDepth} ({(searchStatus == 0 ? "full search" : "partial search")})");
//            }
//            else
//            {
//                Console.WriteLine($"Move score = {bestScoredMove.gameScore}\nMax depth  : {maxDepth - 1} (full search; search with max depth {maxDepth} cancelled)");
//            } 

        }

//        Console.WriteLine($"Score of chosen move = {bestScoredMove.gameScore}\n");
        return bestScoredMove.move;
    }

    // Negamax algorithm that will go down the search tree until reaching the maximum depth, playing turns to try to find the best possible move.
    //
    // The second return parameter is the actual move (scored), but the first one indicates the search completion status:
    //  * 0 = The search completed, so the returned move is valid, and the best one theoretically for the given maximum depth.
    //        Note: the search might not have actually completed because of pruning, but in that case its result will be ignored anyway.
    //  * 1 = The returned move is valid, but the search didn't have time to complete fully, so not all possible moves were studied.
    //        This means that the returned move might not actually be the best one of all the possible moves.
    //  * 2 = The returned move is not valid and must be discarded, as the search didn't have time to fully study any of the possible moves.
    public (int, ScoredMove) SearchForBestScoredMove(Board board, Timer timer, int turnTimeLimit, int maxDepth, int depth = 1, int bestScoreSoFar = -10000000, int bestOpponentScoreSoFar = -10000000, bool onlyCaptures = false)
    {
        var searchAccuracy = 1 + Math.Max(0, maxDepth - depth);

        if (!onlyCaptures && _cachedBestMoves.TryGetValue(board.ZobristKey, out var cachedScoredMove))
        {
            // If this board state has already come up before and we have cached a move for it with enough accuracy, return that result directly to avoid doing any calculations.
            // In case the move score was not exact, but a result of the pruning mechanisms (which means that the move might not be the best possible one for this board state),
            // we check if the move score actually causes pruning here; and if so, we just return the move as the search result.
            var searchResultType = cachedScoredMove.searchResultType;
            if (cachedScoredMove.searchAccuracy >= searchAccuracy &&
                (searchResultType == 0 ||
                    (searchResultType == 1 && cachedScoredMove.searchScore >= -bestOpponentScoreSoFar) ||
                    (searchResultType == 2 && cachedScoredMove.searchScore <= bestScoreSoFar)))
                return (0, cachedScoredMove);
        }
        else cachedScoredMove = default;

        // Get moves sorted by their potential score to make sure we prune bad branches as early as possible
        var isWhite = board.IsWhiteToMove;
        var isRoughlyEndGame = BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard) <= 12;
        var opponentKingSquare = board.GetKingSquare(!isWhite);
        var sortedMoves = board
            .GetLegalMoves(onlyCaptures)
            .OrderByDescending(move =>
                // If this move is stored in the cache as the best one for the current board state, we give it a huge potential score to make sure it is the first one we study,
                // as the chances of it being the best move on this new search are actually very high.
                move == cachedScoredMove.move ? 10000000

                // Otherwise, we calculate the potential score of the move like this:
                // + Score of the piece after moving (divided by 2 if the target square is attacked, as there is a chance the piece will be captured)
                // - Score of the piece before moving
                // + Score of the captured piece, multiplied by 10 to prioritise captures above all (the king is counted as a "captured" piece with a value of 5000)
                : ((CalculatePieceScore(move.IsPromotion ? move.PromotionPieceType : move.MovePieceType, move.TargetSquare, isWhite, isRoughlyEndGame) / (board.SquareIsAttackedByOpponent(move.TargetSquare) ? 2 : 1))
                    - CalculatePieceScore(move.MovePieceType, move.StartSquare, isWhite, isRoughlyEndGame)
                    + (move.IsCapture ? (10 * CalculatePieceScore(move.CapturePieceType, move.TargetSquare, !isWhite, isRoughlyEndGame)) : (opponentKingSquare == move.TargetSquare ? 50000 : 0)))
            )
            .ToArray();

        // In case of victory, the score will be higher the faster we achieve it – that way, we avoid never-ending endgames where the bot chooses a distant victory on each turn
        var immediateCheckmateScore = 10000000 - depth;

        var bestScoredMove = new ScoredMove(sortedMoves.FirstOrDefault());
        var numberOfFullyStudiedMoves = 0;
        var timeoutCancellation = false;

        // Iterate over each candidate move to calculate its score, stopping whenever we run out of moves or time
        while (numberOfFullyStudiedMoves < sortedMoves.Length && !(timeoutCancellation = timer.MillisecondsElapsedThisTurn >= turnTimeLimit))
        {
            var candidateMove = sortedMoves[numberOfFullyStudiedMoves];
            var scoredCandidateMove = new ScoredMove(candidateMove);
            var recaptureDetected = candidateMove.IsCapture && board.SquareIsAttackedByOpponent(candidateMove.TargetSquare);

            board.MakeMove(candidateMove);

            if (board.IsInCheckmate())
                scoredCandidateMove.searchScore = immediateCheckmateScore;
            else if (board.IsRepeatedPosition() || board.IsDraw())
                scoredCandidateMove.searchScore = 0;
            else if (depth < maxDepth || board.IsInCheck() || recaptureDetected)
            {
                // We keep searching down the tree because either of these was true:
                // * We haven't reached the maximum depth yet.
                // * The opponent is in check, so we keep going to avoid the horizon effect.
                // * We have detected a recapture, so we keep going to avoid the horizon effect (in this case, we only keep going while there are recaptures).
                var (searchStatus, opponentBestMove) = SearchForBestScoredMove(board, timer, turnTimeLimit, maxDepth, depth + 1, bestOpponentScoreSoFar, bestScoreSoFar, depth >= maxDepth && !board.IsInCheck());

                // We take the negative of the score obtained by the lower node as the score for this move because what's good for the opponent is bad for us
                scoredCandidateMove.searchScore = -opponentBestMove.searchScore;
                timeoutCancellation = searchStatus != 0;
            }
            else
                // We cannot search further down the tree, time to calculate the score with heuristics
                scoredCandidateMove.searchScore = CalculateHeuristicScore(board, isWhite);

            board.UndoMove(candidateMove);

            if (!timeoutCancellation)
            {
                numberOfFullyStudiedMoves++;

                // This candidate move was fully studied because the search didn't time out, so we need to see if it is actually the new best move
                if (scoredCandidateMove.searchScore > bestScoredMove.searchScore)
                {
                    bestScoreSoFar = Math.Max((bestScoredMove = scoredCandidateMove).searchScore, bestScoreSoFar);

                    // For efficiency, we prune branches here. Basically, there is no need to continue with the search on this branch if either of these is true:
                    // * The ideal move (a checkmate) has already been found at this level of the search tree.
                    // * The best score we have found so far on this branch is too high, as the opponent has already found a move that would give us a lower score.
                    //   Therefore, the scenario we are studying won't ever happen becuse the opponent already knows a way to make us perform worse than here.
                    if (bestScoreSoFar >= immediateCheckmateScore || bestScoreSoFar >= -bestOpponentScoreSoFar)
                        break;
                }
            }
        }

        // This will indicate if the search was fully valid, partially valid, or fully invalid (more info on the description of the search method)
        var searchCompletionState = timeoutCancellation ? (numberOfFullyStudiedMoves > 0 ? 1 : 2) : 0;

        if (sortedMoves.Length == 0)
            // No legal moves were found, but we still have to return a score for the search to work properly.
            // This might happen when searching for captures only, as the previous move might have left us with no capturing moves available here.
            bestScoredMove.searchScore = CalculateHeuristicScore(board, isWhite);
        else if (searchCompletionState == 0 && !onlyCaptures)
        {
            // The search finished without timeouts, so we save the result on the cache, noting its type:
            //  * 1 if the score was too high (i.e., higher than the negative of the opponent, meaning that the opponent would never allow us to get here).
            //  * 2 if the score was too low (i.e., lower than the best score found so far, meaning we would never choose this branch).
            //  * 0 if the score was neither too high nor too low because it was within range.
            bestScoredMove.searchResultType = bestScoreSoFar >= -bestOpponentScoreSoFar ? 1 : (bestScoredMove.searchScore <= bestScoreSoFar ? 2 : 0);
            bestScoredMove.searchAccuracy = searchAccuracy;
            _cachedBestMoves[board.ZobristKey] = bestScoredMove;
        }

        return (searchCompletionState, bestScoredMove);
    }

    // Calculations:
    // + The total score of all our pieces combined (weighted by game phase)
    // - The total score of all the opponent's pieces combined (weighted by game phase)
    public int CalculateHeuristicScore(Board board, bool isWhite)
    {
        int middleGameScore = 0,
            endGameScore = 0,
            gamePhaseWeight = 0,
            squareIndex;

        var allPiecesBitboard = board.AllPiecesBitboard;
        while ((squareIndex = BitboardHelper.ClearAndGetIndexOfLSB(ref allPiecesBitboard)) < 64)
        {
            Square pieceSquare = new(squareIndex);
            var pieceInSquare = board.GetPiece(pieceSquare);
            var pieceType = pieceInSquare.PieceType;
            var isWhitePiece = pieceInSquare.IsWhite;
            var colorSignModifier = isWhitePiece == isWhite ? 1 : -1;

            middleGameScore += colorSignModifier * CalculatePieceScore(pieceType, pieceSquare, isWhitePiece, false);
            endGameScore += colorSignModifier * CalculatePieceScore(pieceType, pieceSquare, isWhitePiece, true);
            gamePhaseWeight += (int)_pieceValues[2 + (19 * (((int)pieceType) - 1))];
        }

        return (middleGameScore * gamePhaseWeight) + (endGameScore * (24 - gamePhaseWeight));
    }

    // Calculations:
    // + The intrinsic value of the piece (will be different during the end game)
    // + Its position value (will also be different during the end game)
    public int CalculatePieceScore(PieceType piece, Square position, bool isWhite, bool isEndGame)
    {
        var pieceOffset = 19 * (((int)piece) - 1);
        var endGameOffset = isEndGame ? 1 : 0;
        return (int)_pieceValues[pieceOffset + endGameOffset]
            + ((sbyte)(_pieceValues[3 + pieceOffset + (endGameOffset * 8) + (isWhite ? (7 - position.Rank) : position.Rank)] >> (8 * (isWhite ? (7 - position.File) : position.File))));
    }
}