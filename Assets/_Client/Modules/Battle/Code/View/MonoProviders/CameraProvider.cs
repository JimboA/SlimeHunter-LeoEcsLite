using JimmboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(Camera))]
    public class CameraProvider : MonoProvider<MonoLink<Camera>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = GetComponent<Camera>();
        }
    }
}