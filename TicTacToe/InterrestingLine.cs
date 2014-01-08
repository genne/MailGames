using GameBase;

namespace TicTacToe
{
    public class InterrestingLine
    {
        public InterrestingLine()
        {
                
        }
        public InterrestingLine(InterrestingLine copy)
        {
            Count = copy.Count;
            IsDead = copy.IsDead;
            CurrentPlayer = copy.CurrentPlayer;
        }

        public void Add(GamePlayer currentPlayer)
        {
            if (IsDead) return;
            if (!CurrentPlayer.HasValue)
            {
                CurrentPlayer = currentPlayer;
                Count = 1;
            }
            else
            {
                if (CurrentPlayer.Value == currentPlayer)
                    Count++;
                else
                {
                    IsDead = true;
                    CurrentPlayer = null;
                    Count = 0;
                }
            }
        }

        public int Count { get; set; }
        public bool IsDead { get; set; }
        public GamePlayer? CurrentPlayer { get; set; }
    }
}