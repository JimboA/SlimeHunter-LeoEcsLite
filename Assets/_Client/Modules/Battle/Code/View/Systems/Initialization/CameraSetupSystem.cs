using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View
{
    public class CameraSetupSystem : IEcsInitSystem
    {
        private EcsCustomInject<BattleSceneData> _sceneData = default;
        private EcsCustomInject<IBoard> _board = default;

        public void Init(IEcsSystems systems)
        {
            SetupBoardCamera();
        }

        // TODO: make padding settings for UI 
        private void SetupBoardCamera()
        {
            var camera = _sceneData.Value.BattleCameraProvider.GetComponent<Camera>();
            var transform = camera.transform;
            var (center, size) = CalculateOrthoSize(camera, GetBoardBounds(1.5f, _board.Value), transform.position.z);
            transform.position = center;
            camera.orthographicSize = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (Vector3 center, float size) CalculateOrthoSize(Camera camera, Bounds bounds, float zPos)
        {
            var vertical = bounds.size.y;
            var horizontal = bounds.size.x * camera.pixelHeight / camera.pixelWidth;
            var size = Mathf.Max(horizontal, vertical) * 0.5f;
            var center = bounds.center + new Vector3(0, 0, zPos);
            return (center, size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Bounds GetBoardBounds(float padding, IBoard board)
        {
            var bounds = new Bounds();
            var len = board.CellsAmount;
            for (int i = 0; i < len; i++)
            {
                var cell = board.GetCellDataFromIndex(i);
                bounds.Encapsulate(cell.WorldPosition);
            }
            bounds.Expand(padding);
            return bounds;
        }
    }
}