using Raylib_cs;
using System.Numerics;
using System;

namespace ChessChallenge.Application
{
    public static class MatchStatsUI
    {
        public static void DrawMatchStats(ChallengeController controller)
        {
            if (controller.PlayerWhite.IsBot || controller.PlayerWhite.IsHuman)
            {
                int nameFontSize = UIHelper.ScaleInt(40);
                int regularFontSize = UIHelper.ScaleInt(35);
                int headerFontSize = UIHelper.ScaleInt(45);
                int zobristKeyFontSize = UIHelper.ScaleInt(30);
                Vector2 buttonSize = UIHelper.Scale(new Vector2(260, 55));
                Color col = new(180, 180, 180, 255);
                Vector2 startPos = UIHelper.Scale(new Vector2(1500, 250));
                float spacingY = UIHelper.Scale(35);

                DrawNextText($"Game {controller.CurrGameNumber} of {controller.TotalGameCount}", headerFontSize, Color.WHITE);
                startPos.Y += spacingY * 2;

                DrawStats(controller.BotStatsA);
                startPos.Y += spacingY * 2;
                DrawStats(controller.BotStatsB);

                startPos.Y += spacingY * 2;
                DrawNextText($"{controller.ZobristKey}", zobristKeyFontSize, col);
                Vector2 zobristButtonPosition = startPos;
                zobristButtonPosition.X += buttonSize.X / 2;
                zobristButtonPosition.Y += buttonSize.Y / 2;
                if (DrawCopyZobristKeyButton(zobristButtonPosition, buttonSize))
                {
                    UIHelper.CopyToClipboard($"{controller.ZobristKey}");
                }

                void DrawStats(ChallengeController.BotMatchStats stats)
                {
                    DrawNextText(stats.BotName + ":", nameFontSize, Color.WHITE);
                    DrawNextText($"Score: +{stats.NumWins} ={stats.NumDraws} -{stats.NumLosses}", regularFontSize, col);
                    DrawNextText($"Num Timeouts: {stats.NumTimeouts}", regularFontSize, col);
                    DrawNextText($"Num Illegal Moves: {stats.NumIllegalMoves}", regularFontSize, col);
                }

                void DrawNextText(string text, int fontSize, Color col)
                {
                    UIHelper.DrawText(text, startPos, fontSize, 1, col);
                    startPos.Y += spacingY;
                }

                bool DrawCopyZobristKeyButton(Vector2 pos, Vector2 size)
                {
                    return UIHelper.Button("Copy Zobrist Key", pos, size);
                }
            }
        }
    }
}