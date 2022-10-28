using Client.AppData;
using Leopotam.EcsLite;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct PathCursor
    {
        public int CurrentPower;
        public int CurrentPathIndex;
    }
}