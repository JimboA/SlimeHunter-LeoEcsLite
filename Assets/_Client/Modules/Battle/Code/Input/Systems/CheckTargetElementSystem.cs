using System.Runtime.CompilerServices;
using Client.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins;
using Unity.Mathematics;

namespace Client.Battle.Simulation 
{
    public sealed class CheckTargetElementSystem : IEcsRunSystem 
    {
        private EcsFilterInject<Inc<Turn, Path, Element, InputReceiver>> _actors;
        private EcsFilterInject<Inc<AddTargetRequest, Element, GridPosition>> _targets;
        private EcsCustomInject<IBoard> _board;

        public void Run(IEcsSystems systems)
        {
            foreach (var targetEntity in _targets.Value)
            {
                foreach (var actorEntity in _actors.Value)
                {
                    ref var actorsPools = ref _actors.Pools;
                    ref var targetPools = ref _targets.Pools;
                    var addTargetPool = targetPools.Inc1;
                    var elementPool = targetPools.Inc2;

                    var path = actorsPools.Inc2.Get(actorEntity).Positions;
                    if (path.Length == 0)
                        continue;

                    if (TryGetLastEntityInPath(path, out var lastTargetEntity))
                    {
                        ref var lastElement = ref elementPool.Get(lastTargetEntity);
                        ref var targetElement = ref elementPool.Get(targetEntity);
                        if(lastElement.HasElement(targetElement.Type))
                            continue;

                        addTargetPool.Del(targetEntity);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetLastEntityInPath(FastList<int2> path, out int entity)
        {
            entity = -1;
            return path.TryGetLast(out var lastPosition) &&
                   _board.Value.TryGetTarget(lastPosition, out entity);
        }
    }
}