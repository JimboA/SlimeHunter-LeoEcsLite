using Client.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public struct CurrentPlaybackEvent
    {
        public int Current;
    }
    
    // for now, I'm saving only the player's input. The rest of the simulation is played again.
    // That's enough for now, if the simulation gets too complex, you can save the AI inputs too, so you don't have to recalculate them.
    // With this option, the playback logic will change a little 
    public sealed class PlayerPlaybackSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<InputReceiver>, Exc<CurrentPlaybackEvent>> _players = default;
        private EcsFilterInject<Inc<InputReceiver, Turn, CurrentPlaybackEvent>, Exc<HasActiveProcess>> _actingPlayers = default;

        private EcsPoolInject<CurrentPlaybackEvent> _currentEventPool = default;
        
        private EcsCustomInject<BattleService> _battle = default;
        private EcsCustomInject<IBoard> _board = default;

        public void Run(IEcsSystems systems)
        {
            var battle = _battle.Value;
            if(!battle.IsPlayback)
                return;

            foreach (var entity in _players.Value)
            {
                _currentEventPool.Value.Add(entity);
            }
            
            foreach (var entity in _actingPlayers.Value)
            {
                var state = battle.State;
                var pools = _actingPlayers.Pools;

                ref Turn                 turn         = ref pools.Inc2.Get(entity);
                ref CurrentPlaybackEvent currentEvent = ref pools.Inc3.Get(entity);
                
                if (!state.RestoreEvent(systems.GetWorld(), _board.Value, battle.TurnsCount, currentEvent.Current))
                {
                    turn.Phase = StatePhase.Complete;
                    currentEvent.Current = 0;
                    _currentEventPool.Value.Del(entity);
                    continue;
                }

                turn.Phase = StatePhase.Process;
                currentEvent.Current++;
            }
        }
    }
}