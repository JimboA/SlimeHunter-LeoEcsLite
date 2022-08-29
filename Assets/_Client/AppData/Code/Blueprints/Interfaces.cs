using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using JimboA.Plugins;
using JimboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.AppData
{
    public interface IModelFactory
    {
        public int CreateModel(EcsWorld world);
        public void SetModelFor(int entity, EcsWorld world);
    }
    
    public interface IViewFactory
    {
        public int CreateView(EcsWorld world, int model, Vector3 position, PoolContainer pool);
    }
}