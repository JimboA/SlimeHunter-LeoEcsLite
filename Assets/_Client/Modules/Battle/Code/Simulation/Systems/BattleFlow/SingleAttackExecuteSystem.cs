using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation 
{
    public sealed class SingleAttackExecuteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<SingleAttackRequest, Turn>> _attackers = default;
        private EcsPoolInject<DamageRequest> _damagePool = default;
        private EcsPoolInject<SingleAttackProcess> _attackPool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;
            
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _attackers.Value)
            {
                var world = systems.GetWorld();
                var singleAttackPool = _attackers.Pools.Inc1;

                ref SingleAttackRequest attackRequest = ref singleAttackPool.Get(entity);
                ref Cell                targetCell    = ref _board.Value.GetCellDataFromPosition(attackRequest.EventData.TargetPosition);
                
                Attack(world, entity, in targetCell, in attackRequest);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Attack(EcsWorld world, int attackerEntity, in Cell targetCell, in SingleAttackRequest request)
        {
            if (targetCell.Target.Unpack(world, out var targetEntity))
            {
                ref var damage = ref _damagePool.Value.Add(targetEntity);
                damage.Source = world.PackEntity(attackerEntity);
                damage.Value = request.Power;
                StartAttackProcess(attackerEntity, targetCell.Target);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartAttackProcess(int entity, EcsPackedEntity target)
        {
            ref var attack = ref _battle.Value.StartNewProcess(_attackPool.Value, entity);
            attack.Target = target;
        }
    }
}