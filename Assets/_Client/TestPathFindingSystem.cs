using System.Diagnostics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins;
using JimmboA.Plugins.FrameworkExtensions;
using Unity.Collections;
using Debug = UnityEngine.Debug;

namespace Client.Battle.Simulation
{
    public sealed class TestPathFindingSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, Path, GridPosition>> _actors;
        private EcsFilterInject<Inc<AddTargetRequest, GridPosition>> _targets;
        private EcsPoolInject<Cell> _cellsPool;
        private EcsPoolInject<Monster> _mobPool;
        private EcsPoolInject<Health> _hpPool;
        private EcsCustomInject<GridPathFinding> _pathFinding;
        private FastList<int> _resultCache = new FastList<int>(32);

        public void Run(IEcsSystems systems)
        {
            // foreach (var targetEntity in _targets.Value)
            // {
            //     foreach (var actor in _actors.Value)
            //     {
            //         float startTime = Time.realtimeSinceStartup;
            //
            //         var world = systems.GetWorld();
            //         ref var targetCellRef = ref _targets.Pools.Inc2.Get(targetEntity);
            //         ref var actorCellRef = ref _actors.Pools.Inc3.Get(actor);
            //         if (!targetCellRef.Cell.Unpack(world, out var targetCellEntity)
            //             || !actorCellRef.Cell.Unpack(world, out var actorCellEntity))
            //             continue;
            //
            //         var board = _shared.Value.Board;
            //         ref var targetCell = ref _cellsPool.Value.Get(targetCellEntity);
            //         ref var actorCell = ref _cellsPool.Value.Get(actorCellEntity);
            //
            //         var stopwatch = new Stopwatch();
            //         stopwatch.Start();
            //         var steps = new NativeArray<int2>(8, Allocator.TempJob)
            //         {
            //             [0] = new int2(-1, 0),
            //             [1] = new int2(+1, 0),
            //             [2] = new int2(0, +1),
            //             [3] = new int2(0, -1),
            //             [4] = new int2(-1, -1),
            //             [5] = new int2(-1, +1),
            //             [6] = new int2(+1, -1),
            //             [7] = new int2(+1, +1)
            //         };
            //         _pathFinding.Value.FindPath(new int2(actorCell.Column, actorCell.Row),
            //             new int2(targetCell.Column, targetCell.Row),
            //             new int2(board.Columns, board.Rows),
            //             steps,
            //             GetWalkableCells(world, board),
            //             _resultCache);
            //         stopwatch.Stop();
            //
            //         //Debug.LogWarning("Time: " + ((Time.realtimeSinceStartup - startTime) * 1000f));
            //         Debug.LogWarning("Time: " + stopwatch.Elapsed.TotalMilliseconds);
            //         if (_resultCache.Length == 0)
            //         {
            //             Debug.LogWarning($"no path founded");
            //         }
            //         else
            //         {
            //             for (int i = 0; i < _resultCache.Length; i++)
            //             {
            //                 var cellIndex = _resultCache[i];
            //                 var cellEntity = board[cellIndex];
            //                 ref var cell = ref _cellsPool.Value.Get(cellEntity);
            //                 Debug.LogWarning($"path Point:{i} is cell at row: {cell.Row}, column: {cell.Column}");
            //                 //CodeMonkey.CMDebug.Text();
            //             }
            //
            //             _resultCache.Clear();
            //         }
            //     }
            // }
        }

        private NativeArray<bool> GetWalkableCells(EcsWorld world, Board board)
        {
            var walkableArray = new NativeArray<bool>(board.CellsAmount, Allocator.TempJob);
            for (int i = 0; i < board.CellsAmount; i++)
            {
                var walkableCellEntity = board[i];
                walkableArray[i] = false;
                ref var walkableCell = ref _cellsPool.Value.Get(walkableCellEntity);
                if (walkableCell.Target.Unpack(world, out var targetEntity)
                    && _mobPool.Value.TryGet(targetEntity, out _)
                    && _hpPool.Value.TryGet(targetEntity, out var hp)
                    && hp.Value == 1)
                    walkableArray[i] = true;
            }

            return walkableArray;
        }
    }
}