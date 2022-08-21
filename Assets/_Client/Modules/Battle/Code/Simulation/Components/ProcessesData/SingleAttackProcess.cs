using Leopotam.EcsLite;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    public struct SingleAttackProcess : IProcessData
    {
        public EcsPackedEntity Target;
    }
}