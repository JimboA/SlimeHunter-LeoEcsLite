using System.Runtime.CompilerServices;
using Client.AppData;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.Simulation 
{
    public sealed class SetTurnSystem : IEcsRunSystem, IEcsInitSystem, IEcsDestroySystem
    {
        private EcsFilterInject<Inc<NewBattleCycleEvent>> _onNewBattleCycle = GlobalIdents.Worlds.EventWorldName;
        private EcsFilterInject<Inc<Initiative, CanAct>, Exc<TurnFinished>> _awaiting = default;
        private EcsFilterInject<Inc<Initiative, TurnFinished>> _finished = default;
        private EcsFilterInject<Inc<Turn>, Exc<TurnFinished>> _actors = default;

        private EcsPoolInject<Turn> _turnPool = default;
        private EcsPoolInject<TurnFinished> _finishedPool = default;

        private EcsCustomInject<BattleService> _battle = default;
        private EcsFilterReorderHandler _reorderHandler = default;

        public void Init(IEcsSystems systems)
        {
            _reorderHandler = GetInitiativeValue;
        }

        public void Run(IEcsSystems systems)
        {
            var battle = _battle.Value;
            if(battle.Phase != BattlePhase.Battle || battle.NextPhase.HasValue)
                return;

            foreach (var _ in _onNewBattleCycle.Value)
            {
                RecycleTurns();
            }

            if(!TryFinishTurn() || battle.IsAnyoneActing())
                return;

            SortCharacters();
            if (!NextTurn())
                battle.NextPhase = BattlePhase.Collapse;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryFinishTurn()
        {
            if (_actors.Value.TryGetFirst(out var entity))
            {
                ref Turn turn  = ref _actors.Pools.Inc1.Get(entity);
                if (turn.Phase == StatePhase.Complete)
                {
                    _finishedPool.Value.Add(entity);
                    _turnPool.Value.Del(entity);
                    return true;
                }

                return false;
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecycleTurns()
        {
            foreach (var entity in _finished.Value)
            {
                _finished.Pools.Inc2.Del(entity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool NextTurn()
        {
            if (_awaiting.Value.TryGetFirst(out var entity))
            {
                _turnPool.Value.Add(entity);
                _battle.Value.TurnsCount++;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SortCharacters()
        {
            _awaiting.Value.Reorder(_reorderHandler);
        }
        
        private int GetInitiativeValue(int entity)
        {
            return _awaiting.Pools.Inc1.Get(entity).Value * -1; // fastest way for descending order
        }

        public void Destroy(IEcsSystems systems)
        {
            _reorderHandler = null;
        }
    }
}