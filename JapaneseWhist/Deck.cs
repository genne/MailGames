using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace JapaneseWhist
{
    public class Deck
    {
        private List<Card> _cards;

        public Deck()
        {
            _cards = new List<Card>();
        }
        private Deck(IEnumerable<Card> cards)
        {
            _cards = cards.ToList();
        }

        public static Deck CreateFull()
        {
            return new Deck(from val in GetAllCardValues()
                   from color in GetAllColors()
                   select new Card(color, val));
        }

        private static IEnumerable<CardColor> GetAllColors()
        {
            return Enum.GetValues(typeof(CardColor)).OfType<CardColor>();
        }

        private static IEnumerable<CardValue> GetAllCardValues()
        {
            return Enum.GetValues(typeof(CardValue)).OfType<CardValue>();
        }

        public void Randomize(Random rng)
        {
            var n = _cards.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = _cards[k];
                _cards[k] = _cards[n];
                _cards[n] = value;
            }
        }

        public void TakeFromTop(Deck moveTo)
        {
            var card = _cards.First();
            _cards.Remove(card);
            moveTo._cards.Add(card);
        }

        public int Count()
        {
            return _cards.Count(c => c != null);
        }

        public Card Get(int index)
        {
            return _cards[index];
        }

        public void MoveCard(int index, Deck toDeck)
        {
            var card = _cards[index];
            if (card == null) throw new ArgumentException("No card at specified index", "index");
            toDeck._cards.Add(card);
            _cards[index] = null;
        }

        public void MoveCardToSameIndex(int index, Deck toDeck)
        {
            if (toDeck._cards[index] != null) throw new ArgumentException("Index already taken", "index");
            toDeck._cards[index] = _cards[index];
            _cards[index] = null;
        }

        public IEnumerable<Card> All(bool includeEmpty = false)
        {
            return includeEmpty ? _cards : _cards.Where(c => c != null);
        }

        public int IndexOf(Card card)
        {
            return _cards.IndexOf(card);
        }

        public void MoveAllTo(Deck deck)
        {
            deck._cards.AddRange(_cards);
            _cards.Clear();
        }

        public IEnumerable<int> AllIndies()
        {
            var i = 0;
            while (i < _cards.Count)
            {
                if (_cards[i] != null) yield return i;
                i++;
            }
        }
    }
}