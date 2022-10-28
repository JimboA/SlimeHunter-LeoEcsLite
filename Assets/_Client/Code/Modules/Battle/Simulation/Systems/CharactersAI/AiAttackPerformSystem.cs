using System.Runtime.CompilerServices;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class AiAttackPerformSystem : IEcsRunSystem 
    {
        private EcsFilterInject<Inc< 
            Turn, 
            Monster, 
            AttackRange, 
            GridPosition,
            AttackPower>, 
            Exc<HasActiveProcess>> _attackingMobs = default;
        
        private EcsFilterInject<Inc<Player, GridPosition>> _players = default;
        private EcsPoolInject<SingleAttackRequest> _attackPool = default;
        
        private EcsCustomInject<IBoard> _board = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _attackingMobs.Value)
            {
                ref var pools = ref _attackingMobs.Pools;

                ref Turn turn = ref pools.Inc1.Get(entity);
                if(turn.Phase != StatePhase.OnStart)
                    continue;
                
                ref AttackRange  range   = ref pools.Inc3.Get(entity);
                ref GridPosition gridPos = ref pools.Inc4.Get(entity);
                ref AttackPower  power   = ref pools.Inc5.Get(entity);
                
                foreach (var playerEntity in _players.Value)
                {
                    ref GridPosition playerGridPos = ref _players.Pools.Inc2.Get(playerEntity);
                    if(!TryFindPlayer(_board.Value, gridPos.Position, playerGridPos.Position, ref range)) 
                        continue;

                    Attack(entity, gridPos.Position, playerGridPos.Position, power.CurrentValue);
                    turn.Phase = StatePhase.Complete;
                    break;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryFindPlayer(IBoard board, int2 center, int2 playerPosition, ref AttackRange range)
        {
            if (range.AreaType == AreaType.Cross)
                return board.CheckNearCross(center, playerPosition, range.Range);
            
            if (range.AreaType == AreaType.Square)
                return board.CheckNearSquare(center, playerPosition, range.Range);

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Attack(int attacker, int2 position, int2 targetPosition, int power)
        {
            var eventPool = _attackPool.Value;
            ref var attackRequest = ref eventPool.RaiseGameEvent(attacker, new GameEventData(position, targetPosition, GameEvents.SingleAttack));
            attackRequest.Power = power;
        }
    }
}