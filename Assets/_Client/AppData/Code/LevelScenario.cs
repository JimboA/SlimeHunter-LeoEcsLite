using System;
using System.Collections.Generic;
using Client.AppData.Blueprints;
using UnityEngine;

namespace Client.AppData
{
    // TODO: add level events
    [Serializable]
    public class LevelScenario
    {
        public AnimationCurve  Probability;
        public List<DropPiece> PiecesToDrop;
        [Space]
        public int MonstersActivationFrequency;
        public int MonstersActivationPerCycle;

        public float GetChance(float randomValue)
        {
            return Probability.Evaluate(randomValue);
        }
        

        [Serializable]
        public struct DropPiece
        {
            [Range(0,1)] public float MinRate;
            [Range(0,1)] public float MaxRate;
            
            public Blueprint Blueprint;
        }
    }
}