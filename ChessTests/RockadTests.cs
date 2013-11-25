using System;
using System.Linq;
using Chess;
using GameBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class RockadTests
    {
        private static readonly int KingPos = new Position(4, 7).ToInt();
        private static readonly int RookRightPos = new Position(7, 7).ToInt();
        private static readonly int KingRockadRightPos = new Position(6, 7).ToInt();

        [TestMethod]
        public void TestRockadRight()
        {
            TestRockad(RookRightPos, KingRockadRightPos);
        }

        [TestMethod]
        public void TestRockadLeft()
        {
            TestRockad(new Position(0, 7).ToInt(), new Position(2, 7).ToInt());
        }

        [TestMethod]
        public void TestRockadWhenKingAttacked()
        {
            TestRockad(RookRightPos, KingRockadRightPos, false, Tuple.Create(new Position(5, 6), new Piece{ GamePlayer = GamePlayer.SecondPlayer, PieceType = PieceType.Pawn }));
        }

        [TestMethod]
        public void TestRockadWhenSpaceAttacked()
        {
            TestRockad(RookRightPos, KingRockadRightPos, false, Tuple.Create(new Position(5, 6), new Piece { GamePlayer = GamePlayer.SecondPlayer, PieceType = PieceType.Rook }));
        }

        [TestMethod]
        public void TestRockadWhenOccupied()
        {
            TestRockad(RookRightPos, KingRockadRightPos, false, Tuple.Create(new Position(5, 7), new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.Knight }));
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
            state.SetCell(rookPos, new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.Rook });
            state.SetCell(KingPos, new Piece { GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.King });

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
            state.SetCell(KingPos, new Piece{ GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.King });
            var rook = new Piece {GamePlayer = GamePlayer.FirstPlayer, PieceType = PieceType.Rook};
            state.SetCell(RookRightPos, rook);
            ChessLogic.ApplyMove(state, KingPos, KingRockadRightPos, null);
            int rookRightPosAfterRockad = new Position(5, 7).ToInt();
            Assert.AreEqual(state.GetCell(rookRightPosAfterRockad), rook);
        }
    }
}
