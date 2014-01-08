using System.Collections.Generic;
using GameBase;

namespace TicTacToe
{
    internal class EqualsComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}