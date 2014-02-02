using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Chess;
using GameBase;
using MailGames.Context;
using MailGames.Filters;
using MailGames.Logic;
using MailGames.Models;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class ChessController : GameControllerBase
    {
        //
        // GET: /Chess/

        public JsonResult GetAvailableCells(Guid board, int selected)
        {
            var db = new MailGamesContext();
            var chessBoard = ChessQueries.Find(db, board);
            var availableCells = ChessLogic.GetAvailableTargets(ChessConversion.GetCurrentState(chessBoard), selected);
            return Json(availableCells);
        }

        public ActionResult Game(Guid id)
        {
            var model = GetViewModel(id);
            return View("GameKO", model);
        }

        private static ChessGameViewModel GetViewModel(Guid id)
        {
            var db = new MailGamesContext();
            var board = ChessQueries.Find(db, id);
            var state = ChessConversion.GetCurrentState(board);
            var state1 = ChessConversion.GetCurrentState(board);
            var check = ChessLogic.IsCheck(state1, state1.CurrentPlayer);
            var model = new ChessGameViewModel(board)
            {
                IsCheck = board.Check,
                CapturedPieces = state.CapturedPieces,
                Moves = state.LastMoves,
                Progress = ChessLogic.GetProgress(state, GameLogic.GetLoggedInPlayer(board)),
                Id = board.Id,
                CurrentColor = state1.CurrentPlayer,
                PlayerColor = GameLogic.GetLoggedInPlayer(board),
                AttackedKing = check
                                   ? state1.CurrentPlayer
                                   : (GamePlayer?) null,
                OpponentMoves = board.ChessMoves.Reverse().Take(1).SelectMany(m => new[] {m.From, m.To}).ToArray(),
                Cells = new Piece[8,8],
            };
            foreach (var cellState in state1.GetCells())
            {
                var pos = Position.FromInt(cellState.Key);
                model.Cells[pos.X, pos.Y] = cellState.Value;
            }
            return model;
        }

        public JsonResult Move(Guid board, int from, int to, PieceType? convertPawnTo)
        {
            var db = new MailGamesContext();
            var boardObj = ChessQueries.Find(db, board);
            GameLogic.EnsurePlayersTurn(boardObj);
            var currentState = ChessConversion.GetCurrentState(boardObj);
            ChessLogic.ApplyMove(currentState, from, to, convertPawnTo);
            
            boardObj.ChessMoves.Add(new ChessMove
            {
                From = from,
                To = to,
                PawnConversion = convertPawnTo.HasValue ? new[]{ new PawnConversion{ ConvertTo = convertPawnTo.Value }} : new PawnConversion[0],
                DateTime = DateTime.Now
            });

            GameLogic.UpdateWinnerState(boardObj);

            SendOpponentMail(db, boardObj);

            boardObj.Check = ChessLogic.IsCheck(currentState, currentState.CurrentPlayer);

            db.SaveChanges();

            return Json(GetViewModel(board));
        }
    }

    public enum Activity
    {
        Active,
        Passive,
        PassiveLostGame,
        GameNeverStarted
    }
}
