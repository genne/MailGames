using System;
using System.Linq;
using GameBase;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Othello;

namespace OthelloTests
{
    [TestClass]
    public class OthelloTests
    {
        [TestMethod]
        public void TestInitialState()
        {
            var state = new OthelloState();
            Assert.AreEqual(GamePlayer.FirstPlayer, state.Get(new Position(3, 3)));
            Assert.AreEqual(GamePlayer.FirstPlayer, state.Get(new Position(4, 4)));
            Assert.AreEqual(GamePlayer.SecondPlayer, state.Get(new Position(3, 4)));
            Assert.AreEqual(GamePlayer.SecondPlayer, state.Get(new Position(4, 3)));
        }

        [TestMethod]
        public void TestPlay()
        {
            var state = new OthelloState();
            OthelloLogic.Play(state, new Position(5, 3));
            Assert.AreEqual(GamePlayer.FirstPlayer, state.Get(new Position(4, 3)));
        }

        [TestMethod]
        public void TestPlayRow()
        {
            var state = new OthelloState(empty: true);
            for (int x = 3; x < 7; x++)
            {
                state.Set(new Position(x, 0));
            }
            state.CurrentPlayer = GamePlayer.SecondPlayer;
            state.Set(new Position(7, 0));
            OthelloLogic.Play(state, new Position(2, 0));
            for (int x = 3; x < 7; x++)
            {
                Assert.AreEqual(GamePlayer.SecondPlayer, state.Get(new Position(x, 0)));
            }
        }

        [TestMethod]
        public void TestSkipWhenNoValidTargets()
        {
            var state = new OthelloState(empty: true);
            for (int x = 1; x < OthelloLogic.Size-1; x++)
            {
                state.Set(new Position(x, 0));
            }
            state.CurrentPlayer = GamePlayer.SecondPlayer;
            state.Set(new Position(2, 0));
            OthelloLogic.Play(state, new Position(0, 0));
            Assert.AreEqual(GamePlayer.SecondPlayer, state.CurrentPlayer);
        }

        [TestMethod]
        public void TestGetValidTargets()
        {
            var state = new OthelloState();
            var validTargets = OthelloLogic.GetValidTargets(state).ToArray();
            Assert.AreEqual(4, validTargets.Length);
            CollectionAssert.Contains(validTargets, new Position(4, 2));
            CollectionAssert.Contains(validTargets, new Position(2, 4));
            CollectionAssert.Contains(validTargets, new Position(3, 5));
            CollectionAssert.Contains(validTargets, new Position(5, 3));
        }

        [TestMethod]
        public void TestGetWinner()
        {
            var state = new OthelloState();
            Assert.IsNull(OthelloLogic.GetWinner(state));

            foreach (var pos in OthelloLogic.GetAllPositions())
            {
                if (state.Get(pos) != null)
                    state.Set(pos);
            }
            Assert.AreEqual(WinnerState.FirstPlayer, OthelloLogic.GetWinner(state), "Winner when no playable positions");

            foreach (var pos in OthelloLogic.GetAllPositions())
            {
                state.Set(pos);
            }
            Assert.AreEqual(WinnerState.FirstPlayer, OthelloLogic.GetWinner(state), "Winner when board full");

            state.CurrentPlayer = GamePlayer.SecondPlayer;
            foreach (var pos in OthelloLogic.GetAllPositions())
            {
                state.Set(pos);
            }
            Assert.AreEqual(WinnerState.SecondPlayer, OthelloLogic.GetWinner(state), "Second played winner");

            foreach (var pos in OthelloLogic.GetAllPositions())
            {
                state.Set(pos);
                state.CurrentPlayer = GameBaseLogic.GetNextPlayer(state.CurrentPlayer);
            }
            Assert.AreEqual(WinnerState.Tie, OthelloLogic.GetWinner(state), "Tie");
        }
    }
}
