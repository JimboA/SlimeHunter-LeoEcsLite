using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public sealed class DamageExecuteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Health, DamageRequest>> _damaged = default;
        
        private EcsPoolInject<KillRequest> _killPool = default;
        private EcsPoolInject<DamageProcess> _damagedPool = default;
        
        private EcsCustomInject<BattleService> _context = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _damaged.Value)
            {
                ref var pools = ref _damaged.Pools;

                ref Health        hp     = ref pools.Inc1.Get(entity);
                ref DamageRequest damage = ref pools.Inc2.Get(entity);
                
                hp.Value -= damage.Value;
                if (hp.Value > 0)
                {
                    StartDamageProcess(entity, damage.Source);
                    continue;
                }
                
                hp.Value = 0;
                AddKillRequest(entity, in damage);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartDamageProcess(int entity, EcsPackedEntity source)
        {
            ref var damaged = ref _context.Value.StartNewProcess(_damagedPool.Value, entity);
            damaged.Source = source;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddKillRequest(int entity, in DamageRequest damage)
        {
            _killPool.Value.Add(entity).Source = damage.Source; 
        }
    }
}