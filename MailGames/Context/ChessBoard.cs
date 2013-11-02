using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chess;

namespace MailGames.Context
{
    public class ChessBoard
    {
        public Guid Id { get; set; }
        public virtual ICollection<ChessMove> ChessMoves { get; set; }

        [Required]
        public virtual Player WhitePlayer { get; set; }
        [Required]
        public virtual Player BlackPlayer { get; set; }

        public PieceColor? Winner { get; set; }
        public ChessRunningState RunningState { get; set; }
    }
}