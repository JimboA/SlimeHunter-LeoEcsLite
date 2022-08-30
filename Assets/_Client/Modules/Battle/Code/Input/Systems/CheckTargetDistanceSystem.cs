using System;
using System.Runtime.CompilerServices;
using Client.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public sealed class CheckTargetDistanceSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, Path, GridPosition, Movable, InputReceiver>> _actors;
        private EcsFilterInject<Inc<SelectedEvent, GridPosition>> _targets;
        private EcsCustomInject<IBoard> _board;
        
        public void Run (IEcsSystems systems) 
        {
            foreach (var targetEntity in _targets.Value)
            {
                foreach (var actorEntity in _actors.Value)
                {
                    ref var actorsPools = ref _actors.Pools;
                    var addTargetPool = _targets.Pools.Inc1;
                    ref var path = ref actorsPools.Inc2.Get(actorEntity);
                    ref var targetPos = ref actorsPools.Inc3.Get(targetEntity);
                    ref var actorPos = ref actorsPools.Inc3.Get(actorEntity);
                    ref var movable = ref actorsPools.Inc4.Get(actorEntity);

                    if (!CheckNear(ref path, actorPos.Position, targetPos.Position, movable.StepType))
                        addTargetPool.Del(targetEntity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckNear(ref Path path, int2 actorPos, int2 targetPos, StepType stepType)
        {
            var length = path.Positions.Length;
            var lastCellPos = length == 0
                ? actorPos
                : path.Positions[length - 1];

            return stepType switch
            {
                StepType.Square => _board.Value.CheckNearSquare(targetPos, lastCellPos, 1),
                StepType.Cross => _board.Value.CheckNearCross(targetPos, lastCellPos, 1),
                StepType.Diagonal => _board.Value.CheckNearDiagonal(targetPos, lastCellPos, 1),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}