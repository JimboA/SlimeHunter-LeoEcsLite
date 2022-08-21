using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class ResetPowerSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, AttackPower>, Exc<HasActiveProcess>> _actors = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _actors.Value)
            {
                ref var pools = ref _actors.Pools;

                ref Turn        turn  = ref pools.Inc1.Get(entity);
                ref AttackPower power = ref pools.Inc2.Get(entity);

                if (turn.Phase == StatePhase.OnStart
                    || turn.Phase == StatePhase.Complete)
                {
                    power.CurrentValue = power.BaseValue;
                }
            }
        }
    }
}