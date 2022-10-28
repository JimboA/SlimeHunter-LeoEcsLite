using UnityEngine;
using UnityEngine.UI;


namespace Client.Battle.View.UI
{
    // TODO: handle screen orientation change
    public class BattleHudScreen : ScreenBase
    {
        [SerializeField] public PlayerHpWidget  PlayerHP;
        [SerializeField] public KillScoreWidget KillScore;
        [SerializeField] public Button GoButton;
        [SerializeField] public Button SaveButton;
    }
}
