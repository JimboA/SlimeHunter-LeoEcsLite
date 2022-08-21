using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    // for now we don't have characters animations, so the data in the view components is the same.
    // But it will be different when the content is delivered.
    [System.Serializable]
    [GenerateDataProvider]
    public struct MoveViewData
    {
        public float Duration;
        public EasingType EasingType;
    }
    
    public sealed class MoveViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveViewData, Started<MoveProcess>, MonoLink<Transform>>,
            Exc<Started<SingleAttackProcess>>> _walkers = default;

        private EcsPoolInject<MoveProcess> _movePool = default;
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;
        

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _walkers.Value)
            {
                ref var pools = ref _walkers.Pools;
                
                ref MoveViewData         moveView    = ref pools.Inc1.Get(entity);
                ref Started<MoveProcess> processLink = ref pools.Inc2.Get(entity);
                ref MoveProcess          moving      = ref processLink.GetProcessData(_movePool.Value);
                ref Transform            transform   = ref pools.Inc3.Get(entity).Value;

                var targetPos = _board.Value.GetCellDataFromPosition(moving.CellPosition).WorldPosition;
                transform.DoMove(systems.GetWorld(), transform.position, targetPos, moveView.Duration)
                    .Easing(moveView.EasingType);

                _battle.Value.SetDurationToProcess(processLink.ProcessEntity, moveView.Duration);
            }
        }
    }
}