using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            board.ChessMoves = new Collection<ChessMove>();
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

        public void MoveAI()
        {
            var aiMove = new ChessAI().GetRandomBestMove(ChessConversion.GetCurrentState(_chessBoard));
            _chessBoard.ChessMoves.Add(new ChessMove
            {
                From = aiMove.From,
                To = aiMove.To,
                PawnConversion =
                    aiMove.ConvertPawnTo.HasValue
                        ? new[] { new PawnConversion { ConvertTo = aiMove.ConvertPawnTo.Value } }
                        : new PawnConversion[0],
                DateTime = DateTime.Now
            });
        }
    }
}