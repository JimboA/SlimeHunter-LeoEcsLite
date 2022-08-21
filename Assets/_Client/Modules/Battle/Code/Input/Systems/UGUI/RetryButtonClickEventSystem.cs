using Client.AppData;
using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Client.Input.Ugui
{
    public sealed class RetryButtonClickEventSystem : EcsUguiCallbackSystem 
    {
        [Preserve]
        [EcsUguiClickEvent(BattleIdents.Ui.RetryButtonName)]
        private void OnClick(in EcsUguiClickEvent evt)
        {
            // TODO: temp. for tests only. Replace with custom scene management service
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}