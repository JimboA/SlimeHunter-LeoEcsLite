using System;
using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using JimboA.Plugins;
using JimboA.Plugins.FrameworkExtensions;
using JimboA.Plugins.Tween;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct AttackRangeViewData
    {
        public Color Color;
        public float FadeDuration;
    }
    
    public class AttackRangeViewSystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        private EcsFilterInject<Inc<Turn, Path>> _actingPathWalkers = default;
        private EcsFilterInject<Inc<SelectedEvent, GridPosition, AttackRange, AttackRangeViewData, CanAct>> _selected = default;
        private EcsFilterInject<Inc<Cell, ViewLink, MonoLink<SpriteRenderer>>> _cellViews = default;

        private EcsCustomInject<IBoard> _board;

        private FastList<int2> _cellsCache;

        public void Init(IEcsSystems systems)
        {
            _cellsCache = new FastList<int2>(_board.Value.CellsAmount);
        }

        public void Run(IEcsSystems systems)
        {
            foreach (var actorEntity in _actingPathWalkers.Value)
            {
                var world = systems.GetWorld();
                var board = _board.Value;
                ref Path path = ref _actingPathWalkers.Pools.Inc2.Get(actorEntity);
                
                foreach (var targetEntity in _selected.Value)
                {
                    ref var pools = ref _selected.Pools;

                    ref GridPosition        gridPos  = ref pools.Inc2.Get(targetEntity);
                    ref AttackRange         range    = ref pools.Inc3.Get(targetEntity);
                    ref AttackRangeViewData viewData = ref pools.Inc4.Get(targetEntity);

                    if(path.Positions.IndexOf(ref gridPos.Position) != -1)
                        continue;
                    
                    SetHighlights(world, board, in range, in gridPos, ref viewData);
                }
            }
        }

        private void SetHighlights(EcsWorld world, IBoard board, in AttackRange range, in GridPosition gridPos, ref AttackRangeViewData viewData)
        {
            _cellsCache.Clear();

            switch (range.AreaType)
            {
                case AreaType.Square:
                    board.GetSquare(gridPos.Position, range.Range, _cellsCache);
                    break;
                case AreaType.Cross:
                    board.GetCross(gridPos.Position, range.Range, _cellsCache);
                    break;
                case AreaType.FullBoard:
                    for (int i = 0; i < board.CellsAmount; i++)
                    {
                        EnableHighlight(world, board[i], ref viewData);
                    }
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (_cellsCache.Length > 0)
            {
                foreach (var pos in _cellsCache)
                {
                    EnableHighlight(world, board[pos.x, pos.y], ref viewData);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnableHighlight(EcsWorld world, int cellEntity, ref AttackRangeViewData viewData)
        {
            if (_cellViews.Value.Contains(cellEntity))
            {
                ref var pools = ref _cellViews.Pools;
                ref SpriteRenderer renderer = ref pools.Inc3.Get(cellEntity).Value;

                renderer.DoColor(world, Color.white, viewData.Color, viewData.FadeDuration)
                    .Loops(4, true);
            }
        }
        
        public void Destroy(IEcsSystems systems)
        {
            _cellsCache = null;
        }
    }
}