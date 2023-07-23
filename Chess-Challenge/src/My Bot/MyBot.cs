using ChessChallenge.API;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    // TODO Simplify these with bits to reduce tokens
    private static readonly Dictionary<PieceType, int[]> _pieceValues = new Dictionary<PieceType, int[]>
    {
        {
            PieceType.Pawn,
            new int[]
            {
                // Intrinsic value
                100,

                // Position modifiers (white at the bottom, black at the top)
                  0,  0,  0,  0,  0,  0,  0,  0,
                 50, 50, 50, 50, 50, 50, 50, 50,
                 10, 10, 20, 30, 30, 20, 10, 10,
                  5,  5, 10, 25, 25, 10,  5,  5,
                  0,  0,  0, 20, 20,  0,  0,  0,
                  5, -5,-10,  0,  0,-10, -5,  5,
                  5, 10, 10,-20,-20, 10, 10,  5,
                  0,  0,  0,  0,  0,  0,  0,  0,
             }
        },

        {
            PieceType.Knight,
            new int[]
            {
                // Intrinsic value
                320,

                // Position modifiers (white at the bottom, black at the top)
                -50,-40,-30,-30,-30,-30,-40,-50,
                -40,-20,  0,  0,  0,  0,-20,-40,
                -30,  0, 10, 15, 15, 10,  0,-30,
                -30,  5, 15, 20, 20, 15,  5,-30,
                -30,  0, 15, 20, 20, 15,  0,-30,
                -30,  5, 10, 15, 15, 10,  5,-30,
                -40,-20,  0,  5,  5,  0,-20,-40,
                -50,-40,-30,-30,-30,-30,-40,-50,
             }
        },

        {
            PieceType.Bishop,
            new int[]
            {
                // Intrinsic value
                333,

                // Position modifiers (white at the bottom, black at the top)
                -20,-10,-10,-10,-10,-10,-10,-20,
                -10,  0,  0,  0,  0,  0,  0,-10,
                -10,  0,  5, 10, 10,  5,  0,-10,
                -10,  5,  5, 10, 10,  5,  5,-10,
                -10,  0, 10, 10, 10, 10,  0,-10,
                -10, 10, 10, 10, 10, 10, 10,-10,
                -10,  5,  0,  0,  0,  0,  5,-10,
                -20,-10,-10,-10,-10,-10,-10,-20,
             }
        },

        {
            PieceType.Rook,
            new int[]
            {
                // Intrinsic value
                510,

                // Position modifiers (white at the bottom, black at the top)
                  0,  0,  0,  0,  0,  0,  0,  0,
                  5, 10, 10, 10, 10, 10, 10,  5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                 -5,  0,  0,  0,  0,  0,  0, -5,
                  0,  0,  0,  5,  5,  0,  0,  0,
            }
        },

        {
            PieceType.Queen,
            new int[]
            {
                // Intrinsic value
                880,

                // Position modifiers (white at the bottom, black at the top)
                -20,-10,-10, -5, -5,-10,-10,-20,
                -10,  0,  0,  0,  0,  0,  0,-10,
                -10,  0,  5,  5,  5,  5,  0,-10,
                 -5,  0,  5,  5,  5,  5,  0, -5,
                  0,  0,  5,  5,  5,  5,  0, -5,
                -10,  5,  5,  5,  5,  5,  0,-10,
                -10,  0,  5,  0,  0,  0,  0,-10,
                -20,-10,-10, -5, -5,-10,-10,-20,
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

    private ScoredMove CalculateBestMove(Board board, Timer timer, double previousBestMoveScore = double.MinValue, int depth = 1)
    {
        // Moves scored by their potential and sorted from the highest to the lowest to cut off branchs early
        var scoredMoves = board
            .GetLegalMoves()
            .Select(m => new ScoredMove(m, board.IsWhiteToMove, CalculateMovePotentialScore(m, board.IsWhiteToMove)))
            .OrderByDescending(m => m.PotentialScore);
        var bestMove = scoredMoves.FirstOrDefault(new ScoredMove(Move.NullMove, board.IsWhiteToMove));

        using var scoredMovesEnumerator = scoredMoves.GetEnumerator();
        while (scoredMovesEnumerator.MoveNext())
        {
            var candidateMove = scoredMovesEnumerator.Current;
            board.MakeMove(candidateMove.Move);

            if (board.IsInCheckmate())
                candidateMove.Score = double.MaxValue;
            else
            {
                // Change maximum depth depending on how many pieces and time there are left
                var maxDepth = (BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard) > 12)
                    ? ((timer.MillisecondsRemaining > 20000) ? 5 : 4)
                    : ((timer.MillisecondsRemaining > 15000) ? 6 : (timer.MillisecondsRemaining > 3500 ? 5 : 4));

                // Calculate the best move for the opponent after this move, then take the inverse as our score.
                // If we cannot go deeper in the search tree, simply get an heuristic to roughly approximate the score.
                candidateMove.Score = (depth < maxDepth)
                    ? -CalculateBestMove(board, timer, bestMove.Score, depth + 1).Score
                    : CalculateHeuristicScore(board);
            }

            board.UndoMove(candidateMove.Move);

            bestMove = candidateMove.Score > bestMove.Score ? candidateMove : bestMove;

            // No need to continue with this loop if one of these is true:
            // * The ideal move (a checkmate) has been found
            // * The parent node has already found a move that will make us perform worse in this turn than we are performing now
            if (bestMove.Score == double.MaxValue || previousBestMoveScore >= -bestMove.Score)
                break;
        }

        return bestMove;
    }

    private double CalculateHeuristicScore(Board board)
    {
        var heuristicScore = 0d;

        // Calculate how well we are doing in terms of piece values, both ours and the opponent's
        for (var index = 0; index < 64; index++)
        {
            var square = new Square(index);
            var pieceInSquare = board.GetPiece(square);
            if (!pieceInSquare.IsNull)
                // We are here because we just moved, so it is the opponent's turn. This means that our pieces,
                // which are the ones that will increase our score, are the ones that aren't supposed to move.
                heuristicScore += pieceInSquare.IsWhite != board.IsWhiteToMove
                    ? CalculatePieceScore(pieceInSquare.PieceType, square, pieceInSquare.IsWhite) / 16d
                    : -CalculatePieceScore(pieceInSquare.PieceType, square, pieceInSquare.IsWhite) / 16d;
        }

        return heuristicScore;
    }

    private double CalculatePieceScore(PieceType piece, Square position, bool isWhite) => (piece == PieceType.None || piece == PieceType.King)
            ? 0d
            // Calculate the intrinsic value of the piece + its position value
            : ((_pieceValues[piece][0] + _pieceValues[piece][1 + ((isWhite ? (7 - position.Rank) : position.Rank) * 8) + (isWhite ? position.File : (7 - position.File))]) / 8.85d);

    private double CalculateMovePotentialScore(Move move, bool isWhite) => (
            CalculatePieceScore((move.IsPromotion ? move.PromotionPieceType : move.MovePieceType), move.TargetSquare, isWhite)
            - CalculatePieceScore(move.MovePieceType, move.StartSquare, isWhite)
            + CalculatePieceScore(move.CapturePieceType, move.TargetSquare, !isWhite)
        ) / 2d;
}