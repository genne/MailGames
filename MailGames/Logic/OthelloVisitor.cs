using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using MailGames.Context;
using MailGames.Controllers;
using Othello;

namespace MailGames.Logic
{
    internal class OthelloVisitor : IGameBoardVisitor
    {
        private readonly OthelloBoard _othelloBoard;

        public OthelloVisitor(OthelloBoard othelloBoard)
        {
            _othelloBoard = othelloBoard;
        }

        public GamePlayer GetCurrentPlayer()
        {
            return OthelloConversion.GetCurrentState(_othelloBoard).CurrentPlayer;
        }

        public IEnumerable<DateTime> GetActivityDates()
        {
            return _othelloBoard.Moves.Select(m => m.DateTime);
        }

        public IGameBoard CreateBoard(MailGamesContext db)
        {
            var board = db.OthelloBoards.Create();
            db.OthelloBoards.Add(board);
            return board;
        }

        public IGameBoard FindBoard(MailGamesContext db, Guid id)
        {
            return db.OthelloBoards.Find(id);
        }

        public string GetName()
        {
            return "Reversi";
        }

        public string GetController()
        {
            return "Othello";
        }

        public WinnerState? GetWinnerState()
        {
            return OthelloLogic.GetWinner(OthelloConversion.GetCurrentState(_othelloBoard));
        }

        public string GetWikipediaId()
        {
            return "Reversi";
        }
    }
}