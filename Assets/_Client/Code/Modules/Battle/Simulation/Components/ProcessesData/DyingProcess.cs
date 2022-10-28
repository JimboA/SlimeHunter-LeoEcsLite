using Leopotam.EcsLite;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    public struct DyingProcess : IProcessData
    {
        public EcsPackedEntity Source;
    }
}