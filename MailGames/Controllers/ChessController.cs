using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Chess;
using MailGames.Chess;
using MailGames.Context;
using MailGames.Filters;
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
            model.CurrentColor = state.CurrentColor;
            model.PlayerColor = board.FirstPlayer.Id == WebSecurity.CurrentUserId ? PieceColor.White : PieceColor.Black;
            model.Cells = new Piece[8,8];
            foreach(var cellState in state.GetCells())
            {
                var pos = Position.FromInt(cellState.Key);
                model.Cells[pos.Col, pos.Row] = cellState.Value;
            }
            return model;
        }

        [Authorize]
        public JsonResult GetAvailableCells(Guid board, int selected)
        {
            var db = new MailGamesContext();
            ChessBoard chessBoard = db.ChessBoards.Find(board);
            var availableCells = ChessLogic.GetAvailableTargets(ChessConversion.GetCurrentState(chessBoard), selected);
            return Json(availableCells);
        }

        public ActionResult Game(Guid id)
        {
            var db = new MailGamesContext();
            if (!WebSecurity.IsAuthenticated) throw new ValidationException("Not logged in");
            ChessBoard board = db.ChessBoards.Find(id);
            var state = ChessConversion.GetCurrentState(board);
            return View(new ChessGameViewModel
            {
                Board = GetBoardViewModel(board),
                State = ChessLogic.GetGameState(board.RunningState, GetCurrentPlayer(board), GetLoggedInPlayer(board)),
                GameType = GameType.Chess,
                IsCheck = board.RunningState == ChessRunningState.Check,
                CapturedPieces = state.CapturedPieces,
                Moves = state.Moves
            });
        }

        public ActionResult Move(Guid board, int from, int to, PieceType? convertPawnTo)
        {
            var db = new MailGamesContext();
            var boardObj = db.ChessBoards.Find(board);
            EnsurePlayersTurn(boardObj);
            var currentState = ChessConversion.GetCurrentState(boardObj);
            var currentColor = currentState.CurrentColor;
            ChessLogic.ApplyMove(currentState, from, to, convertPawnTo);
            
            boardObj.ChessMoves.Add(new ChessMove
            {
                From = from,
                To = to,
                PawnConversion = convertPawnTo.HasValue ? new[]{ new PawnConversion{ ConvertTo = convertPawnTo.Value }} : null,
                DateTime = DateTime.Now
            });

            boardObj.RunningState = ChessLogic.GetRunningState(currentState);
            if (boardObj.RunningState == ChessRunningState.CheckMate)
            {
                boardObj.Winner = currentColor;
            }

            db.SaveChanges();

            if (boardObj.RunningState == ChessRunningState.CheckMate)
                SendMail(db, boardObj, "You lost the game :(", "Checkmate :(");
            else if (boardObj.RunningState == ChessRunningState.StaleMate)
                SendMail(db, boardObj, "You lost the game :(", "Stalemate");
            else if (boardObj.RunningState == ChessRunningState.Check)
                SendMail(db, boardObj, "The opponent made a move and put you in check. Make your move!", "Check :/");
            else
                SendMail(db, boardObj, "It's your turn!", "Your turn!");

            return RedirectToAction("Game", new {id = board});
        }

        private void EnsurePlayersTurn(ChessBoard boardObj)
        {
            if (boardObj.Winner.HasValue) throw new ValidationException("Game over");
            var currentPlayer = GetCurrentPlayer(boardObj);
            var loggedInPlayer = GetLoggedInPlayer(boardObj);
            if (currentPlayer != loggedInPlayer) throw new ValidationException("Not your turn");
        }

        private static PieceColor GetLoggedInPlayer(ChessBoard boardObj)
        {
            return boardObj.FirstPlayer.Id == WebSecurity.CurrentUserId ? PieceColor.White : PieceColor.Black;
        }

        private static PieceColor GetCurrentPlayer(ChessBoard boardObj)
        {
            return boardObj.ChessMoves.Count()%2 == 0 ? PieceColor.White : PieceColor.Black;
        }

        public ActionResult Surrender(Guid id)
        {
            var db = new MailGamesContext();
            var board = db.ChessBoards.Find(id);
            EnsurePlayersTurn(board);
            var state = ChessConversion.GetCurrentState(board);
            board.RunningState = ChessRunningState.Nothing;
            board.Winner = ChessLogic.GetNextColor(state.CurrentColor);
            db.SaveChanges();

            SendMail(db, board, "You won the game, congratulations!", "Opponent resigned");

            return RedirectToAction("Game", new {id});
        }
    }
}
