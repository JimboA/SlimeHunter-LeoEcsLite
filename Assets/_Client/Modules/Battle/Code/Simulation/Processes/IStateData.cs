namespace Client.Battle.Simulation
{
    [System.Serializable]
    public enum StatePhase
    {
        OnStart,
        Process,
        Complete
    }
    
    public interface IStateData
    {
        public StatePhase Phase { get; set; }
    }
}