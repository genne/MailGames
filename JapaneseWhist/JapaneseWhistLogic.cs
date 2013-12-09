using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase;

namespace JapaneseWhist
{
    public static class JapaneseWhistLogic
    {
        public static void Select(JapaneseWhistState state, PlayerDeckIndex index)
        {
            Select(state, index.PlayerDeck, index.Index);
        }
        public static void Select(JapaneseWhistState state, PlayerDeck playerDeck, int index)
        {
            ValidateCard(state, playerDeck, index);
            state.GetPlayerDeck(state.CurrentPlayer, playerDeck).MoveCard(index, state.CurrentStick);
            if (playerDeck == PlayerDeck.Visible)
            {
                var hiddenDeck = state.GetPlayerDeck(state.CurrentPlayer, PlayerDeck.Hidden);
                var hiddenCard = hiddenDeck.Get(index);
                if (hiddenCard != null)
                {
                    var visibleDeck = state.GetPlayerDeck(state.CurrentPlayer, PlayerDeck.Visible);
                    hiddenDeck.MoveCardToSameIndex(index, visibleDeck);
                }
            }
            if (state.CurrentStick.Count() == 2)
            {
                MoveLastStickToSticks(state);

                var stickWinner = FirstPlayerTookStick(state.CurrentStick)
                                      ? GameBaseLogic.GetNextPlayer(state.CurrentPlayer)
                                      : state.CurrentPlayer;
                state.CurrentStick.MoveAllTo(state.GetPlayerDeck(stickWinner, PlayerDeck.LastStick));
                state.CurrentPlayer = stickWinner;

                if (IsRoundComplete(state))
                {
                    MoveLastStickToSticks(state);

                    var firstPlayerStickCount = state.GetPlayerDeck(GamePlayer.FirstPlayer, PlayerDeck.Sticks).Count()/2;
                    if (firstPlayerStickCount > 13)
                        state.AddPoints(GamePlayer.FirstPlayer, firstPlayerStickCount - 13);
                    else
                        state.AddPoints(GamePlayer.SecondPlayer, 13 - firstPlayerStickCount);

                    state.CurrentPlayer = state.StartingPlayer = GameBaseLogic.GetNextPlayer(state.StartingPlayer);

                    state.DealCards();
                }
            }
            else
            {
                state.CurrentPlayer = GameBaseLogic.GetNextPlayer(state.CurrentPlayer);
            }
        }

        private static void MoveLastStickToSticks(JapaneseWhistState state)
        {
            foreach (var player in GameBaseLogic.GetAllPlayers())
            {
                state.GetPlayerDeck(player, PlayerDeck.LastStick).MoveAllTo(state.GetPlayerDeck(player, PlayerDeck.Sticks));
            }
        }

        private static bool IsRoundComplete(JapaneseWhistState state)
        {
            return GameBaseLogic.GetAllPlayers().Sum(p => state.GetPlayerDeck(p, PlayerDeck.Sticks).Count() + state.GetPlayerDeck(p, PlayerDeck.LastStick).Count()) == 52;
        }

        private static void ValidateCard(JapaneseWhistState state, PlayerDeck playerDeck, int index)
        {
            if (playerDeck != PlayerDeck.Hand && playerDeck != PlayerDeck.Visible) 
                throw new InvalidOperationException("Invalid player deck");
            if (playerDeck == PlayerDeck.Hand && IsFirstStick(state)) 
                throw new InvalidOperationException("Can't play from hand cards now");
            if (playerDeck == PlayerDeck.Hidden &&
                state.GetPlayerDeck(state.CurrentPlayer, PlayerDeck.Visible).Get(index) != null)
                throw new InvalidOperationException("Can't play hidden card until visible is taken");
            if (state.CurrentStick.Count() == 1)
            {
                var currentColor = state.CurrentStick.Get(0).Color;
                if (state.GetPlayerDeck(state.CurrentPlayer, playerDeck).Get(index).Color != currentColor)
                {
                    bool playerHasNoCardsOfSameColor = GetAllPlayableCards(state).Any(c => c.Color == currentColor);
                    if (playerHasNoCardsOfSameColor) throw new InvalidOperationException("Must play same color");
                }
            }
        }

        private static IEnumerable<Card> GetAllPlayableCards(JapaneseWhistState state)
        {
            var cards = new List<Card>();
            if (!IsFirstStick(state))
                cards.AddRange(state.GetPlayerDeck(state.CurrentPlayer, PlayerDeck.Hand).All());
            cards.AddRange(state.GetPlayerDeck(state.CurrentPlayer, PlayerDeck.Visible).All());
            return cards;
        }

        private static bool IsFirstStick(JapaneseWhistState state)
        {
            return GameBaseLogic.GetAllPlayers().All(p => state.GetPlayerDeck(p, PlayerDeck.LastStick).Count() == 0);
        }

        private static bool FirstPlayerTookStick(Deck currentStick)
        {
            var baseCard = currentStick.Get(0);
            var otherCard = currentStick.Get(1);
            return otherCard.Color != baseCard.Color || baseCard.HasHigherValueThan(otherCard);
        }

        public static IEnumerable<PlayerDeckIndex> GetPlayableCardIndices(JapaneseWhistState state)
        {
            var allVisibleCards = GetAllVisibleCards(state);
            if (state.CurrentStick.Count() > 0)
            {
                var currentColor = state.CurrentStick.Get(0).Color;
                var cardsOfSameColor = allVisibleCards.Where(c => GetCard(state, c).Color == currentColor);
                if (cardsOfSameColor.Any())
                    return cardsOfSameColor;
            }
            return allVisibleCards;
        }

        public static Card GetCard(JapaneseWhistState state, PlayerDeckIndex playerDeckIndex)
        {
            return state.GetPlayerDeck(state.CurrentPlayer, playerDeckIndex.PlayerDeck).Get(playerDeckIndex.Index);
        }

        private static IEnumerable<PlayerDeckIndex> GetAllVisibleCards(JapaneseWhistState state)
        {
            var cards = new List<PlayerDeckIndex>();
            if (!IsFirstStick(state))
                cards.AddRange(GetDeckIndices(state, PlayerDeck.Hand));
            cards.AddRange(GetDeckIndices(state, PlayerDeck.Visible));
            return cards;
        }

        private static IEnumerable<PlayerDeckIndex> GetDeckIndices(JapaneseWhistState state, PlayerDeck playerDeck)
        {
            return
                state.GetPlayerDeck(state.CurrentPlayer, playerDeck)
                     .AllIndies()
                     .Select(i => new PlayerDeckIndex(playerDeck, i));
        }

        public static GamePlayer? GetWinner(JapaneseWhistState state)
        {
            return GameBaseLogic.GetAllPlayers().OfType<GamePlayer?>().FirstOrDefault(p => state.GetPoints(p.Value) >= 10);
        }

        public static bool CanPlayFromHand(JapaneseWhistState state)
        {
            return !IsFirstStick(state);
        }
    }
}
