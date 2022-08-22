using System.Runtime.CompilerServices;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class SpawnPiecesSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<BattleStateChangedEvent>> _onStateChanged = GlobalIdents.Worlds.EventWorldName;
        private EcsFilterInject<Inc<Cell>> _cells = default;
        
        private EcsPoolInject<FallingProcess> _fallingPool = default;
        
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default; 
        private EcsCustomInject<BattleData> _battleData = default;
        private EcsCustomInject<RandomService> _random = default;

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

        private void SpawnNewPieces(IEcsSystems systems)
        {
            var random = _random.Value.Random;
            var board = _board.Value;
            var scenario = _battleData.Value.CurrentLevel.Scenario;
            var piecesToDrop = scenario.PiecesToDrop;
            
            foreach (var cellEntity in _cells.Value)
            {
                ref Cell cell = ref _cells.Pools.Inc1.Get(cellEntity);
                if(!cell.IsEmpty(systems.GetWorld()))
                    continue;
                
                var chance = scenario.GetChance((float)random.NextDouble());
                foreach (var piece in piecesToDrop)
                {
                    if(chance < piece.MinRate || chance > piece.MaxRate)
                        continue;
                    
                    var modelEntity = piece.Blueprint.CreateModel(systems);
                    board.SetEntityInCell(cell.Position, modelEntity);
                    StartFallingProcess(modelEntity, cell.Position);
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