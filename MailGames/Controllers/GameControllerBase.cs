using System.Net.Mail;
using System.Web.Mvc;
using GameBase;
using MailGames.Context;
using MailGames.Logic;
using MailGames.Models;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    public class GameControllerBase : Controller
    {
        protected void SendOpponentMail(MailGamesContext db, IGameBoard gameBoard, string message, string topic)
        {
            var mail = PlayerManager.GetCurrent(db).Mail;
            var otherPlayer = gameBoard.FirstPlayer.Id == WebSecurity.CurrentUserId ? gameBoard.SecondPlayer : gameBoard.FirstPlayer;
            string url = Url.Action("Game", GameLogic.GetController(gameBoard), new {id = gameBoard.Id});
            var loginUrl = Url.Action("LoginUsingGuid", "Account", new { otherPlayer.Guid, redirectTo = url },
                                      Request.Url.Scheme);
            string body = message + "\r\n\r\n" + loginUrl;
            new SmtpClient().Send(mail, otherPlayer.Mail, string.Format("Mail {0} - {1}", GameLogic.GetName(GameLogic.GetGameType(gameBoard)), topic), body);
        }

        protected void SendOpponentMail(MailGamesContext db, IGameBoard board)
        {
            var gameState = GameLogic.GetGameState(board);
            switch (gameState)
            {
                case GameState.PlayerWon:
                    SendOpponentMail(db, board, "Unfortunately you didn't win this game...", "Game lost :(");
                    break;
                case GameState.OpponentWon:
                    SendOpponentMail(db, board, "Congratulations! You won the game, well played!", "Game won :)");
                    break;
                case GameState.OpponentsTurn:
                    SendOpponentMail(db, board, "It's your turn to make a move...", "Your turn");
                    break;
                case GameState.Tie:
                    SendOpponentMail(db, board, "No one won this game... perhaps time for a rematch?", "Tie");
                    break;
                case GameState.YourTurn:
                    break;
            }
        }
    }
}