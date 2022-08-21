using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View
{
    public sealed class ShakeSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Shake, MonoLink<Transform>>> _shakers = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _shakers.Value)
            {
                ref var pools = ref _shakers.Pools;

                ref Shake     shake     = ref pools.Inc1.Get(entity);
                ref Transform transform = ref pools.Inc2.Get(entity).Value;
                
                if(transform == null)
                    continue;
                
                if (shake.Duration <= 0)
                {
                    transform.position = shake.Pivot;
                    pools.Inc1.Del(entity); 
                    continue;
                }

                transform.position = shake.Pivot + Random.insideUnitSphere * shake.Frequency;
                shake.Duration -= Time.deltaTime;
            }
        }
    }
}