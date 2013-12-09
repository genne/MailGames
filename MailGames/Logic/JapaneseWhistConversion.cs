using JapaneseWhist;
using MailGames.Context;

namespace MailGames.Logic
{
    internal static class JapaneseWhistConversion
    {
        public static JapaneseWhistState GetState(JapaneseWhistBoard japaneseWhistBoard)
        {
            var state = new JapaneseWhistState(japaneseWhistBoard.Seed);
            foreach (var move in japaneseWhistBoard.Moves)
            {
                JapaneseWhistLogic.Select(state, move.PlayerDeck, move.CardIndex);
            }
            return state;
        }
    }
}