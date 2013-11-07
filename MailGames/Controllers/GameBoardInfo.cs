using System;
using MailGames.Context;
using MailGames.Models;

namespace MailGames.Controllers
{
    public class GameBoardInfo
    {
        public static string GetName(GameType type)
        {
            switch (type)
            {
                case GameType.Chess:
                    return "Chess";
                    break;
                case GameType.TicTacToe:
                    return "Tic Tac Toe";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type");
            }
        }

        public static GameType GetGameType(IGameBoard boardObj)
        {
            var type = boardObj.GetType();
            if (IsType<ChessBoard>(type)) return GameType.Chess;
            if (IsType<TicTacToeBoard>(type)) return GameType.TicTacToe;
            throw new ArgumentException("Unknown type: " + type);
        }

        private static bool IsType<T>(Type type)
        {
            return type == typeof (T) || type.BaseType == typeof (T);
        }
    }
}