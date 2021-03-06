﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            board.Moves = new Collection<OthelloMove>();
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

        public void Remove(MailGamesContext db)
        {
            db.OthelloBoards.Remove(_othelloBoard);
        }

        public void MoveAI()
        {
            var move = new OthelloAI(2).GetRandomBestMove(OthelloConversion.GetCurrentState(_othelloBoard));
            _othelloBoard.Moves.Add(new OthelloMove
            {
                DateTime =  DateTime.Now,
                Position = move.ToInt()
            });
        }
    }
}