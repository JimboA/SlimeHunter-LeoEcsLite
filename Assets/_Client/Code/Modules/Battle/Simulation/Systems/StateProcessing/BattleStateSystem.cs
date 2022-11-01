using System.Runtime.CompilerServices;
using Client.AppData;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using JimboA.Plugins.FrameworkExtensions;

namespace Client.Battle.Simulation
{
    public sealed class BattleStateSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsPoolInject<NewBattleCycleEvent> _battleCycleEventPool = GlobalIdents.Worlds.EventWorldName;
        private EcsPoolInject<BattleStateChangedEvent> _battleStateEventPool = GlobalIdents.Worlds.EventWorldName;
        private EcsCustomInject<BattleService> _battle = default;

        public void Init(IEcsSystems systems)
        {
            _battle.Value.NextPhase = BattlePhase.Collapse;
        }

        public void Run(IEcsSystems systems)
        {
            var battle = _battle.Value;
            if (battle.TryChangeState()) 
                StateChanged(battle);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StateChanged(BattleService context)
        {
            var state = context.Phase;
            if (state == BattlePhase.Battle)
            {
                _battle.Value.CyclesCount++;
                _battleCycleEventPool.Value.SendEvent();
                Debug.Log($"New battle cycle {_battle.Value.CyclesCount}");
            }
        
            _battleStateEventPool.Value.SendEvent().phase = state;
            Debug.Log($"Battle state changed to: {state}");
        }
    }
}