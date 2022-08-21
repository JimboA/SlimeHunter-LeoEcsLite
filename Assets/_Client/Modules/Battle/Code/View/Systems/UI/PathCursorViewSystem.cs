using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct PathCursorViewData
    {
        public GameObject CursorPrefab;
        [HideInInspector] public GameObject CursorObj;
        public float AnimationTime;
        public float ScaleSize;
    }
    
    public sealed class PathCursorViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<BattleStateChangedEvent>> _onBattleStateChanged = "Events";
        private EcsFilterInject<Inc<Path, PathCursor, PathCursorViewData, GridPosition,
            MonoLink<Transform>, ViewCreatedEvent>> _createdCursorOwners = default;
        private EcsFilterInject<Inc<
            Turn, 
            Path, 
            Changed<Path>, 
            PathCursor,
            PathCursorViewData,
            GridPosition>> _onPathChanged = default;

        private EcsCustomInject<IBoard> _board = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _createdCursorOwners.Value)
            {
                ref var pools = ref _createdCursorOwners.Pools;

                ref Path               path       = ref pools.Inc1.Get(entity);
                ref PathCursor         cursor     = ref pools.Inc2.Get(entity);
                ref PathCursorViewData cursorView = ref pools.Inc3.Get(entity);
                ref Transform          transform  = ref pools.Inc5.Get(entity).Value;

                if (cursorView.CursorObj == null)
                {
                    cursorView.CursorObj = GameObject.Instantiate(cursorView.CursorPrefab, transform.position, transform.rotation);
                }

                if(cursor.CurrentPathIndex < 0)
                    continue;

                SetupCursor(in cursor, in cursorView, in path);
            }
            
            foreach (var entity in _onPathChanged.Value)
            {
                ref var pools = ref _onPathChanged.Pools;

                ref Turn               turn       = ref pools.Inc1.Get(entity);
                ref Path               path       = ref pools.Inc2.Get(entity);
                ref PathCursor         cursor     = ref pools.Inc4.Get(entity);
                ref PathCursorViewData cursorView = ref pools.Inc5.Get(entity);

                if (turn.Phase == StatePhase.OnStart)
                {
                    if (cursor.CurrentPathIndex < 0 && cursorView.CursorObj.activeSelf)
                    {
                        cursorView.CursorObj.SetActive(false);
                        continue;
                    }

                    SetCursorToPathLastPosition(in cursor, in cursorView, in path, systems.GetWorld());
                }
                else
                {
                    cursorView.CursorObj.SetActive(false);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetupCursor(in PathCursor cursor, in PathCursorViewData cursorView, in Path path)
        {
            ref Cell   lastCell = ref _board.Value.GetCellDataFromPosition(path.Positions[cursor.CurrentPathIndex]);
            cursorView.CursorObj.transform.position = lastCell.WorldPosition;
            cursorView.CursorObj.SetActive(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCursorToPathLastPosition(in PathCursor cursor, in PathCursorViewData cursorView, in Path path, EcsWorld world)
        {
            if (cursor.CurrentPathIndex >= 0)
            {
                if (!cursorView.CursorObj.activeSelf)
                {
                    cursorView.CursorObj.SetActive(true);
                    var tr = cursorView.CursorObj.transform;
                    tr.DoScale(world, Vector3.one, Vector3.one * cursorView.ScaleSize, cursorView.AnimationTime)
                        .Loops(int.MaxValue, true);
                }
                    
                ref var lastCell = ref _board.Value.GetCellDataFromPosition(path.Positions[cursor.CurrentPathIndex]);
                cursorView.CursorObj.transform.position = lastCell.WorldPosition;
            }
        }
    }
}