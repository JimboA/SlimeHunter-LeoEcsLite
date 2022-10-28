using System;
using Client.AppData;
using UnityEngine;

namespace Client.Battle.Simulation 
{
    [Serializable]
    public struct MoveToCellRequest : IGameEvent
    {
        public GameEventData EventData { get; set; }
        public bool WithSwap;
    }
}