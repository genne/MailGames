﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Chess;
using GameBase;
using MailGames.Context;

namespace MailGames.Models
{
    public class ChessGameViewModel : GameViewModel
    {
        public ChessGameViewModel(IGameBoard board) : base(board)
        {
        }

        public Piece[,] Cells { get; set; }

        public GamePlayer CurrentColor { get; set; }

        public Guid Id { get; set; }

        public GamePlayer PlayerColor { get; set; }

        public GamePlayer? AttackedKing { get; set; }

        public IEnumerable<int> OpponentMoves { get; set; }

        public bool IsCheck { get; set; }

        public IEnumerable<Piece> CapturedPieces { get; set; }

        public IEnumerable<PieceMove> Moves { get; set; }

        public float Progress { get; set; }

        public class Move
        {
            public Piece Piece { get; set; }

            public Position From { get; set; }
            public Position To { get; set; }
        }
    }
}