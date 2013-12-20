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

        private ChessBoardViewModel GetBoardViewModel(ChessBoard board)
        {
            var state = ChessConversion.GetCurrentState(board);
            var model = new ChessBoardViewModel();
            model.Id = board.Id;
            model.CurrentColor = state.CurrentPlayer;
            model.PlayerColor = board.FirstPlayer.Id == WebSecurity.CurrentUserId ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer;
            bool check = ChessLogic.IsCheck(state, state.CurrentPlayer);
            model.AttackedKing = check
                                     ? state.CurrentPlayer
                                     : (GamePlayer?) null;
            model.OpponentMoves = board.ChessMoves.Reverse().Take(1).SelectMany(m => new[] {m.From, m.To}).ToArray();
            model.Cells = new Piece[8,8];
            foreach(var cellState in state.GetCells())
            {
                var pos = Position.FromInt(cellState.Key);
                model.Cells[pos.X, pos.Y] = cellState.Value;
            }
            return model;
        }

        public JsonResult GetAvailableCells(Guid board, int selected)
        {
            var db = new MailGamesContext();
            var chessBoard = ChessQueries.Find(db, board);
            var availableCells = ChessLogic.GetAvailableTargets(ChessConversion.GetCurrentState(chessBoard), selected);
            return Json(availableCells);
        }

        public ActionResult Game(Guid id)
        {
            var db = new MailGamesContext();
            var board = ChessQueries.Find(db, id);
            var state = ChessConversion.GetCurrentState(board);
            return View(new ChessGameViewModel(board)
            {
                IsCheck = board.Check,
                CapturedPieces = state.CapturedPieces,
                Moves = state.Moves,
                Board = GetBoardViewModel(board),
                Progress = ChessLogic.GetProgress(state, GameLogic.GetLoggedInPlayer(board)),
            });
        }

        public ActionResult Move(Guid board, int from, int to, PieceType? convertPawnTo)
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

            var opponentIsAI = boardObj.FirstPlayer == null || boardObj.SecondPlayer == null;
            if (opponentIsAI && !boardObj.WinnerState.HasValue)
            {
                var aiMove = ChessAI.GetMove(currentState);
                boardObj.ChessMoves.Add(new ChessMove
                {
                    From = aiMove.From,
                    To = aiMove.To,
                    PawnConversion = aiMove.ConvertPawnTo.HasValue ? new[] { new PawnConversion { ConvertTo = aiMove.ConvertPawnTo.Value } } : new PawnConversion[0],
                    DateTime = DateTime.Now
                });
            }

            boardObj.Check = ChessLogic.IsCheck(currentState, currentState.CurrentPlayer);
            db.SaveChanges();

            if (!opponentIsAI)
            {
                SendOpponentMail(db, boardObj);
            }

            return RedirectToAction("Game", new {id = board});
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
