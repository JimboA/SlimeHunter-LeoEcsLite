using System.Runtime.CompilerServices;
using Client.Battle.Simulation;
using Client.Battle.View.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.View 
{
    public sealed class ScoreViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Score, Changed<Score>, MonoLink<KillScoreWidget>>> _scoreHolders = default;
        private EcsPoolInject<UpdateWidgetRequest<KillScoreWidget, int>> _widgetUpdatePool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _scoreHolders.Value)
            {
                ref Score score = ref _scoreHolders.Pools.Inc1.Get(entity);
                UpdateScoreWidget(entity, score.Value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateScoreWidget(int entity, int value)
        {
            _widgetUpdatePool.Value.Add(entity).Value = value;
        }
    }
}