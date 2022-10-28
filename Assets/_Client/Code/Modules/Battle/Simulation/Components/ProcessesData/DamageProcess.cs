using Leopotam.EcsLite;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    public struct DamageProcess : IProcessData
    {
        public EcsPackedEntity Source;
    }
}