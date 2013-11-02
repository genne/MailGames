using System;
using System.Linq;
using Chess;
using MailGames.Chess;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class RockadTests
    {
        private static readonly int KingPos = new Position {Row = 7, Col = 4}.ToInt();
        private static readonly int RookRightPos = new Position { Row = 7, Col = 7 }.ToInt();
        private static readonly int KingRockadRightPos = new Position { Row = 7, Col = 6 }.ToInt();

        [TestMethod]
        public void TestRockadRight()
        {
            TestRockad(RookRightPos, KingRockadRightPos);
        }

        [TestMethod]
        public void TestRockadLeft()
        {
            TestRockad(new Position { Row = 7, Col = 0 }.ToInt(), new Position { Row = 7, Col = 2 }.ToInt());
        }

        [TestMethod]
        public void TestRockadWhenKingAttacked()
        {
            TestRockad(RookRightPos, KingRockadRightPos, false, Tuple.Create(new Position{ Row = 6, Col = 5}, new Piece{ PieceColor = PieceColor.Black, PieceType = PieceType.Pawn }));
        }

        [TestMethod]
        public void TestRockadWhenSpaceAttacked()
        {
            TestRockad(RookRightPos, KingRockadRightPos, false, Tuple.Create(new Position { Row = 6, Col = 5 }, new Piece { PieceColor = PieceColor.Black, PieceType = PieceType.Rook }));
        }

        [TestMethod]
        public void TestRockadWhenOccupied()
        {
            TestRockad(RookRightPos, KingRockadRightPos, false, Tuple.Create(new Position { Row = 7, Col = 5 }, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.Knight }));
        }

        [TestMethod]
        public void TestRockadWhenKingMoved()
        {
            var state = new ChessState();
            state.MarkAsMoved(KingPos);
            TestRockad(state, RookRightPos, KingRockadRightPos, false);
        }

        [TestMethod]
        public void TestRockadWhenRookMoved()
        {
            var state = new ChessState();
            state.MarkAsMoved(RookRightPos);
            TestRockad(state, RookRightPos, KingRockadRightPos, false);
        }

        // ReSharper disable UnusedParameter.Local
        private static void TestRockad(int rookPos, int kingTargetPos, bool canDoRockad = true, params Tuple<Position, Piece>[] otherPieces)
// ReSharper restore UnusedParameter.Local
        {
            var state = new ChessState();
            TestRockad(state, rookPos, kingTargetPos, canDoRockad, otherPieces);
        }

        private static void TestRockad(ChessState state, int rookPos, int kingTargetPos, bool canDoRockad, params Tuple<Position, Piece>[] otherPieces)
        {
            state.SetCell(rookPos, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.Rook });
            state.SetCell(KingPos, new Piece { PieceColor = PieceColor.White, PieceType = PieceType.King });

            foreach (var p in otherPieces)
            {
                state.SetCell(p.Item1.ToInt(), p.Item2);
            }
            var availableTargets = ChessLogic.GetAvailableTargets(state, KingPos).ToArray();
            Assert.AreEqual(canDoRockad, availableTargets.Contains(kingTargetPos));
        }

        [TestMethod]
        public void TestRockadMove()
        {
            var state = new ChessState();
            state.SetCell(KingPos, new Piece{ PieceColor = PieceColor.White, PieceType = PieceType.King });
            var rook = new Piece {PieceColor = PieceColor.White, PieceType = PieceType.Rook};
            state.SetCell(RookRightPos, rook);
            ChessLogic.ApplyMove(state, KingPos, KingRockadRightPos, null);
            int rookRightPosAfterRockad = new Position{ Col = 5, Row = 7 }.ToInt();
            Assert.AreEqual(state.GetCell(rookRightPosAfterRockad), rook);
        }
    }
}
