using JimboA.Plugins.EcsProviders;
using UnityEngine;
using UnityEngine.Experimental.U2D.Animation;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(SpriteResolver))]
    public class SpriteResolverProvider : MonoProvider<MonoLink<SpriteResolver>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = GetComponent<SpriteResolver>();
        }
    }
}