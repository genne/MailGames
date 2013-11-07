using System.Net.Mail;
using System.Web.Mvc;
using MailGames.Context;
using WebMatrix.WebData;

namespace MailGames.Controllers
{
    public class GameControllerBase : Controller
    {
        protected void SendMail(MailGamesContext db, IGameBoard gameBoard, string message, string topic)
        {
            var mail = PlayerManager.GetCurrent(db).Mail;
            var otherPlayer = gameBoard.FirstPlayer.Id == WebSecurity.CurrentUserId ? gameBoard.SecondPlayer : gameBoard.FirstPlayer;
            string url = Url.Action("Game", null, new {id = gameBoard.Id, player = otherPlayer.Guid});
            var loginUrl = Url.Action("LoginUsingGuid", "Account", new { otherPlayer.Guid, redirectTo = url },
                                      Request.Url.Scheme);
            string body = message + "\r\n\r\n" + loginUrl;
            new SmtpClient().Send(mail, otherPlayer.Mail, string.Format("Mail {0} - {1}", GameBoardInfo.GetName(GameBoardInfo.GetGameType(gameBoard)), topic), body);
        }
    }
}