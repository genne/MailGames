using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;
using JapaneseWhist;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JapaneseWhistTests
{
    [TestClass]
    public class JapaneseWhistTests
    {
        [TestMethod]
        public void TestInitialBoard()
        {
            var state = new JapaneseWhistState(1);
            foreach (var player in GameBaseLogic.GetAllPlayers())
            {
                Assert.AreEqual(6, state.GetPlayerDeck(player, PlayerDeck.Hand).Count());
                Assert.AreEqual(10, state.GetPlayerDeck(player, PlayerDeck.Visible).Count());
                Assert.AreEqual(10, state.GetPlayerDeck(player, PlayerDeck.Hidden).Count());
                Assert.AreEqual(0, state.GetPlayerDeck(player, PlayerDeck.Sticks).Count());
                Assert.AreEqual(0, state.GetPlayerDeck(player, PlayerDeck.LastStick).Count());
            }
            Assert.AreEqual(GamePlayer.FirstPlayer, state.CurrentPlayer);
        }

        [TestMethod]
        public void TestSelect()
        {
            var state = new JapaneseWhistState(1);
            var cardIndex = 8;
            JapaneseWhistLogic.Select(state, PlayerDeck.Visible, cardIndex);
            Assert.AreEqual(10, state.GetPlayerDeck(GamePlayer.FirstPlayer, PlayerDeck.Visible).Count());
            Assert.AreEqual(9, state.GetPlayerDeck(GamePlayer.FirstPlayer, PlayerDeck.Hidden).Count());
            Assert.AreEqual(null, state.GetPlayerDeck(GamePlayer.FirstPlayer, PlayerDeck.Hidden).Get(cardIndex));
            Assert.AreEqual(1, state.CurrentStick.Count());
            Assert.AreEqual(GamePlayer.SecondPlayer, state.CurrentPlayer);
        }

        [TestMethod]
        public void TestTakeStick()
        {
            var state = new JapaneseWhistState(1);
            var winner = PlayStick(state);

            Assert.AreEqual(2, state.GetPlayerDeck(winner, PlayerDeck.LastStick).Count());
            Assert.AreEqual(0, state.CurrentStick.Count());
        }

        private static GamePlayer PlayStick(JapaneseWhistState state)
        {
            var card1 = PlayAnyCard(state);

            var card2Index = JapaneseWhistLogic.GetPlayableCardIndices(state).First();
            var card2 = JapaneseWhistLogic.GetCard(state, card2Index);
            var winner = card1.Color != card2.Color || card1.HasHigherValueThan(card2) ? GameBaseLogic.GetNextPlayer(state.CurrentPlayer) : state.CurrentPlayer;
            JapaneseWhistLogic.Select(state, card2Index);
            return winner;
        }

        private static Card PlayAnyCard(JapaneseWhistState state)
        {
            var cardIndex = JapaneseWhistLogic.GetPlayableCardIndices(state).First();
            var card = JapaneseWhistLogic.GetCard(state, cardIndex);
            JapaneseWhistLogic.Select(state, cardIndex);
            return card;
        }

        [TestMethod]
        public void TestRoundComplete()
        {
            var state = new JapaneseWhistState(2);
            var numSticks = new Dictionary<GamePlayer, int>{ { GamePlayer.FirstPlayer, 0 }, { GamePlayer.SecondPlayer, 0}};
            for (int i = 0; i < 26; i++)
            {
                var stickWinner = PlayStick(state);
                numSticks[stickWinner]++;
            }
            foreach(var player in GameBaseLogic.GetAllPlayers())
                Assert.AreEqual(Math.Max(0, numSticks[player] - 13), state.GetPoints(player));

            Assert.AreEqual(GamePlayer.SecondPlayer, state.CurrentPlayer);
        }

        [TestMethod]
        public void TestGameWinner()
        {
            var state = new JapaneseWhistState(new Random().Next());

            while(GameBaseLogic.GetAllPlayers().All(p => state.GetPoints(p) < 10))
            {
                Assert.IsNull(JapaneseWhistLogic.GetWinner(state));
                PlayStick(state);
            }

            Assert.IsNotNull(JapaneseWhistLogic.GetWinner(state));
        }
    }
}
