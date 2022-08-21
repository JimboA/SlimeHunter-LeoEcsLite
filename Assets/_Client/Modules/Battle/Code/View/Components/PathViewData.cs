using Client.AppData;
using Leopotam.EcsLite;
using JimmboA.Plugins;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct PathViewData : IEcsAutoReset<PathViewData>
    {
        [HideInInspector] public FastList<Vector3> Path;

        public void AutoReset(ref PathViewData c)
        {
            if (c.Path == null)
                c.Path = new FastList<Vector3>(50);
            else
                c.Path.Clear();
        }
    }
}