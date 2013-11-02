using System.Collections.Generic;
using Chess;

namespace MailGames.Context
{
    public class ChessMove
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }

        public virtual ICollection<PawnConversion> PawnConversion { get; set; }
    }

    public class PawnConversion
    {
        public int Id { get; set; }
        public PieceType ConvertTo { get; set; }
    }
}