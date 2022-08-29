using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins.ObjectPool;

namespace Client.Battle.View
{
    public sealed class SpawnBoardPiecesViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ModelCreatedEvent, BlueprintLink, GridPosition>> _models = default;
        
        private EcsPoolInject<Started<FallingProcess>> _fallingPool = default;
        
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<PoolContainer> _viewsPool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _models.Value)
            {
                ref var pools = ref _models.Pools;
                ref var blueprint = ref pools.Inc2.Get(entity).Blueprint;
                
                ref GridPosition gridPos = ref pools.Inc3.Get(entity);
                ref Cell         cell    = ref _board.Value.GetCellDataFromPosition(gridPos.Position);

                var pos = cell.WorldPosition;
                if (_fallingPool.Value.Has(entity))
                    pos.y += _board.Value.Rows;
                
                blueprint.CreateView(systems.GetWorld(), entity, pos, _viewsPool.Value);
            }
        }
    }
}