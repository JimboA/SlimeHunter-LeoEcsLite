using Client.AppData;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct ViewLinkComponent
    {
        public GameObject Prefab;
    }
}