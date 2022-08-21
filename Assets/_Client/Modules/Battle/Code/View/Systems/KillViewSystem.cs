using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.Battle.View
{
    public struct KillViewRequest
    {
        public bool pooled;
    }
    
    public sealed class KillViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<KillViewRequest, MonoLink<Transform>>> _killed = default;
        private EcsCustomInject<PoolContainer> _viewObjectPool = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _killed.Value)
            {
                ref var pools = ref _killed.Pools;
                
                ref KillViewRequest killRequest = ref pools.Inc1.Get(entity);
                ref Transform       transform   = ref pools.Inc2.Get(entity).Value;

                if (killRequest.pooled)
                    _viewObjectPool.Value.Recycle(transform.gameObject);
                else 
                    UnityEngine.Object.Destroy(transform.gameObject);
            }
        }
    }
}