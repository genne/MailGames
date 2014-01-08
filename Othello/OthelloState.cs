using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameBase;

namespace Othello
{
    public class OthelloState
    {
        private readonly Dictionary<Position, GamePlayer> _cells = new Dictionary<Position, GamePlayer>();

        public OthelloState(bool empty = false)
        {
            if (!empty)
            {
                _cells[new Position(3, 3)] = GamePlayer.FirstPlayer;
                _cells[new Position(4, 4)] = GamePlayer.FirstPlayer;
                _cells[new Position(3, 4)] = GamePlayer.SecondPlayer;
                _cells[new Position(4, 3)] = GamePlayer.SecondPlayer;
            }
        }

        public OthelloState(OthelloState state)
        {
            _cells = new Dictionary<Position, GamePlayer>(state._cells);
            CurrentPlayer = state.CurrentPlayer;
        }

        public GamePlayer CurrentPlayer { get; set; }

        public GamePlayer? Get(Position position)
        {
            GamePlayer player;
            if (_cells.TryGetValue(position, out player))
            {
                return player;
            }
            return null;
        }

        public void Set(Position pos)
        {
            _cells[pos] = CurrentPlayer;
        }
    }
}
