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
                var hpPool = _damaged.Pools.Inc1;
                var damagePool = _damaged.Pools.Inc2;

                ref Health        hp     = ref hpPool.Get(entity);
                ref DamageRequest damage = ref damagePool.Get(entity);
                
                hp.Value -= damage.Value;
                if (hp.Value > 0)
                {
                    StartDamageProcess(entity, damage.Source);
                    continue;
                }
                
                hp.Value = 0;
                _killPool.Value.Add(entity).Source = damage.Source; 
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartDamageProcess(int entity, EcsPackedEntity source)
        {
            ref var damaged = ref _context.Value.StartNewProcess(_damagedPool.Value, entity);
            damaged.Source = source;
        }
    }
}