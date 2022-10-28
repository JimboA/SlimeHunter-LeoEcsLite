using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    public struct FallingProcess : IProcessData
    {
        public int2 CellPosition;
    }
}