using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public struct PlaybackEndEvent
    {
    }
    
    public sealed class PlaybackEndSystem : IEcsRunSystem
    {
        private EcsPoolInject<PlaybackEndEvent> _playbackEndPool = default;
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            var battle = _battle.Value;
            if(!battle.IsPlayback)
                return;

            if (battle.TurnsCount == battle.State.TurnsCount)
            {
                battle.IsPlayback = false;
                battle.BlockInput = false;
                _playbackEndPool.Value.SendEvent();
            }
        }
    }
}