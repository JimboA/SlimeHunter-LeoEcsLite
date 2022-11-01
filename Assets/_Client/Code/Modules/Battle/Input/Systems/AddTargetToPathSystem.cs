using System.Runtime.CompilerServices;
using Client.Input;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class AddTargetToPathSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, InputReceiver, Path, Movable, Element, PathCursor>> _actors = default;
        private EcsFilterInject<Inc<AddTargetRequest, GridPosition>> _targets = default;
        
        private EcsPoolInject<Changed<Path>> _changedPool = default;
        private EcsPoolInject<Element> _elementPool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BoardMovementHelpers> _boardHelpers = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var targetEntity in _targets.Value)
            {
                foreach (var actorEntity in _actors.Value)
                {
                    ref var actorPools = ref _actors.Pools;
                    ref var targetPools = ref _targets.Pools;

                    ref Path         path          = ref actorPools.Inc3.Get(actorEntity);
                    ref Movable      movable       = ref actorPools.Inc4.Get(actorEntity);
                    ref GridPosition targetGridPos = ref targetPools.Inc2.Get(targetEntity);
                    ref GridPosition actorGridPos  = ref targetPools.Inc2.Get(actorEntity);
                    ref PathCursor   cursor        = ref actorPools.Inc6.Get(actorEntity);
                    Element          element       = actorPools.Inc5.Get(actorEntity); // yes I want a copy here

                    var (pathNotEmpty, lastPos) = TryGetLastEntityInPath(ref path, actorGridPos.Position, out var lastTargetEntity);
                    if (pathNotEmpty)
                    {
                        GetElementFromLastEntity(lastTargetEntity, out element);
                    }

                    int power = cursor.CurrentPower > 0 ? cursor.CurrentPower : 1;
                    var isMovable = _boardHelpers.Value.IsMovementPossible(targetGridPos.Position, in movable, actorEntity,
                        element.Type, power);
                    
                    if (_board.Value.IsReachable(lastPos, targetGridPos.Position, in movable)
                        && isMovable.isMovable)
                    {
                        path.Positions.Add(targetGridPos.Position);
                        _changedPool.Value.Add(actorEntity);
                    }
                    else
                    {
                        targetPools.Inc1.Del(targetEntity);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private (bool result, int2 position) TryGetLastEntityInPath(ref Path path, int2 actorPos, out int entity)
        {
            entity = -1;
            var positions = path.Positions;
            var length = positions.Length;
            var result = false;
            int2 lastPos;
            if (length > 0)
            {
                lastPos = path.Positions[length - 1];
                result = _board.Value.TryGetTarget(lastPos, out entity);
            }
            else
            {
                lastPos = actorPos;
            }

            return (result, lastPos);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void GetElementFromLastEntity(int lastTargetEntity, out Element element)
        {
            _elementPool.Value.TryGet(lastTargetEntity, out element);
        }
    }
}