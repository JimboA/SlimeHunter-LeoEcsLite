using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class PathComboApplySystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, AttackPower, PathComboAttackModifier, MoveToCellRequest>> _attackers = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _attackers.Value)
            {
                ref var pools = ref _attackers.Pools;
                
                ref Turn        turn  = ref pools.Inc1.Get(entity);
                ref AttackPower power = ref pools.Inc2.Get(entity);

                if(turn.Phase == StatePhase.Process)
                    power.CurrentValue++;
            }
        }
    }
}