using UnityEngine;

namespace Client.AppData
{
    public static class GlobalIdents
    {
        public static class Worlds
        {
            public const string EventWorldName = "Events";
            public const string SimulationGroupName = "SimulationSystems";
            public const string ViewGroupName = "ViewSystems";
        }
        
        public static class AppData
        {
            public static readonly string SavePath = $"{Application.persistentDataPath}/Saves";
        }
    }
}