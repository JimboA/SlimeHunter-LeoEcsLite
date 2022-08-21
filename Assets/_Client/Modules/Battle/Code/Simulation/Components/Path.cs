using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Input;
using Leopotam.EcsLite;
using JimmboA.Plugins;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.Simulation 
{
    public struct Path : IEcsAutoReset<Path>
    {
        public FastList<int2> Positions;
        public int Current;
        
        public void AutoReset(ref Path c)
        {
            if (c.Positions == null)
                c.Positions = new FastList<int2>(64);
            else
                c.Positions.Clear();

            c.Current = 0;
        }
    }

    public static class PathExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetLastEntityInPath(in this Path path, IBoard board, out int entity)
        {
            entity = -1;
            return path.Positions.TryGetLast(out var lastPosition) &&
                   board.TryGetTarget(lastPosition, out entity);
        }
    }
}