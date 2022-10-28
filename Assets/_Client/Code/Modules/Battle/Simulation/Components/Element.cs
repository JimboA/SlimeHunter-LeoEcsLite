using System;
using System.Runtime.CompilerServices;
using Client.AppData;
using UnityEngine.Serialization;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    [System.Flags]
    public enum Elements
    {
        None = 0,
        Fire = 1 << 0,
        Water = 1 << 1,
        Ice = 1 << 2,
        Electric = 1 << 3,
        Earth = 1 << 4,
        All = ~0
    }

    [System.Serializable]
    [GenerateDataProvider]
    public struct Element
    {
        public Elements Type;
        public bool InitInRuntime;
    }

    public static class ElementsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddElement(ref this Element component, Elements element)
        {
            component.Type |= element;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElement(in this Element component, Elements element)
        {
            return (component.Type & element) == element;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasElement(this Elements element, Elements other)
        {
            return (element & other) == other;
        }
    }
}