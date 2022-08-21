using System;
using UnityEngine;

namespace Client.Battle.View
{
    [Serializable]
    public struct Shake
    {
        public float Frequency;
        public float Duration;
        public Vector3 Pivot;
    }
}