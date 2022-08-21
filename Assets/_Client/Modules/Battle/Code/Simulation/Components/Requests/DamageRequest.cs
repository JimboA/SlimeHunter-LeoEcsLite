using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Unity.IL2CPP.CompilerServices;

namespace Client.Battle.Simulation
{
    public struct DamageRequest
    {
        public int Value;
        public EcsPackedEntity Source;
    }
}