using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class ElementsSetupSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ModelCreatedEvent, Element>> _elementals = default;
        private EcsCustomInject<RandomService> _random = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _elementals.Value)
            {
                ref Element element = ref _elementals.Pools.Inc2.Get(entity);
                if (element.InitInRuntime)
                {
                    // TODO: Temp. Will be replaced with setup from level asset
                    element.Type = (Elements)(1 << _random.Value.Random.Next(0, 5));
                }
            }
        }
    }
}