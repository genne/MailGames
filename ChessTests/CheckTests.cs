using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chess;
using GameBase;
using MailGames.Context;
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
            state.SetCell(7, 3, GamePlayer.FirstPlayer, PieceType.King);
            state.SetCell(6, 3, GamePlayer.SecondPlayer, PieceType.Rook);
            bool check;
            ChessLogic.GetWinnerState(state, out check);
            Assert.AreEqual(true, check);
        }

        [TestMethod]
        public void TestCheckMate()
        {
            var state = new ChessState();
            state.SetCell(7, 3, GamePlayer.FirstPlayer, PieceType.King);
            state.SetCell(6, 3, GamePlayer.SecondPlayer, PieceType.Queen);
            state.SetCell(5, 3, GamePlayer.SecondPlayer, PieceType.Queen); // Guard the other queen
            bool isCheck;
            Assert.AreEqual(WinnerState.SecondPlayer, ChessLogic.GetWinnerState(state, out isCheck));
        }

        [TestMethod]
        public void TestStaleMate()
        {
            var state = new ChessState();
            state.SetCell(7, 3, GamePlayer.FirstPlayer, PieceType.King);
            state.SetCell(6, 4, GamePlayer.SecondPlayer, PieceType.Rook);
            state.SetCell(6, 2, GamePlayer.SecondPlayer, PieceType.Rook);
            bool isCheck;
            Assert.AreEqual(WinnerState.Tie, ChessLogic.GetWinnerState(state, out isCheck));
        }

        [TestMethod]
        public void TestCapturedPieces()
        {
            var state = ChessLogic.CreateInitialChessState();
            Assert.AreEqual(0, state.CapturedPieces.Count);
            ChessLogic.ApplyMove(state, new Position(3, 6).ToInt(), new Position(3, 4).ToInt(), null);
            Assert.AreEqual(0, state.CapturedPieces.Count);
        }
    }
}
