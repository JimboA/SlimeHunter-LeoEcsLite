using System;
using System.Runtime.CompilerServices;
using Client.AppData;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public sealed class AiFollowByPlayerSystem : IEcsRunSystem, IEcsDestroySystem
    {
        private EcsFilterInject<Inc<
            IsFollowByPlayer, 
            Turn,
            Movable, 
            GridPosition>,
            Exc<HasActiveProcess>> _followers = default;
        
        private EcsFilterInject<Inc<Player, GridPosition>> _players = default;

        private EcsPoolInject<Path> _pathPool = default;
        private EcsPoolInject<MoveToCellRequest> _moveRequestPool = default;
        private EcsPoolInject<Element> _elementPool = default;
        private EcsPoolInject<AttackPower> _powerPool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<GridPathFinding> _pathFinding = default;
        private EcsCustomInject<BoardMovementHelpers> _boardHelpers = default;

        private FastList<int> _pathResultCache = new FastList<int>(64);
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _followers.Value)
            {
                if(!_players.Value.TryGetFirst(out var playerEntity))
                    break;

                var board = _board.Value;
                ref var pools = ref _followers.Pools;
                
                ref Turn turn  = ref pools.Inc2.Get(entity);
                if(turn.Phase != StatePhase.OnStart)
                    continue;
                
                ref Movable movable = ref pools.Inc3.Get(entity);
                if(movable.Steps == 0)
                    continue;
                
                ref GridPosition followerGridPos = ref pools.Inc4.Get(entity);
                ref GridPosition playerGridPos   = ref pools.Inc4.Get(playerEntity);

                _pathResultCache.Clear();
                _pathFinding.Value.FindPath(
                    followerGridPos.Position, 
                    playerGridPos.Position,
                    new int2(board.Columns, board.Rows),
                    GridPathfindingHelpers.GetStepOffsets(movable.StepType, movable.StepLenght),
                    GetWalkableCells(board, entity, in movable, playerGridPos.Position),
                    _pathResultCache);

                if (_pathResultCache.Length <= 1)
                {
                    turn.Phase = StatePhase.Complete;
                    continue;
                }
                
                if (movable.Steps > 1)
                {
                    AddPath(board, entity, ref movable);
                    turn.Phase = StatePhase.Process;
                }
                else
                {
                    AddMoveRequest(board, followerGridPos.Position, entity);
                    turn.Phase = StatePhase.Complete;
                }
            }
        }

        // TODO: make "IsMovable" method burst - ready and pass it to pathFinding via the function pointer.
        private NativeArray<bool> GetWalkableCells(IBoard board, int walkerEntity, in Movable movable, int2 targetPosition)
        {
            var walkableArray = new NativeArray<bool>(board.CellsAmount, Allocator.TempJob);
            _elementPool.Value.TryGet(walkerEntity, out var element);
            int currentPower = 0;
            if (_powerPool.Value.TryGet(walkerEntity, out var power))
            {
                currentPower = power.CurrentValue;
            }

            var boardHelpers = _boardHelpers.Value; 
            for (int i = 0; i < board.CellsAmount; i++)
            {
                walkableArray[i] = false;
                ref var targetCell = ref board.GetCellDataFromIndex(i);
                var isMovable = boardHelpers.IsMovementPossible(in targetCell, in movable, walkerEntity, element.Type, currentPower);
                walkableArray[i] = isMovable.isMovable;
            }

            var targetIndex = targetPosition.x + targetPosition.y * board.Columns;
            walkableArray[targetIndex] = true;
            
            return walkableArray;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddPath(IBoard board, int entity, ref Movable movable)
        {
            ref var path = ref _pathPool.Value.GetOrAdd(entity);
            var len = _pathResultCache.Length - movable.Steps;
            for (int i = _pathResultCache.Length - 1; i >= len; i--)
            {
                ref var cell = ref board.GetCellDataFromIndex(_pathResultCache[i]);
                path.Positions.Add(new int2(cell.Position));
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AddMoveRequest(IBoard board, int2 currentPos, int entity)
        {
            var indexNearTarget = _pathResultCache.Length - 2;
            ref var cell = ref board.GetCellDataFromIndex(_pathResultCache[indexNearTarget]);
            _moveRequestPool.Value.RaiseGameEvent(entity,
                new GameEventData(currentPos, cell.Position, GameEvents.Move));
        }

        public void Destroy(IEcsSystems systems)
        {
            _pathResultCache = null;
        }
    }
}