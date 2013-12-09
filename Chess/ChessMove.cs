using GameBase;

namespace Chess
{
    public class PieceMove
    {
        public Piece Piece { get; set; }

        public Position From { get; set; }
        public Position To { get; set; }

        public Piece CapturedPiece { get; set; }
    }
}