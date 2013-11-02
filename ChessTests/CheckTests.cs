using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess;
using MailGames.Chess;
using MailGames.Controllers;
using MailGames.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChessTests
{
    [TestClass]
    public class CheckTests
    {
        [TestMethod]
        public void TestCheck()
        {
            var state = new ChessState();
            state.SetCell(7, 3, PieceColor.White, PieceType.King);
            state.SetCell(6, 3, PieceColor.Black, PieceType.Rook);
            Assert.AreEqual(ChessRunningState.Check, ChessLogic.GetRunningState(state));
        }

        [TestMethod]
        public void TestCheckMate()
        {
            var state = new ChessState();
            state.SetCell(7, 3, PieceColor.White, PieceType.King);
            state.SetCell(6, 3, PieceColor.Black, PieceType.Queen);
            state.SetCell(5, 3, PieceColor.Black, PieceType.Queen); // Guard the other queen
            Assert.AreEqual(ChessRunningState.CheckMate, ChessLogic.GetRunningState(state));
        }

        [TestMethod]
        public void TestStaleMate()
        {
            var state = new ChessState();
            state.SetCell(7, 3, PieceColor.White, PieceType.King);
            state.SetCell(6, 4, PieceColor.Black, PieceType.Rook);
            state.SetCell(6, 2, PieceColor.Black, PieceType.Rook);
            Assert.AreEqual(ChessRunningState.StaleMate, ChessLogic.GetRunningState(state));
        }
    }
}
