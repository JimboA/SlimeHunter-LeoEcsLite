using System;
using JimmboA.Plugins.EcsProviders;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client.AppData
{
    public abstract class ComponentProviderBase : ScriptableObject, IConvertToEntity
    {
        public abstract void Convert(int entity, EcsWorld world);
        public abstract Type GetComponentType();
    }
}