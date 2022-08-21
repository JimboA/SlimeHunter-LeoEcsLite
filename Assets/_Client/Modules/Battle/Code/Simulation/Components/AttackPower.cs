using System.Runtime.CompilerServices;
using Client.AppData;
using Unity.IL2CPP.CompilerServices;
using UnityEngine.Serialization;


namespace Client.Battle.Simulation
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct AttackPower
    {
        public int BaseValue;
        public int CurrentValue;
    }
}