using System;
using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    // for now we don't have characters animations, so the data in the view components may be the same.
    // But it will be different when the content is delivered.
    [Serializable]
    [GenerateDataProvider]
    public struct AttackWhileMovingViewData
    {
        public float Duration;
        public EasingType EasingType;
        public float HitTime;
    }
    
    public sealed class AttackWhileMovingViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<AttackWhileMovingViewData, Started<SingleAttackProcess>, Started<MoveProcess>,
            MonoLink<Transform>>> _attackers = default;
        
        private EcsPoolInject<DelayedAdd<AttackHitAnimationEvent>> _hitTimerPool = default;
        private EcsPoolInject<SingleAttackProcess> _attackPool = default;
        private EcsPoolInject<MoveProcess> _movePool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _context = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _attackers.Value)
            {
                var world = systems.GetWorld();
                ref var pools = ref _attackers.Pools;

                ref AttackWhileMovingViewData    movingAttackView  = ref pools.Inc1.Get(entity);
                ref Started<SingleAttackProcess> attackProcessLink = ref pools.Inc2.Get(entity);
                ref Started<MoveProcess>         moveProcessLink   = ref pools.Inc3.Get(entity);
                ref SingleAttackProcess          attack            = ref attackProcessLink.GetProcessData(_attackPool.Value);
                ref MoveProcess                  moved             = ref moveProcessLink.GetProcessData(_movePool.Value);
                ref Cell                         targetCell        = ref _board.Value.GetCellDataFromPosition(moved.CellPosition);
                ref Transform                    transform         = ref pools.Inc4.Get(entity).Value;
                

                transform.DoMove(world, transform.position, targetCell.WorldPosition, movingAttackView.Duration)
                    .Easing(movingAttackView.EasingType);

                if (attack.Target.Unpack(world, out var targetEntity))
                    SetAttackHitTime(world, targetEntity, movingAttackView.HitTime);

                _context.Value.SetDurationToProcess(attackProcessLink.ProcessEntity, movingAttackView.Duration);
                _context.Value.SetDurationToProcess(moveProcessLink.ProcessEntity, movingAttackView.Duration);
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