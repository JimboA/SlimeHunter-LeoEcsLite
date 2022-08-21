using System.Runtime.CompilerServices;
using Client.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation 
{
    public sealed class CheckTargetAlreadyHasSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, Path, InputReceiver>> _actors = default;
        private EcsFilterInject<Inc<AddTargetRequest, GridPosition>> _targets = default;
        private EcsPoolInject<Changed<Path>> _changedPathPool = default;

        public void Run (IEcsSystems systems) 
        {
            foreach (var targetEntity in _targets.Value)
            {
                var actorsPools = _actors.Pools;
                var targetsPools = _targets.Pools;
                
                foreach (var actorEntity in _actors.Value)
                {
                    ref Path         path          = ref actorsPools.Inc2.Get(actorEntity);
                    ref GridPosition targetGridPos = ref targetsPools.Inc2.Get(targetEntity);
                    
                    if (targetEntity == actorEntity)
                    {
                        ClearPathAt(actorEntity, targetEntity, 0, ref path);
                        continue;
                    }
                    var index = path.Positions.IndexOf(ref targetGridPos.Position);
                    if (index != -1)
                    {
                        ClearPathAt(actorEntity, targetEntity, index + 1, ref path);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearPathAt(int actor, int target, int index, ref Path path)
        {
            _targets.Pools.Inc1.Del(target);
            path.Positions.ClearAt(index);
            _changedPathPool.Value.Add(actor);
        }
    }
}