using System.Runtime.CompilerServices;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.View.UI
{
    public sealed class HealthWidgetSetupSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilterInject<Inc<Health, MonoLink<OnBoardHpWidget>, ViewCreatedEvent>> _characters = default;
        
        public void Init(IEcsSystems systems)
        {
            SetupWidgets(systems.GetWorld());
        }

        public void Run(IEcsSystems systems)
        {
            SetupWidgets(systems.GetWorld());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetupWidgets(EcsWorld world)
        {
            foreach (var entity in _characters.Value)
            {
                ref var pools = ref _characters.Pools;
                
                ref Health          hp     = ref pools.Inc1.Get(entity);
                ref OnBoardHpWidget widget = ref pools.Inc2.Get(entity).Value;
                
                widget.OnInit(hp.Value, world);
            }
        }
    }
}