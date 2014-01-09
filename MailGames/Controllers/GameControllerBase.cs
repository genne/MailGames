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
            if (opponent == null || opponent.Mail == null) return;
            if (!opponent.WaitingGames.Any())
            {
                var message = new MailMessage(new MailAddress("noreply@mailgames.azurewebsites.net", "Mail Games"), new MailAddress(opponent.Mail, opponent.FullName));
                message.Subject = "Pending games";
                var loginUrl = Url.Action("LoginUsingGuid", "Account", new { opponent.Guid }, Request.Url.Scheme);
                message.Body = string.Format("Hello {0},\r\n\r\nYou've got pending games waiting for you. Click this link to login:\r\n\r\n{1}", PlayerManager.GetPlayerName(opponent), loginUrl);
                new SmtpClient().Send(message);
            }

            opponent.WaitingGames.Add(new WaitingGame
            {
                GameType = GameLogic.GetGameType(gameBoard),
                GameId = gameBoard.Id,
                DateTime = DateTime.Now
            });
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