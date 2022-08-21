using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using JimmboA.Plugins;
using JimmboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.AppData
{
    public interface IModelFactory
    {
        public int CreateModel(IEcsSystems systems);
    }
    
    public interface IViewFactory
    {
        public int CreateView(IEcsSystems systems, int model, Vector3 position, PoolContainer pool);
    }
}