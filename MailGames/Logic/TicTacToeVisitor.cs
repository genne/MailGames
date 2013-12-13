using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using MailGames.Context;
using TicTacToe;

namespace MailGames.Logic
{
    class TicTacToeVisitor : IGameBoardVisitor
    {
        private readonly TicTacToeBoard _ticTacToeBoard;
        private readonly TicTacToeVariant _variant;

        public TicTacToeVisitor(TicTacToeBoard ticTacToeBoard, TicTacToeVariant variant)
        {
            _ticTacToeBoard = ticTacToeBoard;
            _variant = variant;
        }

        public GamePlayer GetCurrentPlayer()
        {
            return _ticTacToeBoard.Moves.Count() % 2 == 0 ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer;
        }

        public IEnumerable<DateTime> GetActivityDates()
        {
            return _ticTacToeBoard.Moves.Select(m => m.DateTime);
        }

        public IGameBoard CreateBoard(MailGamesContext db)
        {
            var board = db.TicTacToeBoards.Create();
            board.Variant = _variant;
            db.TicTacToeBoards.Add(board);
            return board;
        }

        public IGameBoard FindBoard(MailGamesContext db, Guid id)
        {
            return db.TicTacToeBoards.Find(id);
        }

        public string GetName()
        {
            switch (_variant)
            {
                case TicTacToeVariant.Original:
                    return "Tic Tac Toe";
                case TicTacToeVariant.Lufferschess:
                    return "Lufferschess";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string GetController()
        {
            return "TicTacToe";
        }

        public WinnerState? GetWinnerState()
        {
            return TicTacToeLogic.GetWinner(TicTacToeConversion.GetState(_ticTacToeBoard));
        }

        public string GetWikipediaId()
        {
            switch (_variant)
            {
                case TicTacToeVariant.Original:
                    return "Tictactoe";
                    break;
                case TicTacToeVariant.Lufferschess:
                    return "Renju";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Remove(MailGamesContext db)
        {
            db.TicTacToeBoards.Remove(_ticTacToeBoard);
        }
    }
}