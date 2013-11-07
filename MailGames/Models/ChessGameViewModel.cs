using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Chess;
using MailGames.Chess;

namespace MailGames.Models
{
    public class ChessGameViewModel : GameViewModel
    {
        public ChessBoardViewModel Board { get; set; }

        public bool IsCheck { get; set; }

        public IEnumerable<Piece> CapturedPieces { get; set; }

        public IEnumerable<PieceMove> Moves { get; set; }

        public class Move
        {
            public Piece Piece { get; set; }

            public Position From { get; set; }
            public Position To { get; set; }
        }
    }
}