using System.Runtime.CompilerServices;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.View
{
    public sealed class PathViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<BattleStateChangedEvent>> _onBattleStateChanged = "Events";
        private EcsFilterInject<Inc<Path, PathViewData, GridPosition, MonoLink<LineRenderer>>> _pathOwners = default;
        private EcsFilterInject<Inc<
            Turn, 
            Path, 
            Changed<Path>, 
            PathViewData, 
            GridPosition, 
            MonoLink<LineRenderer>>> _onPathChanged = default;

        private EcsCustomInject<IBoard> _board = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _onBattleStateChanged.Value)
            {
                BattlePhase state = _onBattleStateChanged.Pools.Inc1.Get(eventEntity).phase;
                if (state == BattlePhase.Battle)
                {
                    SetPathStartPositions();
                }
            }
            
            foreach (var entity in _onPathChanged.Value)
            {
                ref var pools = ref _onPathChanged.Pools;
                
                ref Path path     = ref pools.Inc2.Get(entity);
                ref LineRenderer  renderer = ref pools.Inc6.Get(entity).Value;

                var len = path.Positions.Length;
                if (len == 0)
                {
                    renderer.positionCount = 1;
                    continue;
                }

                var dif = len - (renderer.positionCount - 1);
                if (dif < 0)
                    renderer.positionCount = len + 1;
                else 
                    DrawPath(_board.Value, path.Positions, len, renderer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetPathStartPositions()
        {
            foreach (var entity in _pathOwners.Value)
            {
                var pools = _pathOwners.Pools;
                
                ref GridPosition gridPos  = ref pools.Inc3.Get(entity);
                ref LineRenderer renderer = ref pools.Inc4.Get(entity).Value;
                ref Cell         cell     = ref _board.Value.GetCellDataFromPosition(gridPos.Position);
                
                renderer.positionCount = 1;
                renderer.SetPosition(0, cell.WorldPosition);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DrawPath(IBoard board, FastList<int2> path, int startIndex, LineRenderer renderer)
        {
            var length = path.Length;
            renderer.positionCount = length + 1;

            for (int i = startIndex; i < renderer.positionCount; i++)
            {
                ref var cell = ref board.GetCellDataFromPosition(path[i - 1]);
                renderer.SetPosition(i, cell.WorldPosition);
            }
        }
    }
}