using System.Collections.Generic;
using System.Diagnostics;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using Debug = UnityEngine.Debug;

namespace Client.Battle.Simulation 
{
    public sealed class BoardInitSystem : IEcsInitSystem
    {
        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleData> _battleData = default;

        public void Init(IEcsSystems systems)
        {
            _board.Value.Init();
            SetupFromBoardData(systems.GetWorld());
            FillBoard(systems.GetWorld());
        }

        private void FillBoard(EcsWorld world)
        {
            var battleData = _battleData.Value;
            var board = _board.Value;

            for (int i = 0; i < board.CellsAmount; i++)
            {
                ref var cell = ref board.GetCellDataFromIndex(i);
                if (cell.IsEmpty(world))
                {
                    if (battleData.TryGet(BattleIdents.Blueprints.Slime, out var blueprint))
                    {
                        var mobModel = blueprint.CreateModel(world);
                        board.SetEntityInCell(i, mobModel);
                    }
                }
            }
        }
        
        private void SetupFromBoardData(EcsWorld world)
        {
            var board = _board.Value;
            var battleData = _battleData.Value;
            var boardData = battleData.CurrentLevel.Board;
            
            CreateModelsForCellPieces(boardData.Cells, world, battleData, board);
            CreateModelsFromPieces(boardData.Characters, world, battleData, board);
            CreateModelsFromPieces(boardData.Items, world, battleData, board);
            CreateModelsFromPieces(boardData.Blocks, world, battleData, board);
        }

        private void CreateModelsForCellPieces(List<BoardPiece> pieces, EcsWorld world, BattleData battleData, IBoard board)
        {
            foreach (var piece in pieces)
            {
                if (!battleData.TryGet(piece.Name, out var blueprint))
                {
                    DebugNoBlueprint(piece);
                    continue;
                }
                
                var cellEntity = board[piece.Position.x, piece.Position.y];
                ref var cell = ref board.GetCellDataFromPosition(piece.Position);
                cell.WorldPosition = piece.WorldPosition;
                
                blueprint.SetModelFor(cellEntity, world);
            }
        }

        private void CreateModelsFromPieces(List<BoardPiece> pieces, EcsWorld world, BattleData battleData, IBoard board)
        {
            foreach (var piece in pieces)
            {
                if(!battleData.TryGet(piece.Name, out var blueprint))
                { 
                    DebugNoBlueprint(piece);
                    continue;
                }
                
                var model = blueprint.CreateModel(world);
                board.SetEntityInCell(piece.Position, model);
            }
        }

        #region Debug

        [Conditional("DEBUG")]
        private void DebugNoBlueprint(BoardPiece piece)
        {
            Debug.LogWarning($"can't find blueprint {piece.Name} in battleData asset. piece position: {piece.Position}");
        }

        #endregion
    }
}