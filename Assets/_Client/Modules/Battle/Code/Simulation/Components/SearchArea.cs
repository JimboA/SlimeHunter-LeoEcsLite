using Client.AppData;

namespace Client.Battle.Simulation
{
    //Temp. Will be changed to setup area from character editor (in progress)
    [System.Serializable]
    public enum AreaType
    {
        Square,
        Cross,
        FullBoard
    }
    
    [System.Serializable]
    [GenerateDataProvider]
    public struct SearchArea
    {
        public int Radius;
        public AreaType AreaType;
        //public GameType TargetType;
        public bool AllowMultiple;
    }
}