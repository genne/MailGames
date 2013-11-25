using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chess;
using GameBase;

namespace MailGames.Models
{
    public class ChessBoardViewModel
    {
        public Piece[,] Cells { get; set; }

        public GamePlayer CurrentColor { get; set; }

        public Guid Id { get; set; }

        public GamePlayer PlayerColor { get; set; }

        public GamePlayer? AttackedKing { get; set; }

        public IEnumerable<int> OpponentMoves { get; set; }
    }
}