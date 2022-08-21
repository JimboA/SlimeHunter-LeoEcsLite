using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.Scripting;

namespace Client.Input.Ugui
{
    public sealed class GoButtonClickEventSystem : EcsUguiCallbackSystem
    {
        private EcsFilterInject<Inc<InputReceiver, Turn>> _actingPlayers = default;
        private EcsCustomInject<BattleService> _Battle = default;

        [Preserve]
        [EcsUguiClickEvent(BattleIdents.Ui.GoButtonName)]
        private void OnClick(in EcsUguiClickEvent evt)
        {
            if(_Battle.Value.BlockInput)
                return;

            foreach (var entity in _actingPlayers.Value)
            {
                ref Turn turn = ref _actingPlayers.Pools.Inc2.Get(entity);
                turn.Phase = StatePhase.Process;
            }
        }
    }
}