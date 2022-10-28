using System;
using System.Runtime.CompilerServices;
using Client.AppData;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;


namespace Client.Battle.Simulation
{
    public struct Cell
    {
        public int2 Position;
        public EcsPackedEntity Target;
        public Vector3 WorldPosition;
    }

    public static class CellExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsEmpty(in this Cell cell, EcsWorld world)
        {
            return !cell.Target.Unpack(world, out _);
        }
    }

}