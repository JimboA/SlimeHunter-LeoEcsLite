using System.Runtime.CompilerServices;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class CollapseSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<BattleStateChangedEvent>> _onStateChanged = GlobalIdents.Worlds.EventWorldName;
        
        private EcsPoolInject<CanFall> _canFallPool = default;
        private EcsPoolInject<FallingProcess> _fallingPool = default;
        
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _onStateChanged.Value)
            {
                ref var stateChanged = ref _onStateChanged.Pools.Inc1.Get(eventEntity);
                if (stateChanged.phase == BattlePhase.Collapse)
                {
                    Collapse(systems.GetWorld());
                    _battle.Value.NextPhase = BattlePhase.Battle;
                }
            }
        }

        private void Collapse(EcsWorld world)
        {
            var board = _board.Value;
            var canFallPool = _canFallPool.Value;

            for (int column = 0; column < board.Columns; column++)
            {
                for (int row = 0; row < board.Rows; row++)
                {
                    var targetPos = new int2(column, row);
                    ref Cell cell = ref board.GetCellDataFromPosition(targetPos);
                    if(!cell.IsEmpty(world))
                        continue;

                    for (int emptyPlace = row + 1; emptyPlace < board.Rows; emptyPlace++)
                    {
                        var cellPos = new int2(column, emptyPlace);
                        ref var occupiedCell = ref board.GetCellDataFromPosition(cellPos);
                        if(!occupiedCell.Target.Unpack(world, out var targetEntity) || !canFallPool.Has(targetEntity))
                            continue;
                        
                        board.SetEntityInCell(targetPos, targetEntity);
                        board.ReleaseCell(cellPos);
                        StartFallingProcess(targetEntity, targetPos);
                        break;
                    }
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartFallingProcess(int entity, int2 cellPosition)
        {
            ref var falling = ref _battle.Value.StartNewProcess(_fallingPool.Value, entity);
            falling.CellPosition = cellPosition;
        }
    }
}