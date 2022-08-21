using System;
using JimmboA.Plugins.EcsProviders;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client.AppData
{
    public abstract class ComponentProvider<TComponent> : ComponentProviderBase where TComponent : struct
    {
        [SerializeField] public TComponent Value;
        public override void Convert(int entity, EcsWorld world)
        {
            world.GetPool<TComponent>().Add(entity) = Value;
        }

        public override Type GetComponentType()
        {
            return typeof(TComponent);
        }
    }
}