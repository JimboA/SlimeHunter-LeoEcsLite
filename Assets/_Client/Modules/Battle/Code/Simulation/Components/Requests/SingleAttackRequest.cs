using System;
using Client.AppData;
using UnityEngine;

namespace Client.Battle.Simulation
{
    [Serializable]
    public struct SingleAttackRequest : IGameEvent
    {
        public GameEventData EventData { get; set; }
        public int Power;
    }
}