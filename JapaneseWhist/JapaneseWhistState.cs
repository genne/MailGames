using System;
using System.Collections.Generic;
using System.Linq;
using GameBase;

namespace JapaneseWhist
{
    public class JapaneseWhistState
    {
        public Random Random { get; set; }
        private Dictionary<GamePlayer, Dictionary<PlayerDeck, Deck>> _playerCards = new Dictionary<GamePlayer, Dictionary<PlayerDeck, Deck>>();
        private Dictionary<GamePlayer, int> _points = GameBaseLogic.GetAllPlayers().ToDictionary(x => x, x => 0);

        public GamePlayer CurrentPlayer { get; set; }
        public GamePlayer StartingPlayer { get; set; }

        public Deck CurrentStick { get; set; }

        public JapaneseWhistState(int seed)
        {
            Random = new Random(seed);
            CurrentPlayer = GamePlayer.FirstPlayer;

            DealCards();
        }

        public void DealCards()
        {
            CurrentStick = new Deck();
            var cards = Deck.CreateFull();
            cards.Randomize(Random);

            _playerCards[GamePlayer.FirstPlayer] = CreateEmptyDecks();
            _playerCards[GamePlayer.SecondPlayer] = CreateEmptyDecks();
            DealCardsTo(cards, PlayerDeck.Hand, 6);
            DealCardsTo(cards, PlayerDeck.Hidden, 10);
            DealCardsTo(cards, PlayerDeck.Visible, 10);
        }

        private void DealCardsTo(Deck cards, PlayerDeck playerDeck, int num)
        {
            for (int i = 0; i < num; i++)
            {
                cards.TakeFromTop(_playerCards[GamePlayer.FirstPlayer][playerDeck]);
                cards.TakeFromTop(_playerCards[GamePlayer.SecondPlayer][playerDeck]);
            }
        }

        private Dictionary<PlayerDeck, Deck> CreateEmptyDecks()
        {
            return Enum.GetValues(typeof (PlayerDeck)).OfType<PlayerDeck>().ToDictionary(p => p, p => new Deck());
        }

        public Deck GetPlayerDeck(GamePlayer player, PlayerDeck playerDeck)
        {
            return _playerCards[player][playerDeck];
        }

        public int GetPoints(GamePlayer player)
        {
            return _points[player];
        }

        public void AddPoints(GamePlayer gamePlayer, int points)
        {
            _points[gamePlayer] += points;
        }
    }

    public enum PlayerDeck
    {
        Hand,
        Visible,
        Hidden,
        Sticks,
        LastStick
    }
}