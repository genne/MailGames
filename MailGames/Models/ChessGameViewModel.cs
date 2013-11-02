using System.Web.Mvc;

namespace MailGames.Models
{
    public class ChessGameViewModel
    {
        public ChessBoardViewModel Board { get; set; }

        public string YourMail { get; set; }

        public string OpponentMail { get; set; }
    }
}