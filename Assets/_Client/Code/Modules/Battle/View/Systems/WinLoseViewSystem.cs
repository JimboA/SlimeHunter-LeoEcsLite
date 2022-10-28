using Client.Battle.Simulation;
using Client.Battle.View.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.View 
{
    public sealed class WinLoseViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<BattleStateChangedEvent>> _onStateChanged = "Events";
        private EcsFilterInject<Inc<Player, WinLoseEvent>> _players = default;
        
        private EcsPoolInject<ShowScreenRequest> _showScreenPool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _onStateChanged.Value)
            {
                ref BattleStateChangedEvent stateChanged = ref _onStateChanged.Pools.Inc1.Get(eventEntity);
                if (stateChanged.phase == BattlePhase.WinLose)
                {
                    foreach (var entity in _players.Value)
                    {
                        ref WinLoseEvent winLose = ref _players.Pools.Inc2.Get(entity);
                        if(winLose.IsWin)
                            _showScreenPool.Value.ShowScreen<WinScreen>();
                        else 
                            _showScreenPool.Value.ShowScreen<LoseScreen>();
                    }
                }
            }
        }
    }
}