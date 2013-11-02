using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
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
    [InitializeSimpleMembership]
    public class ChessController : Controller
    {
        //
        // GET: /Chess/

        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult StartGame(string opponentsmail)
        {
            var db = new MailGamesContext();
            var board = db.ChessBoards.Create();
            board.Id = Guid.NewGuid();
            string yourmail = PlayerManager.GetCurrent(db).Mail;
            board.WhitePlayer = FindOrCreatePlayer(yourmail, db);
            board.BlackPlayer = FindOrCreatePlayer(opponentsmail, db);
            db.ChessBoards.Add(board);
            db.SaveChanges();
            return RedirectToAction("Game", new {id = board.Id, player = board.WhitePlayer.Guid });
        }

        private ChessBoardViewModel GetBoardViewModel(ChessBoard board)
        {
            var state = ChessConversion.GetCurrentState(board);
            var model = new ChessBoardViewModel();
            model.Id = board.Id;
            model.CurrentColor = state.CurrentColor;
            model.PlayerColor = board.WhitePlayer.Id == WebSecurity.CurrentUserId ? PieceColor.White : PieceColor.Black;
            model.Cells = new Piece[8,8];
            foreach(var cellState in state.GetCells())
            {
                var pos = Position.FromInt(cellState.Key);
                model.Cells[pos.Col, pos.Row] = cellState.Value;
            }
            return model;
        }

        private static Player FindOrCreatePlayer(string yourmail, MailGamesContext db)
        {
            var player = db.Players.FirstOrDefault(p => p.Mail == yourmail);
            if (player == null)
            {
                player = db.Players.Create();
                player.Mail = yourmail;
                player.Guid = Guid.NewGuid();
                db.Players.Add(player);
            }
            return player;
        }

        [Authorize]
        public JsonResult GetAvailableCells(Guid board, int selected)
        {
            var db = new MailGamesContext();
            ChessBoard chessBoard = db.ChessBoards.Find(board);
            var availableCells = ChessLogic.GetAvailableTargets(ChessConversion.GetCurrentState(chessBoard), selected);
            return Json(availableCells);
        }

        public ActionResult Game(Guid id, Guid? player)
        {
            var db = new MailGamesContext();
            if (player.HasValue)
            {
                var userName = db.Players.Single(p => p.Guid == player.Value).UserName;
                FormsAuthentication.SetAuthCookie(userName, false);
                return RedirectToAction("Game", new {id}); // Reload this action, but now logged in
            }
            if (!WebSecurity.IsAuthenticated) throw new ValidationException("Not logged in");
            ChessBoard board = db.ChessBoards.Find(id);
            return View(new ChessGameViewModel
            {
                Board = GetBoardViewModel(board),
                YourMail = db.Players.Single(p => p.Id == WebSecurity.CurrentUserId).Mail,
                OpponentMail = board.WhitePlayer.Id == WebSecurity.CurrentUserId ? board.BlackPlayer.Mail : board.WhitePlayer.Mail
            });
        }

        [Authorize]
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
                PawnConversion = convertPawnTo.HasValue ? new[]{ new PawnConversion{ ConvertTo = convertPawnTo.Value }} : null
            });

            boardObj.RunningState = ChessLogic.GetRunningState(currentState);
            if (boardObj.RunningState == ChessRunningState.CheckMate)
            {
                boardObj.Winner = currentColor;
            }

            db.SaveChanges();

            if (boardObj.RunningState == ChessRunningState.CheckMate)
                SendMail(board, db, boardObj, "It's your turn! Click here: ", "Your turn!");
            else
            {
                SendMail(board, db, boardObj, "You lost the game :(\r\n\r\n", "Checkmate");
            }

            return RedirectToAction("Game", new {id = board});
        }

        private void SendMail(Guid board, MailGamesContext db, ChessBoard boardObj, string message, string topic)
        {
            var mail = PlayerManager.GetCurrent(db).Mail;
            var otherPlayer = boardObj.WhitePlayer.Id == WebSecurity.CurrentUserId ? boardObj.BlackPlayer : boardObj.WhitePlayer;
            string url = Url.Action("Game", null, new {id = board, player = otherPlayer.Guid}, Request.Url.Scheme);
            string body = message + url;
            new SmtpClient().Send(mail, otherPlayer.Mail, "Mail Chess - " + topic, body);
        }

        private void EnsurePlayersTurn(ChessBoard boardObj)
        {
            if (boardObj.Winner.HasValue) throw new ValidationException("Game over");
            var currentPlayer = boardObj.ChessMoves.Count()%2 == 0 ? PieceColor.White : PieceColor.Black;
            var loggedInPlayer = boardObj.WhitePlayer.Id == WebSecurity.CurrentUserId ? PieceColor.White : PieceColor.Black;
            if (currentPlayer != loggedInPlayer) throw new ValidationException("Not your turn");
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

            SendMail(id, db, board, "You won the game, congratulations!\r\n\r", "Opponent resigned");

            return RedirectToAction("Game", new {id});
        }
    }

    public class PlayerManager
    {
        public static Player GetCurrent(MailGamesContext db)
        {
            return db.Players.Single(p => p.Id == WebSecurity.CurrentUserId);
        }
    }
}
