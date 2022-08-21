using System.Collections.Generic;

namespace JimmboA.Plugins
{
    public class FastComparable : IEqualityComparer<int>
    {
        public static readonly FastComparable Default = new FastComparable();

        public bool Equals(int x, int y)
        {
            return x == y;
        }

        public int GetHashCode(int obj)
        {
            return obj.GetHashCode();
        }
    }
}

