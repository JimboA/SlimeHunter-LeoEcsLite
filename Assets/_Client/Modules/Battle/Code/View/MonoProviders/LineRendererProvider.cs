using JimmboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineRendererProvider : MonoProvider<MonoLink<LineRenderer>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = GetComponent<LineRenderer>();
        }
    }
}
