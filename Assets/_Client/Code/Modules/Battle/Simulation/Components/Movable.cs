using Client.AppData;

namespace Client.Battle.Simulation
{
    [System.Serializable]
    public enum StepType
    {
        Square,
        Cross,
        Diagonal
    }
    
    [System.Serializable]
    [GenerateDataProvider]
    public struct Movable
    {
        // TODO: change it to something more obvious
        public int Steps; // -1 for infinity
        public int StepLenght;
        public StepType StepType;
        public bool CanSwap;
        public bool CanAttackWhileMoving;
        public bool CanMoveOnlyToEqualElements;
        public Elements AllowedElements;
    }
}