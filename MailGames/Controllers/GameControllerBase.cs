using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using Chess;
using GameBase;
using MailGames.Context;
using MailGames.Logic;
using MailGames.Models;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    public class GameControllerBase : Controller
    {
        protected void SendOpponentMail(IGameBoard gameBoard)
        {
            var opponent = PlayerManager.GetOpponent(gameBoard);
            SendOpponentMail(opponent);
        }

        protected void SendOpponentMail(Player opponent)
        {
            if (opponent == null || opponent.Mail == null) return;
            if (opponent.PendingGamesMailSent == null ||
                opponent.PendingGamesMailSent < DateTime.Now.Subtract(TimeSpan.FromDays(1)))
            {
                var @from = new MailAddress("noreply@mailgames.azurewebsites.net", "Mail Games");
                var to = new MailAddress(opponent.Mail, opponent.FullName);
                var loginUrl = Url.Action("LoginUsingGuid", "Account", new {opponent.Guid}, Request.Url.Scheme);
                var message = new MailMessage(@from, to)
                {
                    Subject = "Pending games",
                    Body =
                        string.Format(
                            "Hello {0},\r\n\r\nYou've got pending games waiting for you. Click this link to login:\r\n\r\n{1}",
                            PlayerManager.GetPlayerName(opponent), loginUrl)
                };
                new SmtpClient().Send(message);
                opponent.PendingGamesMailSent = DateTime.Now;
            }
        }

        protected void SendOpponentMail(MailGamesContext db, IGameBoard board)
        {
            GameLogic.UpdateWinnerState(board);
            if (GameLogic.HasAIPlayer(board))
            {
                if (!board.WinnerState.HasValue)
                {
                    GameLogic.MoveAI(board);
                    GameLogic.UpdateWinnerState(board);
                }
            }
            else
            {
                var gameState = GameLogic.GetGameState(board);
                switch (gameState)
                {
                    case GameState.PlayerWon:
                    case GameState.OpponentWon:
                    case GameState.OpponentsTurn:
                    case GameState.Tie:
                        SendOpponentMail(board);
                        break;
                    case GameState.YourTurn:
                        break;
                }
            }
        }
    }
}