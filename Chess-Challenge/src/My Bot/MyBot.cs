using ChessChallenge.API;
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

    private class ScoredMove
    {
        public ScoredMove(Move move, bool isWhite, double potentialScore = double.MinValue) =>
          (Move, IsWhite, PotentialScore, Score) = (move, isWhite, potentialScore, double.MinValue);

        public Move Move { get; }
        public bool IsWhite { get; }
        public double PotentialScore { get; }

        public double Score { get; set; } // To be set later by the function CalculateBestMove
    }

    public Move Think(Board board, Timer timer) => CalculateBestMove(board, timer).Move;

    private static ScoredMove CalculateBestMove(Board board, Timer timer, double previousBestMoveScore = double.MinValue, int depth = 1)
    {
        var legalScoredMoves = GetLegalScoredMoves(board);
        var bestMove = legalScoredMoves.Length > 0 ? legalScoredMoves[0] : new ScoredMove(Move.NullMove, board.IsWhiteToMove);

        foreach (var candidateMove in legalScoredMoves)
        {
            board.MakeMove(candidateMove.Move);

            if (board.IsInCheckmate())
            {
                candidateMove.Score = double.MaxValue;
            }
            else
            {
                // Calculate the best move for the opponent after this move, then take the inverse as our score.
                // If we cannot go deeper, simply get an heuristic to roughly approximate the score.
                var maxDepth = timer.MillisecondsRemaining > 25000
                    ? 6
                    : (timer.MillisecondsRemaining > 15000 ? 5 : 4);
                candidateMove.Score = (depth < maxDepth)
                    ? -CalculateBestMove(board, timer, bestMove.Score, depth + 1).Score
                    : CalculateHeuristicScore(board, candidateMove.IsWhite);
            }

            board.UndoMove(candidateMove.Move);

            bestMove = candidateMove.Score > bestMove.Score ? candidateMove : bestMove;

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
        var boardScore = 0d;

        // Simply calculate how well we are doing in terms of piece values of the player and the opponent
        for (var index = 0; index < 64; index++)
        {
            var square = new Square(index);
            var piece = board.GetPiece(square);
            if (!piece.IsNull)
            {
                boardScore += piece.IsWhite == isWhite
                    ? CalculatePieceScore(piece.PieceType, square, piece.IsWhite)
                    : -CalculatePieceScore(piece.PieceType, square, piece.IsWhite);
            }
        }

        return boardScore / 16d;
    }

    private static double CalculatePieceScore(PieceType piece, Square position, bool isWhite)
    {
        // Calculate the intrinsic value of the piece + its value modifier due to its current position
        var verticalIndex = isWhite ? (7 - position.Rank) : position.Rank;
        var horizontalIndex = isWhite ? position.File : (7 - position.File);
        return (piece == PieceType.None || piece == PieceType.King)
            ? 0d
            : 100d * ((_piecePositionValues[piece][verticalIndex + 1][horizontalIndex] + _piecePositionValues[piece][0][0]) / 885d);
    }

    private static ScoredMove[] GetLegalScoredMoves(Board board) => board
        .GetLegalMoves()
        .Select(m => new ScoredMove(m, board.IsWhiteToMove, CalculateMovePotentialScore(m, board.IsWhiteToMove)))
        .OrderByDescending(m => m.PotentialScore)
        .ToArray();

    private static double CalculateMovePotentialScore(Move move, bool isWhite) => (
            CalculatePieceScore((move.IsPromotion ? move.PromotionPieceType : move.MovePieceType), move.TargetSquare, isWhite)
            - CalculatePieceScore(move.MovePieceType, move.StartSquare, isWhite) + 100d
            + CalculatePieceScore(move.CapturePieceType, move.TargetSquare, !isWhite)
        ) / 3d;
}