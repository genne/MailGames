using System;

namespace MailGames.Chess
{
    public class Move
    {
        public int DeltaCol { get; set; }
        public int DeltaRow { get; set; }

        public Move Forward()
        {
            return new Move
            {
                DeltaCol = DeltaCol != 0 ? DeltaCol + DeltaCol / Math.Abs(DeltaCol) : 0,
                DeltaRow = DeltaRow != 0 ? DeltaRow + DeltaRow / Math.Abs(DeltaRow) : 0
            };
        }
    }
}