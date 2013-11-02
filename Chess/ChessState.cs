using System.Collections.Generic;
using MailGames.Chess;

namespace Chess
{
    public class ChessState
    {
        private readonly List<Position> _movedPositions = new List<Position>();
        private Dictionary<int,Piece> Cells { get; set; }
        public PieceColor CurrentColor { get; set; }

        public ChessState()
        {
            Cells = new Dictionary<int, Piece>();
        }

        public ChessState(ChessState state)
        {
            Cells = new Dictionary<int, Piece>(state.Cells);
            CurrentColor = state.CurrentColor;
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

        public void SetCell(int row, int col, PieceColor pieceColor, PieceType pieceType)
        {
            SetCell(new Position{ Row = row, Col = col}.ToInt(), new Piece{ PieceColor = pieceColor, PieceType = pieceType});
        }
    }

    public class Piece
    {
        public PieceType PieceType { get; set; }
        public PieceColor PieceColor { get; set; }
    }

    public enum PieceColor
    {
        White,
        Black
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