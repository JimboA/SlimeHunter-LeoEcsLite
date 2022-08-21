using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class MoveToCellExecuteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveToCellRequest, GridPosition, Turn>> _walkers = default;
        private EcsPoolInject<MoveProcess> _movedPool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _walkers.Value)
            {
                var world = systems.GetWorld();
                var pools = _walkers.Pools;

                ref MoveToCellRequest moveRequest = ref pools.Inc1.Get(entity);
                ref GridPosition      gridPos     = ref pools.Inc2.Get(entity);

                Move(world, entity, in moveRequest, in gridPos, _board.Value);
                StartMoveProcess(entity, moveRequest.EventData.TargetPosition);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Move(EcsWorld world, int entity, in MoveToCellRequest moveRequest, in GridPosition gridPos, IBoard board)
        {
            var data = moveRequest.EventData;
            ref var targetCell = ref board.GetCellDataFromPosition(data.TargetPosition);
            // swap
            if (moveRequest.WithSwap && targetCell.Target.Unpack(world, out var targetEntity))
            {
                StartMoveProcess(targetEntity, gridPos.Position);
                board.SwapTargets(gridPos.Position, data.TargetPosition);
            }
            else
            {
                board.ReleaseCell(gridPos.Position);
                board.SetEntityInCell(data.TargetPosition, entity);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartMoveProcess(int entity, int2 cellPosition)
        {
            ref var moved = ref _battle.Value.StartNewProcess(_movedPool.Value, entity);
            moved.CellPosition = cellPosition;
        }
    }
}