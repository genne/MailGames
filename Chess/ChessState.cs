using System.Collections.Generic;
using GameBase;

namespace Chess
{
    public class ChessState
    {
        private readonly List<Position> _movedPositions = new List<Position>();
        private Dictionary<int,Piece> Cells { get; set; }
        public GamePlayer CurrentPlayer { get; set; }

        public List<Piece> CapturedPieces { get; set; }

        public LinkedList<PieceMove> Moves { get; set; }

        public ChessState()
        {
            Cells = new Dictionary<int, Piece>();
            CapturedPieces= new List<Piece>();
            Moves = new LinkedList<PieceMove>();
        }

        public ChessState(ChessState state)
        {
            Cells = new Dictionary<int, Piece>(state.Cells);
            CurrentPlayer = state.CurrentPlayer;
            CapturedPieces = new List<Piece>(state.CapturedPieces);
            Moves = new LinkedList<PieceMove>(state.Moves);
            _movedPositions = new List<Position>(state._movedPositions);
        }

        public Piece GetCell(int cell)
        {
            return Cells.ContainsKey(cell) ? Cells[cell] : null;
        }

        public Piece GetCell(Position move)
        {
            return GetCell(move.ToInt());
        }

        public void SetCell(int cell, Piece piece)
        {
            if (piece != null)
            {
                var capturedPiece = GetCell(cell);
                if (capturedPiece != null)
                {
                    CapturedPieces.Add(capturedPiece);
                }
            }
            if (piece == null) Cells.Remove(cell);
            else Cells[cell] = piece;
        }

        public Dictionary<int, Piece> GetCells()
        {
            return Cells;
        }

        public bool HasMoved(Position rook)
        {
            return _movedPositions.Contains(rook);
        }

        public void MarkAsMoved(int to)
        {
            _movedPositions.Add(Position.FromInt(to));
        }

        public void SetCell(int row, int col, GamePlayer GamePlayer, PieceType pieceType)
        {
            SetCell(new Position(col, row).ToInt(), new Piece{ GamePlayer = GamePlayer, PieceType = pieceType});
        }

        public void AddMove(Piece piece, int @from, int to, Piece capturedPiece)
        {
            Moves.AddFirst(new PieceMove
            {
                Piece = piece, 
                From = Position.FromInt(from), 
                To = Position.FromInt(to),
                CapturedPiece = capturedPiece
            });
        }
    }

    public class Piece
    {
        public PieceType PieceType { get; set; }
        public GamePlayer GamePlayer { get; set; }
    }

    public enum PieceType
    {
        Pawn,
        Knight,
        Bishop,
        Rook,
        Queen,
        King
    }
}