using System;
using System.Collections.Generic;
using System.Linq;

namespace JapaneseWhist
{
    public class Card
    {
        public CardColor Color { get; private set; }
        public CardValue Val { get; private set; }

        public Card(CardColor color, CardValue val)
        {
            Color = color;
            Val = val;
        }

        public bool HasHigherValueThan(Card card2)
        {
            return (int) Val > (int) card2.Val;
        }
    }
}