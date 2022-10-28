using UnityEngine;

namespace Client.Battle.View.UI
{
    // for now a simple mediator will be enough. 
    public class EcsUguiMediator : MonoBehaviour
    {
        [SerializeField] public BattleHudScreen battleHudScreen;
        [SerializeField] public WinScreen winScreen;
        [SerializeField] public LoseScreen loseScreen;
    }
}
