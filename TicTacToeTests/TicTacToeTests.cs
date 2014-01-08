using System;
using System.Linq;
using GameBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiniProfiler.Windows;
using StackExchange.Profiling;
using TicTacToe;

namespace TicTacToeTests
{
    [TestClass]
    public class TicTacToeTests
    {
        [TestMethod]
        public void TestTie()
        {
            var state = new TicTacToeState(TicTacToeVariant.Original);
            SetState(state, new[]
            {
                "XXO",
                "OXX",
                "XOO"
            });
            Assert.AreEqual(WinnerState.Tie, TicTacToeLogic.GetWinner(state));
        }

        private void SetState(TicTacToeState state, string[] matrix)
        {
            var random = new Random();
            var numLeft = matrix.Sum(row => row.Sum(c => c != ' ' ? 1 : 0));
            while (true)
            {
                int x = random.Next(state.Width);
                int y = random.Next(state.Height);
                if (state.Get(new Position(x, y)) == null)
                {
                    var color = matrix[y][x] == 'X' ? GamePlayer.FirstPlayer : GamePlayer.SecondPlayer;
                    if (state.CurrentPlayer == color)
                    {
                        state.Play(x, y);
                        numLeft--;
                        if (numLeft == 0) return;
                    }
                }
            }
        }

        [TestMethod]
        public void TestHorizontalWinner()
        {
            var state = new TicTacToeState(TicTacToeVariant.Original);
            state.Play(1, 1);
            state.Play(1, 2);
            state.Play(0, 1);
            state.Play(1, 0);
            state.Play(2, 1);
            Assert.AreEqual(WinnerState.FirstPlayer, TicTacToeLogic.GetWinner(state));
        }

        [TestMethod]
        public void TestVerticalWinner()
        {
            var state = new TicTacToeState(TicTacToeVariant.Original);
            state.Play(1, 1);
            state.Play(2, 1);
            state.Play(1, 0);
            state.Play(0, 1);
            state.Play(1, 2);
            Assert.AreEqual(WinnerState.FirstPlayer, TicTacToeLogic.GetWinner(state));
        }

        [TestMethod]
        public void TestDiagonalWinner()
        {
            var state = new TicTacToeState(TicTacToeVariant.Original);
            state.Play(1, 1);
            state.Play(1, 2);
            state.Play(0, 0);
            state.Play(1, 0);
            state.Play(2, 2);
            var winner = TicTacToeLogic.GetWinner(state);
            Assert.AreEqual(WinnerState.FirstPlayer, winner);
        }

        [TestMethod]
        public void TestSimpleAI()
        {
            var ai = new TicTacToeAI(0);
            var state = new TicTacToeState(TicTacToeVariant.Lufferschess);
            state.Play(8, 8);
            state.Play(0, 0);
            state.Play(8, 9);
            state.Play(8, 7);
            state.Play(8, 10);
            state.Play(1, 1);
            state.Play(8, 11);
            var move = ai.GetRandomBestMove(state);
            Assert.AreEqual(8, move.X);
            Assert.AreEqual(12, move.Y);
        }
    }
}
