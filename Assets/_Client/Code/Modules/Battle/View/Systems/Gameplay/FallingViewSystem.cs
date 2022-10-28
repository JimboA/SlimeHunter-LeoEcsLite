using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    // public sealed class FallingViewSystem : IEcsRunSystem 
    // {
    //     private EcsFilterInject<Inc<CanFallTag, FallingComponent, GridPositionComponent, MonoLinkComponent<Transform>>> _falling;
    //     private EcsPoolInject<CellComponent> _cellsPool;
    //     private EcsSharedInject<SharedData> _shared;
    //     
    //     public void Run(EcsSystems systems)
    //     {
    //         foreach (var entity in _falling.Value)
    //         {
    //             var world = systems.GetWorld();
    //             var cellsPool = _cellsPool.Value;
    //             var records = _shared.Value.RecordsContainer;
    //             var fallingTime = _shared.Value.BattleData.fallingTime;
    //             var cellRefPool = _falling.Pools.Inc3;
    //             var transformPool = _falling.Pools.Inc4;
    //             var packedCellEntity = cellRefPool.Get(entity).Cell;
    //             if(!packedCellEntity.Unpack(world, out var cellEntity))
    //                 continue;
    //                     
    //             ref var cell = ref cellsPool.Get(cellEntity);
    //             var transform = transformPool.Get(entity).Value;
    //                     
    //             transform.DoMove(records.CommandBuffer, out var command, world, cell.WorldPosition, fallingTime);
    //             records.Add(command, 0);
    //             //Debug.LogWarning($"collapse entity: {entity}, frame: {Time.frameCount}");
    //         }
    //     }
    // }
}