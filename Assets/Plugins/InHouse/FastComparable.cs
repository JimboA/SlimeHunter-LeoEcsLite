using System.Collections.Generic;

namespace JimboA.Plugins
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

