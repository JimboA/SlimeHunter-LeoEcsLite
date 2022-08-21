using Client.AppData;
using Client.Battle.Simulation;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct CollapseViewData
    {
        public float Duration;
        public EasingType EasingType;
    }
    
    public sealed class CollapseViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<CollapseViewData, Started<FallingProcess>,
            MonoLink<Transform>, CanFall>> _falling = default;

        private EcsPoolInject<FallingProcess> _fallingPool = default;
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _falling.Value)
            {
                ref var pools = ref _falling.Pools;
                
                ref CollapseViewData        collapseView  = ref pools.Inc1.Get(entity);
                ref Started<FallingProcess> processLink   = ref pools.Inc2.Get(entity);
                ref FallingProcess          falling       = ref processLink.GetProcessData(_fallingPool.Value);
                ref Cell                    cell          = ref _board.Value.GetCellDataFromPosition(falling.CellPosition);
                ref Transform               transform     = ref pools.Inc3.Get(entity).Value;

                transform
                    .DoMove(systems.GetWorld(), transform.position, cell.WorldPosition, collapseView.Duration)
                    .Easing(collapseView.EasingType);

                _battle.Value.SetDurationToProcess(processLink.ProcessEntity, collapseView.Duration);
            }
        }
    }
}