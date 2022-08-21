using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class MovableSettingsSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Movable, ModelCreatedEvent>> _movables = default;
        private EcsPoolInject<Path> _pathPool = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _movables.Value)
            {
                ref Movable movable = ref _movables.Pools.Inc1.Get(entity);
                if (movable.Steps > 1 || movable.Steps < 0)
                    _pathPool.Value.Add(entity);
            }
        }
    }
}