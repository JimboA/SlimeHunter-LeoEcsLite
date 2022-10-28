using Client.AppData;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct AttackRange
    {
        public int Range;
        public AreaType AreaType;
        public bool AllowMultiple;
    }
}