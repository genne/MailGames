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
    }
}
