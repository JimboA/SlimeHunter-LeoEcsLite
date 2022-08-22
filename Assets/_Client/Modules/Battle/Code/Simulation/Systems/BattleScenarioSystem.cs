using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    // TODO: in progress
    public sealed class BattleScenarioSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NewBattleCycleEvent>> _onNewBattleCycle = GlobalIdents.Worlds.EventWorldName;

        private EcsCustomInject<BattleData> _battleData;
        private EcsCustomInject<BattleService> _battle;
        private EcsCustomInject<IBoard> _board;
    
        public void Run(IEcsSystems systems)
        {
            foreach (var _ in _onNewBattleCycle.Value)
            {
                var battle = _battle.Value;
                var scenario = _battleData.Value.CurrentLevel.Scenario;
            }
        }
    }
}