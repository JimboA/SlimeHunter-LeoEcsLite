using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Unity.Mathematics;

namespace Client.Battle.Simulation
{
    public sealed class PathExecuteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, Path, GridPosition>, Exc<HasActiveProcess>> _walkers = default;
        
        private EcsPoolInject<MoveToCellRequest> _moveRequestPool = default;
        private EcsPoolInject<Changed<Path>> _pathChangedPool = default;
        private EcsPoolInject<InputReceiver> _inputReceiverPool = default;
        
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _walkers.Value)
            {
                if (_battle.Value.IsPlayback 
                    && _inputReceiverPool.Value.Has(entity))
                {
                    continue;
                }
                
                ref var pools = ref _walkers.Pools;
                
                ref Turn         turn    = ref pools.Inc1.Get(entity);
                ref Path         path    = ref pools.Inc2.Get(entity);
                ref GridPosition gridPos = ref pools.Inc3.Get(entity);

                if(turn.Phase != StatePhase.Process)
                    continue;
                
                if (path.Positions.Length == 0)
                {
                    turn.Phase = StatePhase.Complete;
                    continue;
                }

                if (MoveNext(entity, gridPos.Position, ref path))
                {
                    turn.Phase = StatePhase.Process;
                }
                else
                {
                    FinishMoving(entity, ref path);
                    turn.Phase = StatePhase.Complete;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool MoveNext(int entity, int2 currentPos, ref Path path)
        {
            ref var current = ref path.Current;
            if (current == path.Positions.Length)
                return false;

            var battle = _battle.Value;
            var eventPool = _moveRequestPool.Value;
            
            ref var moveRequest = ref eventPool.RaiseGameEvent(entity,
                new GameEventData(currentPos, path.Positions[current], GameEvents.Move));
            if(_inputReceiverPool.Value.Has(entity))
                battle.State.LogEvent(battle, entity, moveRequest.EventData, eventPool);
            
            current++;
            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FinishMoving(int entity, ref Path path)
        {
            path.Current = 0;
            path.Positions.Clear();
            _pathChangedPool.Value.Add(entity);
        }
    }
}