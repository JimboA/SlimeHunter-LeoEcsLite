using System.Runtime.CompilerServices;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class SpawnSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<BattleStateChangedEvent>> _onStateChanged = GlobalIdents.Worlds.EventWorldName;
        private EcsFilterInject<Inc<Cell>> _cells = default;
        
        private EcsPoolInject<FallingProcess> _fallingPool = default;
        
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default; 
        private EcsCustomInject<BattleData> _battleData = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _onStateChanged.Value)
            {
                ref var stateChanged = ref _onStateChanged.Pools.Inc1.Get(eventEntity);
                if (stateChanged.phase == BattlePhase.Collapse)
                {
                    SpawnNewPieces(systems);
                }
            }
        }

        // TODO: temp. will be replaced with spawn from level asset
        private void SpawnNewPieces(IEcsSystems systems)
        { 
            if(!_battleData.Value.TryGet(BattleIdents.Blueprints.Slime, out var blueprint))
                return;
            
            var board = _board.Value;
            foreach (var cellEntity in _cells.Value)
            {
                ref Cell cell = ref _cells.Pools.Inc1.Get(cellEntity);
                if(!cell.IsEmpty(systems.GetWorld()))
                    continue;
                
                var modelEntity = blueprint.CreateModel(systems);
                board.SetEntityInCell(cell.Position, modelEntity);
                StartFallingProcess(modelEntity, cell.Position);
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