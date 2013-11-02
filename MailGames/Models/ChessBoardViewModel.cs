using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Chess;

namespace MailGames.Models
{
    public class ChessBoardViewModel
    {
        public Piece[,] Cells { get; set; }

        public PieceColor CurrentColor { get; set; }

        public Guid Id { get; set; }

        public PieceColor PlayerColor { get; set; }
    }
}