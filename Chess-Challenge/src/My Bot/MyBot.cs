using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    // TODO Simplify these with bits to reduce tokens
    private static readonly Dictionary<PieceType, int[][]> _piecePositionValues = new Dictionary<PieceType, int[][]>
    {
        {
            PieceType.Pawn,
            new int[9][]
            {
                // Intrinsic value
                new int[1] { 100 },

                // Position modifiers
                new int[8] {  0,  0,  0,  0,  0,  0,  0,  0, },
                new int[8] { 50, 50, 50, 50, 50, 50, 50, 50, },
                new int[8] { 10, 10, 20, 30, 30, 20, 10, 10, },
                new int[8] {  5,  5, 10, 25, 25, 10,  5,  5, },
                new int[8] {  0,  0,  0, 20, 20,  0,  0,  0, },
                new int[8] {  5, -5,-10,  0,  0,-10, -5,  5, },
                new int[8] {  5, 10, 10,-20,-20, 10, 10,  5, },
                new int[8] {  0,  0,  0,  0,  0,  0,  0,  0, },
             }
        },

        {
            PieceType.Knight,
            new int[9][]
            {
                // Intrinsic value
                new int[1] { 320 },

                // Position modifiers
                new int[8] { -50,-40,-30,-30,-30,-30,-40,-50, },
                new int[8] { -40,-20,  0,  0,  0,  0,-20,-40, },
                new int[8] { -30,  0, 10, 15, 15, 10,  0,-30, },
                new int[8] { -30,  5, 15, 20, 20, 15,  5,-30, },
                new int[8] { -30,  0, 15, 20, 20, 15,  0,-30, },
                new int[8] { -30,  5, 10, 15, 15, 10,  5,-30, },
                new int[8] { -40,-20,  0,  5,  5,  0,-20,-40, },
                new int[8] { -50,-40,-30,-30,-30,-30,-40,-50, },
             }
        },

        {
            PieceType.Bishop,
            new int[9][]
            {
                // Intrinsic value
                new int[1] { 333 },

                // Position modifiers
                new int[8] { -20,-10,-10,-10,-10,-10,-10,-20, },
                new int[8] { -10,  0,  0,  0,  0,  0,  0,-10, },
                new int[8] { -10,  0,  5, 10, 10,  5,  0,-10, },
                new int[8] { -10,  5,  5, 10, 10,  5,  5,-10, },
                new int[8] { -10,  0, 10, 10, 10, 10,  0,-10, },
                new int[8] { -10, 10, 10, 10, 10, 10, 10,-10, },
                new int[8] { -10,  5,  0,  0,  0,  0,  5,-10, },
                new int[8] { -20,-10,-10,-10,-10,-10,-10,-20, },
             }
        },

        {
            PieceType.Rook,
            new int[9][]
            {
                // Intrinsic value
                new int[1] { 510 },

                // Position modifiers
                new int[8] {  0,  0,  0,  0,  0,  0,  0,  0, },
                new int[8] {  5, 10, 10, 10, 10, 10, 10,  5, },
                new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
                new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
                new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
                new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
                new int[8] { -5,  0,  0,  0,  0,  0,  0, -5, },
                new int[8] {  0,  0,  0,  5,  5,  0,  0,  0, },
             }
        },

        {
            PieceType.Queen,
            new int[9][]
            {
                // Intrinsic value
                new int[1] { 880 },

                // Position modifiers
                new int[8] { -20,-10,-10, -5, -5,-10,-10,-20, },
                new int[8] { -10,  0,  0,  0,  0,  0,  0,-10, },
                new int[8] { -10,  0,  5,  5,  5,  5,  0,-10, },
                new int[8] {  -5,  0,  5,  5,  5,  5,  0, -5, },
                new int[8] {   0,  0,  5,  5,  5,  5,  0, -5, },
                new int[8] { -10,  5,  5,  5,  5,  5,  0,-10, },
                new int[8] { -10,  0,  5,  0,  0,  0,  0,-10, },
                new int[8] { -20,-10,-10, -5, -5,-10,-10,-20, },
             }
        },
    };

    private struct ScoredMove
    {
        public ScoredMove(Move move, bool isWhite, double potentialScore = double.MinValue) =>
          (Move, IsWhite, PotentialScore, Score) = (move, isWhite, potentialScore, double.MinValue);

        public Move Move { get; }
        public bool IsWhite { get; }
        public double PotentialScore { get; } // Based only on the piece positions and the captures

        public double Score { get; set; } // To be set later by the function CalculateBestMove
    }

    public Move Think(Board board, Timer timer) => CalculateBestMove(board, timer).Move;

    private static ScoredMove CalculateBestMove(Board board, Timer timer, double previousBestMoveScore = double.MinValue, int depth = 1)
    {
        var scoredMoves = GetScoredMoves(board).ToArray(); // sorted from the highest potential score to the lowest to cut off branchs early
        var bestMove = scoredMoves.Length > 0 ? scoredMoves[0] : new ScoredMove(Move.NullMove, board.IsWhiteToMove);

        for (var moveIndex = 0; moveIndex < scoredMoves.Length; moveIndex++)
        {
            board.MakeMove(scoredMoves[moveIndex].Move);

            if (board.IsInCheckmate())
            {
                scoredMoves[moveIndex].Score = double.MaxValue;
            }
            else
            {
                // Change maximum depth depending on how many pieces and time there are left
                var maxDepth = (BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard) > 12)
                    ? ((timer.MillisecondsRemaining > 20000) ? 5 : 4)
                    : ((timer.MillisecondsRemaining > 20000) ? 6 : (timer.MillisecondsRemaining > 5000 ? 5 : 4));

                // Calculate the best move for the opponent after this move, then take the inverse as our score.
                // If we cannot go deeper in the search tree, simply get an heuristic to roughly approximate the score.
                scoredMoves[moveIndex].Score = (depth < maxDepth)
                    ? -CalculateBestMove(board, timer, bestMove.Score, depth + 1).Score
                    : CalculateHeuristicScore(board, scoredMoves[moveIndex].IsWhite);
            }

            board.UndoMove(scoredMoves[moveIndex].Move);

            bestMove = scoredMoves[moveIndex].Score > bestMove.Score ? scoredMoves[moveIndex] : bestMove;

            if (bestMove.Score == double.MaxValue || previousBestMoveScore >= -bestMove.Score)
            {
                // No need to continue with this loop because one of these is true:
                // * The ideal move (a checkmate) has been found
                // * The parent node has already found a move that will make us perform worse in this turn than we are performing now
                break;
            }
        }

        return bestMove;
    }

    private static double CalculateHeuristicScore(Board board, bool isWhite)
    {
        // To incentive aggresivity, start by skweing the score slightly favorably if the user move caused a check
        var heuristicScore = board.IsInCheck() ? 1d : 0d;

        // Calculate how well we are doing in terms of piece values of the player and the opponent
        for (var index = 0; index < 64; index++)
        {
            var square = new Square(index);
            var pieceInSquare = board.GetPiece(square);
            if (!pieceInSquare.IsNull)
            {
                heuristicScore += pieceInSquare.IsWhite == isWhite
                    ? CalculatePieceScore(pieceInSquare.PieceType, square, pieceInSquare.IsWhite) / 16d
                    : -CalculatePieceScore(pieceInSquare.PieceType, square, pieceInSquare.IsWhite) / 16d;
            }
        }

        // We are here because we cannot look any further down the search tree, so apart from looking at how good
        // the board looks for us right now, we also need to consider how good our rival has it in terms of moves,
        // that way we try to avoid the horizon effect.
        return (90d * heuristicScore) - (10d * GetMovesScore(board));
    }

    private static double CalculatePieceScore(PieceType piece, Square position, bool isWhite)
    {
        // Calculate the intrinsic value of the piece + its position value
        var verticalIndex = isWhite ? (7 - position.Rank) : position.Rank;
        var horizontalIndex = isWhite ? position.File : (7 - position.File);
        return (piece == PieceType.None || piece == PieceType.King)
            ? 0d
            : 100d * ((_piecePositionValues[piece][verticalIndex + 1][horizontalIndex] + _piecePositionValues[piece][0][0]) / 885d);
    }

    private static double GetMovesScore(Board board) => board
        .GetLegalMoves()
        .Select(m => CalculateMovePotentialScore(m, board.IsWhiteToMove))
        .DefaultIfEmpty(0d)
        .Sum() / 218d; // 218 is the theoretical maximum number of available moves for a turn in chess

    private static IOrderedEnumerable<ScoredMove> GetScoredMoves(Board board) => board
        .GetLegalMoves()
        .Select(m => new ScoredMove(m, board.IsWhiteToMove, CalculateMovePotentialScore(m, board.IsWhiteToMove)))
        .OrderByDescending(m => m.PotentialScore);

    private static double CalculateMovePotentialScore(Move move, bool isWhite) => (
            CalculatePieceScore((move.IsPromotion ? move.PromotionPieceType : move.MovePieceType), move.TargetSquare, isWhite)
            - CalculatePieceScore(move.MovePieceType, move.StartSquare, isWhite)
            + CalculatePieceScore(move.CapturePieceType, move.TargetSquare, !isWhite)
        ) / 2d;
}