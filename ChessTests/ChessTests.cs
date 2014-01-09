using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess;
using GameBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class ChessTests
    {
        [TestMethod]
        public void TestGetProgress()
        {
            var state = ChessLogic.CreateInitialChessState();
            Assert.AreEqual(50, ChessLogic.GetProgress(state, GamePlayer.FirstPlayer));
        }

        [TestMethod]
        public void TestGetProgressWhiteNoQueen()
        {
            var state = ChessLogic.CreateInitialChessState();
            state.SetCell(new Position(3, 7).ToInt(), null);
            Assert.AreEqual(0, ChessLogic.GetProgress(state, GamePlayer.FirstPlayer));
            Assert.AreEqual(100, ChessLogic.GetProgress(state, GamePlayer.SecondPlayer));
        }

        [TestMethod]
        public void TestGetProgressBlackNoQueeen()
        {
            var state = ChessLogic.CreateInitialChessState();
            state.SetCell(new Position(3, 0).ToInt(), null);
            Assert.AreEqual(100, ChessLogic.GetProgress(state, GamePlayer.FirstPlayer));
            Assert.AreEqual(0, ChessLogic.GetProgress(state, GamePlayer.SecondPlayer));
        }

        [TestMethod]
        public void TestParsePosition()
        {
            var pos = new Position(2, 4);
            Assert.AreEqual(pos, ChessLogic.ParseChessPosition(ChessLogic.FormatChessPosition(pos)));
        }

        [TestMethod]
        public void TestEnPassant()
        {
            var state = new ChessState();
            AddKings(state);
            state.SetCell("a2", GamePlayer.FirstPlayer, PieceType.Pawn);
            state.SetCell("b4", GamePlayer.SecondPlayer, PieceType.Pawn);
            ChessLogic.ApplyMove(state, "e1", "e2", null);
            ChessLogic.ApplyMove(state, "e8", "e7", null);
            ChessLogic.ApplyMove(state, "a2", "a4", null);
            ChessLogic.ValidateMove(state, "b4", "a3", null, validateTarget: true);
            ChessLogic.ApplyMove(state, "b4", "a3", null);
            Assert.IsNull(state.GetCell("a4"));
        }

        private void AddKings(ChessState state)
        {
            state.SetCell("e1", GamePlayer.FirstPlayer, PieceType.King);
            state.SetCell("e8", GamePlayer.SecondPlayer, PieceType.King);
        }
    }
}
