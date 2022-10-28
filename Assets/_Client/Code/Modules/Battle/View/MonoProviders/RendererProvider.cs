using JimboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class RendererProvider : MonoProvider<MonoLink<SpriteRenderer>>
    {
        private void Awake()
        {
            if (value.Value == null)
                GetComponent<SpriteRenderer>();
        }
    }
}