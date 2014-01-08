using GameBase;

namespace Chess
{
    public class Piece
    {
        public PieceType PieceType { get; private set; }
        public GamePlayer GamePlayer { get; private set; }

        public Piece(GamePlayer gamePlayer, PieceType pieceType)
        {
            PieceType = pieceType;
            GamePlayer = gamePlayer;
        }
    }
}