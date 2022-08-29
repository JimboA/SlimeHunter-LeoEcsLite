using System;
using JimboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteRendererProvider : MonoProvider<MonoLink<SpriteRenderer>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = GetComponent<SpriteRenderer>();
        }
    }
}