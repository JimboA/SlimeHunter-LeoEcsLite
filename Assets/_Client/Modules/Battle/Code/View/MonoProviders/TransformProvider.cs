using JimboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    public class TransformProvider : MonoProvider<MonoLink<Transform>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = transform;
        }
    }
}