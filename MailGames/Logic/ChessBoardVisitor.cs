using System;
using System.Collections.Generic;
using System.Linq;
using Chess;
using GameBase;
using MailGames.Context;

namespace MailGames.Logic
{
    class ChessBoardVisitor : IGameBoardVisitor
    {
        private readonly ChessBoard _chessBoard;

        public ChessBoardVisitor(ChessBoard chessBoard)
        {
            _chessBoard = chessBoard;
        }

        public GamePlayer GetCurrentPlayer()
        {
            return _chessBoard.ChessMoves.Count() % 2 == 0 ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer;
        }

        public IEnumerable<DateTime> GetActivityDates()
        {
            return _chessBoard.ChessMoves.Select(m => m.DateTime);
        }

        public IGameBoard CreateBoard(MailGamesContext db)
        {
            var board = db.ChessBoards.Create();
            db.ChessBoards.Add(board);
            return board;
        }

        public IGameBoard FindBoard(MailGamesContext db, Guid id)
        {
            return db.ChessBoards.Find(id);
        }

        public string GetName()
        {
            return "Chess";
        }

        public string GetController()
        {
            return "Chess";
        }

        public WinnerState? GetWinnerState()
        {
            bool isCheck;
            return ChessLogic.GetWinnerState(ChessConversion.GetCurrentState(_chessBoard), out isCheck);
        }

        public string GetWikipediaId()
        {
            return "Chess";
        }

        public void Remove(MailGamesContext db)
        {
            db.ChessBoards.Remove(_chessBoard);
        }
    }
}