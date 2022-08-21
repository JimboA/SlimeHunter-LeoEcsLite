using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation 
{
    public sealed class BoardInitSystem : IEcsInitSystem
    {
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleData> _battleData = default;
        private EcsCustomInject<RandomService> _random = default;
        
        public void Init(IEcsSystems systems)
        {
            _board.Value.Init();
            SetupPlayer(systems, new int2(3, 0));
            FillBoard(systems);
        }
        
        // TODO: temp. Will be changed to setup from level asset
        private int SetupPlayer(IEcsSystems systems, int2 pos)
        {
            if (_battleData.Value.TryGet(BattleIdents.Blueprints.Hero, out var blueprint))
            {
                var playerModel = blueprint.CreateModel(systems);
                _board.Value.SetEntityInCell(pos, playerModel);
                return playerModel;
            }

            return -1;
        }
        
        // TODO: temp. Will be changed to setup from level asset
        private void FillBoard(IEcsSystems systems)
        {
            var battleData = _battleData.Value;
            var board = _board.Value;
            var world = systems.GetWorld();
            var random = _random.Value.Random;

            for (int i = 0; i < 1; i++)
            {
                var index = random.Next(0, board.CellsAmount);
                ref var cell = ref board.GetCellDataFromIndex(index);
                if (cell.IsEmpty(world))
                {
                    if (battleData.TryGet(BattleIdents.Blueprints.Hunter, out var blueprint))
                    {
                        var mobModel = blueprint.CreateModel(systems);
                        board.SetEntityInCell(index, mobModel);
                    }
                }
                
            }
            
            for (int i = 0; i < board.CellsAmount; i++)
            {
                ref var cell = ref board.GetCellDataFromIndex(i);
                if (cell.IsEmpty(world))
                {
                    if (battleData.TryGet(BattleIdents.Blueprints.Slime, out var blueprint))
                    {
                        var mobModel = blueprint.CreateModel(systems);
                        board.SetEntityInCell(i, mobModel);
                    }
                }
            }
        }
    }
}