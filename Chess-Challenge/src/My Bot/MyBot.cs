using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

public class MyBot : IChessBot
{
    // TODO Simplify these further to reduce tokens somehow?
    private readonly Dictionary<PieceType, ulong[]> _pieceValues = new()
    {
        {
            PieceType.Pawn,
            new ulong[]
            {
                // Intrinsic value
                100,

                // Position values (board representation with the player at the bottom)
                //  0,  0,  0,  0,  0,  0,  0,  0,
                // 50, 50, 50, 50, 50, 50, 50, 50,
                // 10, 10, 20, 30, 30, 20, 10, 10,
                //  5,  5, 10, 25, 25, 10,  5,  5,
                //  0,  0,  0, 20, 20,  0,  0,  0,
                //  5, -5,-10,  0,  0,-10, -5,  5,
                //  5, 10, 10,-20,-20, 10, 10,  5,
                //  0,  0,  0,  0,  0,  0,  0,  0,
                0,
                3617008641903833650,
                723412809732590090,
                361706447983740165,
                86234890240,
                431208669220633349,
                363114732645386757,
                0,
             }
        },

        {
            PieceType.Knight,
            new ulong[]
            {
                // Intrinsic value
                320,

                // Position values (board representation with the player at the bottom)
                // -50,-40,-30,-30,-30,-30,-40,-50,
                // -40,-20,  0,  0,  0,  0,-20,-40,
                // -30,  0, 10, 15, 15, 10,  0,-30,
                // -30,  5, 15, 20, 20, 15,  5,-30,
                // -30,  0, 15, 20, 20, 15,  0,-30,
                // -30,  5, 10, 15, 15, 10,  5,-30,
                // -40,-20,  0,  5,  5,  0,-20,-40,
                // -50,-40,-30,-30,-30,-30,-40,-50,
                14904912430879660238,
                15630868406696209624,
                16285027312364814562,
                16286440206365558242,
                16285032831482003682,
                16286434687248369122,
                15630868428254932184,
                14904912430879660238,
             }
        },

        {
            PieceType.Bishop,
            new ulong[]
            {
                // Intrinsic value
                333,

                // Position values (board representation with the player at the bottom)
                // -20,-10,-10,-10,-10,-10,-10,-20,
                // -10,  0,  0,  0,  0,  0,  0,-10,
                // -10,  0,  5, 10, 10,  5,  0,-10,
                // -10,  5,  5, 10, 10,  5,  5,-10,
                // -10,  0, 10, 10, 10, 10,  0,-10,
                // -10, 10, 10, 10, 10, 10, 10,-10,
                // -10,  5,  0,  0,  0,  0,  5,-10,
                // -20,-10,-10,-10,-10,-10,-10,-20,
                17075106577787582188,
                17726168133330272502,
                17726173674006184182,
                17727581048889738742,
                17726179171564650742,
                17728993921331759862,
                17727575508213827062,
                17075106577787582188,
             }
        },

        {
            PieceType.Rook,
            new ulong[]
            {
                // Intrinsic value
                510,

                // Position values (board representation with the player at the bottom)
                //  0,  0,  0,  0,  0,  0,  0,  0,
                //  5, 10, 10, 10, 10, 10, 10,  5,
                // -5,  0,  0,  0,  0,  0,  0, -5,
                // -5,  0,  0,  0,  0,  0,  0, -5,
                // -5,  0,  0,  0,  0,  0,  0, -5,
                // -5,  0,  0,  0,  0,  0,  0, -5,
                // -5,  0,  0,  0,  0,  0,  0, -5,
                //  0,  0,  0,  5,  5,  0,  0,  0,
                0,
                363113758191127045,
                18086456103519912187,
                18086456103519912187,
                18086456103519912187,
                18086456103519912187,
                18086456103519912187,
                21558722560,
            }
        },

        {
            PieceType.Queen,
            new ulong[]
            {
                // Intrinsic value
                880,

                // Position values (board representation with the player at the bottom)
                // -20,-10,-10, -5, -5,-10,-10,-20,
                // -10,  0,  0,  0,  0,  0,  0,-10,
                // -10,  0,  5,  5,  5,  5,  0,-10,
                //  -5,  0,  5,  5,  5,  5,  0, -5,
                //   0,  0,  5,  5,  5,  5,  0, -5,
                // -10,  5,  5,  5,  5,  5,  0,-10,
                // -10,  0,  5,  0,  0,  0,  0,-10,
                // -20,-10,-10, -5, -5,-10,-10,-20,
                17075106599346304748,
                17726168133330272502,
                17726173652447461622,
                18086461622637101307,
                5519117189371,
                17727581027331014902,
                17726173630888411382,
                17075106599346304748,
             }
        },

        {
            PieceType.King,
            new ulong[]
            {
                // Intrinsic value
                0,

                // Position values (board representation with the player at the bottom)
                // -30,-40,-40,-50,-50,-40,-40,-30,
                // -30,-40,-40,-50,-50,-40,-40,-30,
                // -30,-40,-40,-50,-50,-40,-40,-30,
                // -30,-40,-40,-50,-50,-40,-40,-30,
                // -20,-30,-30,-40,-40,-30,-30,-20,
                // -10,-20,-20,-20,-20,-20,-20,-10,
                //  20, 20,  0,  0,  0,  0, 20, 20,
                //  20, 30, 10,  0,  0, 10, 30, 20,
                16346053230286395618,
                16346053230286395618,
                16346053230286395618,
                16346053230286395618,
                17069454958667162348,
                17792856730165374198,
                1446781380292776980,
                1449607125176819220,

                // Position values in the endgame (board representation with the player at the bottom)
                // -50,-40,-30,-20,-20,-30,-40,-50,
                // -30,-20,-10,  0,  0,-10,-20,-30,
                // -30,-10, 20, 30, 30, 20,-10,-30,
                // -30,-10, 30, 40, 40, 30,-10,-30,
                // -30,-10, 30, 40, 40, 30,-10,-30,
                // -30,-10, 20, 30, 30, 20,-10,-30,
                // -30,-30,  0,  0,  0,  0,-30,-30,
                // -50,-30,-30,-30,-30,-30,-30,-50,
                14904912473997105358,
                16351714826952043746,
                16354281216428799714,
                16354292254663177954,
                16354292254663177954,
                16354281216428799714,
                16348629597308379874,
                14907727180646769358,
             }
        },
    };

    private struct ScoredMove
    {
        public ScoredMove(Move move, int potentialScore = int.MinValue) =>
            (Move, PotentialScore, GameScore) = (move, potentialScore, int.MinValue);

        public Move Move { get; }
        public int PotentialScore { get; } // Based only on the piece position change and the potential capture
        public int GameScore { get; set; } // To be set later by the function CalculateBestMove(...)
    }

    public Move Think(Board board, Timer timer) => CalculateBestMove(board, timer).Move;

    // Simple minimax algorithm to guess the best possible move
    private ScoredMove CalculateBestMove(Board board, Timer timer, int previousBestMoveScore = int.MinValue, int depth = 1)
    {
        // Moves scored by their potential and sorted from the highest to the lowest to cut off branchs early
        var numberOfPiecesLeft = BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard);
        var scoredMoves = board
            .GetLegalMoves()
            .Select(m => new ScoredMove(m, CalculateMovePotentialScore(m, board.IsWhiteToMove, numberOfPiecesLeft <= 12)))
            .OrderByDescending(m => m.PotentialScore);

        var bestMove = scoredMoves.FirstOrDefault(new ScoredMove(Move.NullMove));

        using var scoredMovesEnumerator = scoredMoves.GetEnumerator();
        while (scoredMovesEnumerator.MoveNext())
        {
            var candidateMove = scoredMovesEnumerator.Current;
            board.MakeMove(candidateMove.Move);

            if (board.IsInCheckmate())
                candidateMove.GameScore = int.MaxValue;
            else
            {
                // Change maximum depth depending on how much time we have and how many pieces there are left
                numberOfPiecesLeft = BitboardHelper.GetNumberOfSetBits(board.AllPiecesBitboard);
                var maxDepth = numberOfPiecesLeft <= 12 || timer.MillisecondsRemaining > 20000
                    ? (numberOfPiecesLeft <= 5 ? 7 : 6)
                    : (timer.MillisecondsRemaining > 5000 || numberOfPiecesLeft <= 5 ? 5 : 4);

                // Calculate the best move for the opponent after our move, then take the inverse as our score.
                // If we cannot go deeper in the search tree, then we calculate the score using heuristics.
                candidateMove.GameScore = (depth < maxDepth)
                    ? -CalculateBestMove(board, timer, bestMove.GameScore, depth + 1).GameScore
                    : CalculateHeuristicScore(board, numberOfPiecesLeft);
            }

            board.UndoMove(candidateMove.Move);

            bestMove = candidateMove.GameScore > bestMove.GameScore ? candidateMove : bestMove;

            // No need to continue with the search in this branch if one of these is true:
            // * The ideal move (a checkmate) has already been found.
            // * The parent node has already found a move that would make us perform worse in this turn than we are performing now.
            //   Thus, they will choose that move and not the one that would lead to the hypothetical situation we are analysing here.
            if (bestMove.GameScore == int.MaxValue || previousBestMoveScore >= -bestMove.GameScore)
                break;
        }

        return bestMove;
    }

    // Calculations: See how well we are doing looking at the piece scores, both ours (positive) and the opponent's (negative)
    private int CalculateHeuristicScore(Board board, int numberOfPiecesLeft)
    {
        var heuristicScore = 0;

        for (int squareIndex = 0, numberOfPiecesFound = 0; numberOfPiecesFound < numberOfPiecesLeft; squareIndex++)
        {
            Square square = new(squareIndex);
            var pieceInSquare = board.GetPiece(square);
            if (pieceInSquare.IsNull)
                continue;

            // To know if a piece is ours, we just have to remember that we are here because we just moved (it is the opponent's turn)
            heuristicScore += (pieceInSquare.IsWhite != board.IsWhiteToMove ? 1 : -1) * CalculatePieceScore(pieceInSquare.PieceType, square, pieceInSquare.IsWhite, numberOfPiecesLeft <= 12);
            numberOfPiecesFound++;
        }

        return heuristicScore;
    }

    // Calculations: score of the piece after moving - score of the piece before moving + score of the captured piece
    private int CalculateMovePotentialScore(Move move, bool isWhite, bool isEndGame) => CalculatePieceScore(move.IsPromotion ? move.PromotionPieceType : move.MovePieceType, move.TargetSquare, isWhite, isEndGame)
        - CalculatePieceScore(move.MovePieceType, move.StartSquare, isWhite, isEndGame)
        + CalculatePieceScore(move.CapturePieceType, move.TargetSquare, !isWhite, isEndGame);

    // Calculations: the intrinsic value of the piece + its position value
    private int CalculatePieceScore(PieceType piece, Square position, bool isWhite, bool isEndGame) => piece != PieceType.None
        ? (int)_pieceValues[piece][0] + ((sbyte)(_pieceValues[piece][1 + (isWhite ? (7 - position.Rank) : position.Rank) + (piece == PieceType.King && isEndGame ? 8 : 0)] >> (8 * (isWhite ? (7 - position.File) : position.File))))
        : 0;
}