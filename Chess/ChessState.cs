﻿using System.Collections.Generic;
using GameBase;

namespace Chess
{
    public class ChessState
    {
        private readonly List<Position> _movedPositions = new List<Position>();
        private Dictionary<int,Piece> Cells { get; set; }
        public GamePlayer CurrentPlayer { get; set; }

        public List<Piece> CapturedPieces { get; set; }

        public LinkedList<PieceMove> LastMoves { get; set; }

        public ChessState()
        {
            Cells = new Dictionary<int, Piece>();
            CapturedPieces= new List<Piece>();
            LastMoves = new LinkedList<PieceMove>();
        }

        public ChessState(ChessState state)
        {
            Cells = new Dictionary<int, Piece>(state.Cells);
            CurrentPlayer = state.CurrentPlayer;
            CapturedPieces = new List<Piece>(state.CapturedPieces);
            LastMoves = new LinkedList<PieceMove>(state.LastMoves);
            _movedPositions = new List<Position>(state._movedPositions);
        }

        public Piece GetCell(int cell)
        {
            Piece piece;
            Cells.TryGetValue(cell, out piece);
            return piece;
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
            SetCell(new Position(col, row).ToInt(), new Piece(GamePlayer, pieceType));
        }

        public void AddMove(Piece piece, int @from, int to, Piece capturedPiece)
        {
            LastMoves.AddFirst(new PieceMove
            {
                Piece = piece, 
                From = Position.FromInt(from), 
                To = Position.FromInt(to),
                CapturedPiece = capturedPiece
            });
        }

        public void SetCell(string cell, GamePlayer player, PieceType pieceType)
        {
            SetCell(ChessLogic.ParseChessPosition(cell).ToInt(), new Piece(player, pieceType));
        }

        public Piece GetCell(string cell)
        {
            return GetCell(ChessLogic.ParseChessPosition(cell));
        }
    }
}