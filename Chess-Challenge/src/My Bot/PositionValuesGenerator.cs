using ChessChallenge.API;
using System;
using System.Collections.Generic;
using System.Linq;

internal static class PositionValuesGenerator
{
    private static readonly Dictionary<PieceType, int[]> _pieceValues = new()
    {
        {
            PieceType.Pawn,
            new int[]
            {
                // Position values (board representation with the player at the bottom)
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
                // Position values (board representation with the player at the bottom)
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
                // Position values (board representation with the player at the bottom)
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
                // Position values (board representation with the player at the bottom)
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
                // Position values (board representation with the player at the bottom)
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

        {
            PieceType.King,
            new int[]
            {
                // Position values (board representation with the player at the bottom)
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -30,-40,-40,-50,-50,-40,-40,-30,
                -20,-30,-30,-40,-40,-30,-30,-20,
                -10,-20,-20,-20,-20,-20,-20,-10,
                 20, 20,  0,  0,  0,  0, 20, 20,
                 20, 30, 10,  0,  0, 10, 30, 20,

                // Position values in the endgame (board representation with the player at the bottom)
                -50,-40,-30,-20,-20,-30,-40,-50,
                -30,-20,-10,  0,  0,-10,-20,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 30, 40, 40, 30,-10,-30,
                -30,-10, 20, 30, 30, 20,-10,-30,
                -30,-30,  0,  0,  0,  0,-30,-30,
                -50,-30,-30,-30,-30,-30,-30,-50,
             }
        },
    };

    // Crappy helper to convert each one of the rows shown above into an unsigned long to help reduce tokens
    internal static void PrintPositionValues()
    {
        foreach (var (pieceType, values) in _pieceValues)
        {
            Console.WriteLine(pieceType + ":");

            var valuesString = "";

            var valuesStringLocal = "    ";
            var count = 0;
            foreach (var value in values)
            {
                var convertedToHex = value.ToString("x2");
                valuesStringLocal += string.Join("", convertedToHex.ToCharArray().Skip(convertedToHex.Length - 2));

                if (++count == 8)
                {
                    valuesString += ulong.Parse(valuesStringLocal, System.Globalization.NumberStyles.HexNumber) + ",\n";
                    valuesStringLocal = "    ";
                    count = 0;
                }
            }

            Console.WriteLine(valuesString);
        }
    }
}