namespace JapaneseWhist
{
    public class PlayerDeckIndex
    {
        public PlayerDeckIndex(PlayerDeck playerDeck, int index)
        {
            PlayerDeck = playerDeck;
            Index = index;
        }

        public PlayerDeck PlayerDeck { get; set; }
        public int Index { get; set; }
    }
}