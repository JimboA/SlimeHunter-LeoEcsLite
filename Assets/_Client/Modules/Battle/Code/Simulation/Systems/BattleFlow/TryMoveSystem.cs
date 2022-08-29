using System.Runtime.CompilerServices;
using JimboA.Plugins.FrameworkExtensions;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class TryMoveSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<MoveToCellRequest, Movable>, Exc<SingleAttackRequest>> _walkers = default;
        
        private EcsPoolInject<Element> _elementPool = default;
        private EcsPoolInject<AttackPower> _powerPool = default;
        private EcsPoolInject<SingleAttackRequest> _attackRequestPool = default;
        
        private EcsCustomInject<BoardMovementHelpers> _boardHelpers = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _walkers.Value)
            {
                var requestPool = _walkers.Pools.Inc1;
                
                ref MoveToCellRequest moveRequest = ref requestPool.Get(entity);
                ref Movable           movable     = ref _walkers.Pools.Inc2.Get(entity);

                _elementPool.Value.TryGet(entity, out Element element);
                _powerPool.Value.TryGet(entity, out AttackPower power);

                var isMovable = _boardHelpers.Value.IsMovable(moveRequest.EventData.TargetPosition, 
                    in movable, entity, element.Type, power.CurrentValue);

                if (isMovable.isMovable)
                {
                    if (isMovable.withAttack)
                    {
                        SendAttackRequest(entity, in moveRequest, in power);
                    }
                    else if (isMovable.withSwap)
                    {
                        moveRequest.WithSwap = true;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SendAttackRequest(int entity, in MoveToCellRequest moveRequest, in AttackPower power)
        {
            ref var attackRequest = ref _attackRequestPool.Value.RaiseGameEvent(entity, moveRequest.EventData);
            attackRequest.Power = power.CurrentValue;
        }
    }
}