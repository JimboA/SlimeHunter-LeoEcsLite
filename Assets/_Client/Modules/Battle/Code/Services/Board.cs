using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Client.AppData;
using Client.AppData.Blueprints;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using JimboA.Plugins;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.Simulation
{
    // TODO: Pretty ugly interface... It might be better to replace it with an abstract class (or just final type) and move all public API to extensions.
    public interface IBoard
    {
        public int CellsAmount { get; }
        public int Rows { get; }
        public int Columns { get; }
        public EcsWorld World { get; }
        public void Init();
        public int this[int index] { get; }
        public int this[int column, int row] { get; }
        public void SetEntityInCell(int index, int entity);
        public void SetEntityInCell(int2 pos, int entity);
        public ref Cell GetCellDataFromIndex(int index);
        public ref Cell GetCellDataFromPosition(int2 pos);
        public void ReleaseCell(int2 pos);
        public void ReleaseCell(int index);
        public void SwapTargets(int2 pos1, int2 pos2);
        public void SwapCells(int2 pos1, int2 pos2);
        public bool TryGetTarget(int2 pos, out int targetEntity);
        public bool CheckBounds(int2 pos);
    }
    
    public class Board : IBoard
    {
        private int[] _cells;
        private int _columns;
        private int _rows;
        private int _zPos;
        private int _cellsAmount;
        private EcsWorld _world;
        private EcsPool<Cell> _cellsPool;
        private EcsPool<GridPosition> _gridPosPool;

        public int CellsAmount => _cellsAmount;
        public int Rows => _rows;
        public int Columns => _columns;
        public EcsWorld World => _world;

        public int this[int column, int row]
        {
            get => _cells[row * _columns + column];
            private set => _cells[row * _columns + column] = value;
        }

        public int this[int index] => _cells[index];

        public Board(EcsWorld world, BoardData data)
        {
            _world = world;
            _columns = data.Columns;
            _rows = data.Rows;
            _zPos = data.ZOffset;
            _cellsPool = world.GetPool<Cell>();
            _gridPosPool = world.GetPool<GridPosition>();
            _cellsAmount = _rows * _columns;
            _cells = new int[_cellsAmount];
        }

        public void Init()
        {
            for (int row = 0; row < _rows; row++)
            {
                for (int column = 0; column < _columns; column++)
                {
                    var entity = _world.NewEntity();
                    ref var cell = ref _cellsPool.Add(entity);
                    cell.Position = new int2(column, row);
                    ref var gridPos = ref _gridPosPool.Add(entity);
                    gridPos.Position = cell.Position;
                    this[column, row] = entity;
                }
            }
        }

        public void SetEntityInCell(int index, int entity)
        {
            var cell = _cells[index];
            SetEntity(cell, entity);
        }

        public void SetEntityInCell(int2 pos, int entity)
        {
            var cell = this[pos.x, pos.y];
            SetEntity(cell, entity);
        }

        public ref Cell GetCellDataFromIndex(int index)
        {
            return ref _cellsPool.Get(_cells[index]);
        }

        public ref Cell GetCellDataFromPosition(int2 pos)
        {
            return ref _cellsPool.Get(this[pos.x, pos.y]);
        }

        public void ReleaseCell(int2 pos)
        {
            _cellsPool.Get(this[pos.x, pos.y]).Target = new EcsPackedEntity();
        }

        public void ReleaseCell(int index)
        {
            _cellsPool.Get(_cells[index]).Target = new EcsPackedEntity();
        }

        public void SwapTargets(int2 pos1, int2 pos2)
        {
            ref var cell1 = ref _cellsPool.Get(this[pos1.x, pos1.y]);
            ref var cell2 = ref _cellsPool.Get(this[pos2.x, pos2.y]);

            if (cell1.Target.Unpack(_world, out var entity1))
                _gridPosPool.Get(entity1).Position = pos2; 
            
            if (cell2.Target.Unpack(_world, out var entity2))
                _gridPosPool.Get(entity2).Position = pos1;

            var temp = cell1.Target;
            cell1.Target = cell2.Target;
            cell2.Target = temp;
        }

        public void SwapCells(int2 pos1, int2 pos2)
        {
            var temp = this[pos1.x, pos1.y];
            this[pos1.x, pos1.y] = this[pos2.x, pos2.y];
            this[pos2.x, pos2.y] = temp;
        }

        public bool TryGetTarget(int2 pos, out int targetEntity)
        {
            return _cellsPool.Get(this[pos.x, pos.y]).Target.Unpack(_world, out targetEntity);
        }

        public bool CheckBounds(int2 pos)
        {
            return !(pos.x < 0 || pos.x > _columns - 1 || pos.y < 0 || pos.y > _rows - 1);
        }

        private void SetEntity(int cellEntity, int entity)
        {
            ref var cell = ref _cellsPool.Get(cellEntity);
            cell.Target = _world.PackEntity(entity);
            ref var gridPos = ref _gridPosPool.GetOrAdd(entity);
            gridPos.Position = cell.Position;
        }
    }
    
    public delegate bool EntityPredicate(EcsWorld world, int entity);

    // TODO: this was done to avoid closure when using searching helpers. Seems dump, working on it :)
    public delegate bool EntityEqualsPredicate(EcsWorld world, int entity1, int entity2);

    public static class BoardExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReachable(this IBoard board, int2 from, int2 to, in Movable movable)
        {
            ref var cellFrom = ref board.GetCellDataFromPosition(from);
            ref var cellTo = ref board.GetCellDataFromPosition(to);

            return movable.StepType switch
            {
                StepType.Square => CheckNearSquare(in cellFrom, in cellTo, movable.StepLenght),
                StepType.Cross => CheckNearCross(in cellFrom, in cellTo, movable.StepLenght),
                StepType.Diagonal => CheckNearDiagonal(in cellFrom, in cellTo, movable.StepLenght),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReachableFrom(in this Cell cellTo, in Cell cellFrom, in Movable movable)
        {
            return movable.StepType switch
            {
                StepType.Square => CheckNearSquare(in cellFrom, in cellTo, movable.StepLenght),
                StepType.Cross => CheckNearCross(in cellFrom, in cellTo, movable.StepLenght),
                StepType.Diagonal => CheckNearDiagonal(in cellFrom, in cellTo, movable.StepLenght),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #region Check Near

        public static bool CheckNearSquare(in Cell cell1, in Cell cell2, int radius)
        {
            return Mathf.Abs(cell1.Position.y - cell2.Position.y) <= radius
                   && Mathf.Abs(cell1.Position.x - cell2.Position.x) <= radius;
        }
        
        public static bool CheckNearCross(in Cell cell1, in Cell cell2, int radius)
        {
            return (cell1.Position.x == cell2.Position.x || cell1.Position.y  == cell2.Position.y )
                   && Mathf.Abs(cell1.Position.y - cell2.Position.y ) <= radius
                   && Mathf.Abs(cell1.Position.x - cell2.Position.x) <= radius;
        }
        
        public static bool CheckNearDiagonal(in Cell cell1, in Cell cell2, int radius)
        {
            return (cell1.Position.x != cell2.Position.x && cell1.Position.y != cell2.Position.y)
                   && Mathf.Abs(cell1.Position.y  - cell2.Position.y ) <= radius
                   && Mathf.Abs(cell1.Position.x - cell2.Position.x) <= radius;
        }

        public static bool CheckNearSquare(this IBoard board, int2 pos1, int2 pos2, int radius)
        {
            return CheckNearSquare(in board.GetCellDataFromPosition(pos1), in board.GetCellDataFromPosition(pos2),
                radius);
        }

        public static bool CheckNearCross(this IBoard board, int2 pos1, int2 pos2, int radius)
        {
            return CheckNearCross(in board.GetCellDataFromPosition(pos1), in board.GetCellDataFromPosition(pos2),
                radius);
        }
        
        public static bool CheckNearDiagonal(this IBoard board, int2 pos1, int2 pos2, int radius)
        {
            return CheckNearDiagonal(in board.GetCellDataFromPosition(pos1), in board.GetCellDataFromPosition(pos2),
                radius);
        }

        #endregion

        #region Find target templates

        public static void FindTargetFull(this IBoard board, int2 center, FastList<int2> founded, EntityPredicate check,
            bool allowMultiple = false)
        {
            var world = board.World;
            var len = board.CellsAmount;
            for (int i = 0; i < len; i++)
            {
                ref var cell = ref board.GetCellDataFromIndex(i);

                if (cell.Target.Unpack(world, out var targetEntity) && check(world, targetEntity))
                {
                    founded.Add(cell.Position);
                    if (!allowMultiple)
                        break;
                }
            }
        }

        public static void FindTargetFull(this IBoard board, int2 center, FastList<int2> founded, EntityEqualsPredicate check,
            int outerEntity, bool allowMultiple = false)
        {
            var world = board.World;
            var len = board.CellsAmount;
            for (int i = 0; i < len; i++)
            {
                ref var cell = ref board.GetCellDataFromIndex(i);

                if (cell.Target.Unpack(world, out var targetEntity) && check(world, targetEntity, outerEntity))
                {
                    founded.Add(cell.Position);
                    if (!allowMultiple)
                        break;
                }
            }
        }

        public static void FindTargetSquare(this IBoard board, int2 center, int radius, FastList<int2> founded, EntityPredicate check,
            bool allowMultiple = false)
        {
            ref var cell = ref board.GetCellDataFromPosition(center);
            for (int row = cell.Position.y - radius; row <= cell.Position.y + radius; row++)
            {
                if (row < 0 || row > board.Rows - 1)
                    break;

                for (int column = cell.Position.x - radius; column <= cell.Position.x + radius; column++)
                {
                    if (column < 0 || column > board.Columns - 1)
                        break;

                    ref var targetCell = ref board.GetCellDataFromPosition(new int2(column, row));
                    if (targetCell.Target.Unpack(board.World, out var targetEntity) 
                        && check(board.World, targetEntity))
                    {
                        founded.Add(targetCell.Position);
                        if (!allowMultiple)
                            break;
                    }
                }
            }
        }

        public static void FindTargetSquare(this IBoard board, int2 center, int radius, FastList<int2> founded,
            EntityEqualsPredicate check, int outerEntity, bool allowMultiple = false)
        {
            ref var cell = ref board.GetCellDataFromPosition(center);
            for (int row = cell.Position.y - radius; row <= cell.Position.y + radius; row++)
            {
                if (row < 0 || row > board.Rows - 1)
                    break;

                for (int column = cell.Position.x - radius; column <= cell.Position.x + radius; column++)
                {
                    if (column < 0 || column > board.Columns - 1)
                        break;

                    ref var targetCell = ref board.GetCellDataFromPosition(new int2(column, row));
                    if (targetCell.Target.Unpack(board.World, out var targetEntity) 
                        && check(board.World, targetEntity, outerEntity))
                    {
                        founded.Add(targetCell.Position);
                        if (!allowMultiple)
                            break;
                    }
                }
            }
        }

        public static void FindTargetCross(this IBoard board, int2 center, int radius, FastList<int2> founded, EntityPredicate check,
            bool allowMultiple = false)
        {
            ref var cell = ref board.GetCellDataFromPosition(center);
            for (int row = cell.Position.y - radius; row <= cell.Position.y + radius; row++)
            {
                if (row < 0 || row > board.Rows - 1)
                    break;

                ref var targetCell = ref board.GetCellDataFromPosition(new int2(cell.Position.x, row));
                if (targetCell.Target.Unpack(board.World, out var targetEntity) 
                    && check(board.World, targetEntity))
                {
                    founded.Add(targetCell.Position);
                    if (!allowMultiple)
                        break;
                }
            }

            if (!allowMultiple && founded.Length > 0)
                return;

            for (int column = cell.Position.x - radius; column <= cell.Position.x + radius; column++)
            {
                if (column < 0 || column > board.Columns - 1)
                    break;

                ref var targetCell = ref board.GetCellDataFromPosition(new int2(column, cell.Position.y));
                if (targetCell.Target.Unpack(board.World, out var targetEntity) 
                    && check(board.World, targetEntity))
                {
                    founded.Add(targetCell.Position);
                    if (!allowMultiple)
                        break;
                }
            }
        }

        public static void FindTargetCross(this IBoard board, int2 center, int radius, FastList<int2> founded,
            EntityEqualsPredicate check, int outerEntity, bool allowMultiple = false)
        {
            ref var cell = ref board.GetCellDataFromPosition(center);
            for (int row = cell.Position.y - radius; row <= cell.Position.y + radius; row++)
            {
                if (row < 0 || row > board.Rows - 1)
                    break;

                ref var targetCell = ref board.GetCellDataFromPosition(new int2(cell.Position.x, row));
                if (targetCell.Target.Unpack(board.World, out var targetEntity) 
                    && check(board.World, targetEntity, outerEntity))
                {
                    founded.Add(targetCell.Position);
                    if (!allowMultiple)
                        break;
                }
            }

            if (!allowMultiple && founded.Length > 0)
                return;

            for (int column = cell.Position.x - radius; column <= cell.Position.x + radius; column++)
            {
                if (column < 0 || column > board.Columns - 1)
                    break;
                
                ref var targetCell = ref board.GetCellDataFromPosition(new int2(column, cell.Position.y));
                if (targetCell.Target.Unpack(board.World, out var targetEntity) 
                    && check(board.World, targetEntity, outerEntity))
                {
                    founded.Add(targetCell.Position);
                    if (!allowMultiple)
                        break;
                }
            }
        }

        #endregion
    }
}