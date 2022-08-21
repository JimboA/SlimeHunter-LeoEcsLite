using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    public struct MoveProcess : IProcessData
    {
        public int2 CellPosition;
    }
}