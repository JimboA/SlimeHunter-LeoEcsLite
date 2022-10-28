using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class ClearPathOnNewCycleSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NewBattleCycleEvent>> _onNewBattleCycle = "Events";
        private EcsFilterInject<Inc<Path>> _pathOwners = default;
        
        private EcsPoolInject<Changed<Path>> _changedPathPool = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var _ in _onNewBattleCycle.Value)
            {
                foreach (var entity in _pathOwners.Value)
                {
                    ClearPath(entity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ClearPath(int entity)
        {
            _pathOwners.Pools.Inc1.Get(entity).Positions.Clear();
            _changedPathPool.Value.Add(entity);
        }
    }
}