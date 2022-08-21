using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.Simulation
{
    // TODO: split to different systems maybe
    public sealed class DelayedOperationsSystem<TFlag> : IEcsRunSystem where TFlag : struct
    {
        private EcsFilterInject<Inc<DelayedAdd<TFlag>>> _delayedAdd = default;
        private EcsFilterInject<Inc<DelayedRemove<TFlag>>> _delayedRemove = default;
        private EcsPoolInject<TFlag> _flagPool = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _delayedAdd.Value)
            {
                ref DelayedAdd<TFlag> delayed = ref _delayedAdd.Pools.Inc1.Get(entity);
                delayed.TimeLeft -= Time.deltaTime;
                if (delayed.TimeLeft <= 0 && delayed.Target.Unpack(systems.GetWorld(), out var targetEntity))
                {
                    _flagPool.Value.Add(targetEntity);
                    systems.GetWorld().DelEntity(entity);
                }
            }
            
            foreach (var entity in _delayedRemove.Value)
            {
                ref DelayedRemove<TFlag> delayed = ref _delayedRemove.Pools.Inc1.Get(entity);
                delayed.TimeLeft -= Time.deltaTime;
                if (delayed.TimeLeft <= 0 && delayed.Target.Unpack(systems.GetWorld(), out var targetEntity))
                {
                    _flagPool.Value.Del(targetEntity);
                    systems.GetWorld().DelEntity(entity);
                }
            }
        }
    }
}