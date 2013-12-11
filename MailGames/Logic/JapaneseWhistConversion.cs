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
                if (move.Trumf.HasValue)
                {
                    JapaneseWhistLogic.SelectTrumf(state, move.Trumf.Value);
                }
                else
                {
                    JapaneseWhistLogic.Select(state, move.PlayerDeck, move.CardIndex);
                }
            }
            return state;
        }
    }
}