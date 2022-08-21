using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class ActivateMonstersSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<NewBattleCycleEvent>> _onNewBattleCycle = GlobalIdents.Worlds.EventWorldName;
        private EcsFilterInject<Inc<Monster, Initiative>, Exc<CanAct>> _mobs = default;
        
        private EcsPoolInject<CanAct> _canActPool = default;
        private EcsPoolInject<ActivateProcess> _activatePool = default;
        
        private EcsCustomInject<BattleData> _battleData = default;
        private EcsCustomInject<BattleService> _battle = default;
        private EcsCustomInject<RandomService> _random = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var _ in _onNewBattleCycle.Value)
            {
                if(_battle.Value.CyclesCount % _battleData.Value.CurrentLevel.MonstersActivationFrequency == 0) 
                    RandomActivate();
            }
        }

        // TODO: replace with actual balanced rule (ha-ha.. balanced...).
        private void RandomActivate()
        {
            var battle = _battle.Value;
            var canActPool = _canActPool.Value;
            var mobs = _mobs.Value;
            var dense = mobs.GetRawEntities();
            var count = _battleData.Value.CurrentLevel.MonstersActivationPerCycle;
            var random = _random.Value.Random;

            for (int i = 0; i < count; i++)
            {
                var index = random.Next(0, mobs.GetEntitiesCount());
                var entity = dense[index];
                canActPool.Add(entity);
                battle.StartNewProcess(_activatePool.Value, entity);
            }
        }
    }
}