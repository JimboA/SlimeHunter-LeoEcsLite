using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine.Scripting;

namespace Client.Input.Ugui
{
    // TODO: for tests
    public sealed class SaveButtonClickEventSystem : EcsUguiCallbackSystem
    {
        private EcsCustomInject<BattleService> _context = default;
        private EcsCustomInject<IBoard> _board = default;

        [Preserve]
        [EcsUguiClickEvent(BattleIdents.Ui.SaveButtonName)]
        private void OnClick(in EcsUguiClickEvent evt)
        {
            var battle = _context.Value;
            if(battle.BlockInput)
                return;
            
            battle.SaveState(_board.Value, battle.World);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }
    }
}