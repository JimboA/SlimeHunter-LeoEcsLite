using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    [GenerateDataProvider]
    [System.Serializable]
    public struct SingleAttackViewData
    {
        public float Duration;
        public float HitTime;
        public EasingType EasingType;
    }
    
    public sealed class SingleAttackViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SingleAttackViewData, Started<SingleAttackProcess>, MonoLink<Transform>>,
            Exc<MoveProcess>> _attackers = default;
        
        private EcsPoolInject<DelayedAdd<AttackHitAnimationEvent>> _hitTimerPool = default;
        private EcsPoolInject<SingleAttackProcess> _attackPool = default;

        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _attackers.Value)
            {
                var world = systems.GetWorld();
                var pools = _attackers.Pools;
                var transformPool = _attackers.Pools.Inc3;

                ref SingleAttackViewData         singleAttackView = ref pools.Inc1.Get(entity);
                ref Started<SingleAttackProcess> processLink      = ref pools.Inc2.Get(entity);
                ref SingleAttackProcess          attack           = ref processLink.GetProcessData(_attackPool.Value);
                ref Transform                    transform        = ref transformPool.Get(entity).Value;

                if(!attack.Target.Unpack(world, out var targetEntity) 
                   || !transformPool.TryGet(targetEntity, out var targetTransform))
                    continue;

                transform.DoMove(systems.GetWorld(), transform.position, targetTransform.Value.position, singleAttackView.Duration)
                    .Easing(singleAttackView.EasingType)
                    .Loops(2, true);
                
                _battle.Value.SetDurationToProcess(processLink.ProcessEntity, singleAttackView.Duration);
                SetAttackHitTime(world, targetEntity, singleAttackView.HitTime);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetAttackHitTime(EcsWorld world, int entity, float time)
        {
            var delayedEntity = world.NewEntity();
            ref var hitTimer = ref _hitTimerPool.Value.Add(delayedEntity);
            hitTimer.TimeLeft = time;
            hitTimer.Target = world.PackEntity(entity);
        }
    }
}