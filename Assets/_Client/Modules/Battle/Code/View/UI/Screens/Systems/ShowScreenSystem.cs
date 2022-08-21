using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.View.UI
{
    public sealed class ShowScreenSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ShowScreenRequest>> _onShowRequest = default;
        private EcsCustomInject<ScreensStorage> _screens = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _onShowRequest.Value)
            {
                var screens = _screens.Value;
                
                ref ShowScreenRequest showRequest = ref _onShowRequest.Pools.Inc1.Get(entity);
                if (!screens.ScreenByType.TryGetValue(showRequest.ScreenType, out var screen))
                    continue;

                var world = systems.GetWorld();
                var screenStack = screens.ActiveScreens;
                if (screenStack.Count > 0)
                {
                    var activeScreen = screenStack.Peek();
                    if(activeScreen != null)
                        activeScreen.Deactivate(world);
                }
                screenStack.Push(screen);
                screen.Show(world);
            }
        }
    }
}