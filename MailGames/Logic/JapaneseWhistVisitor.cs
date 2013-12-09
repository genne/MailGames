using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using JapaneseWhist;
using MailGames.Context;

namespace MailGames.Logic
{
    internal class JapaneseWhistVisitor : IGameBoardVisitor
    {
        private readonly JapaneseWhistBoard _japaneseWhistBoard;

        public JapaneseWhistVisitor(JapaneseWhistBoard japaneseWhistBoard)
        {
            _japaneseWhistBoard = japaneseWhistBoard;
        }

        public GamePlayer GetCurrentPlayer()
        {
            return GetState().CurrentPlayer;
        }

        private JapaneseWhistState GetState()
        {
            return JapaneseWhistConversion.GetState(_japaneseWhistBoard);
        }

        public IEnumerable<DateTime> GetActivityDates()
        {
            return _japaneseWhistBoard.Moves.Select(m => m.DateTime);
        }

        public IGameBoard CreateBoard(MailGamesContext db)
        {
            var board = db.JapaneseWhistBoards.Create();
            db.JapaneseWhistBoards.Add(board);
            return board;
        }

        public IGameBoard FindBoard(MailGamesContext db, Guid id)
        {
            return db.JapaneseWhistBoards.Find(id);
        }

        public string GetName()
        {
            return "Japanese Whist";
        }

        public string GetController()
        {
            return "JapaneseWhist";
        }

        public WinnerState? GetWinnerState()
        {
            var winner = JapaneseWhistLogic.GetWinner(GetState());
            if (!winner.HasValue) return null;
            switch (winner.Value)
            {
                case GamePlayer.FirstPlayer:
                    return WinnerState.FirstPlayer;
                case GamePlayer.SecondPlayer:
                    return WinnerState.SecondPlayer;
            }
            throw new IndexOutOfRangeException("Invalid winner value");
        }

        public string GetWikipediaId()
        {
            return "whist";
        }
    }
}