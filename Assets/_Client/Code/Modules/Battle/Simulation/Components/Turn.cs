namespace Client.Battle.Simulation 
{
    public struct Turn : IStateData
    {
        public StatePhase Phase { get; set; }
    }
}