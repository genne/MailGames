using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chess;

namespace MailGames.Context
{
    public class ChessBoard : IGameBoard
    {
        public Guid Id { get; set; }
        public virtual ICollection<ChessMove> ChessMoves { get; set; }

        public virtual Player FirstPlayer { get; set; }
        public virtual Player SecondPlayer { get; set; }

        public PieceColor? Winner { get; set; }
        public ChessRunningState RunningState { get; set; }
    }

    public interface IGameBoard
    {
        Guid Id { get; set; }
        Player FirstPlayer { get; set; }
        Player SecondPlayer { get; set; }
    }
}