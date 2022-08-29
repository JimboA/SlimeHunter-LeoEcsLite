using System.Collections;
using System.Collections.Generic;
using Leopotam.EcsLite;
using JimboA.Plugins.Tween;
using TMPro;
using UnityEngine;

namespace Client.Battle.View.UI
{
    public class KillScoreWidget : WidgetBase, IStatView<int>
    {
        [SerializeField] private TextMeshProUGUI scoreValue;
        [SerializeField] private TextMeshProUGUI requiredValue;

        public void OnInit(int amount, EcsWorld world)
        {
            requiredValue.text = "/ " + amount;
        }
        
        public void OnUpdate(int amount, EcsWorld world)
        {
            scoreValue.text = amount.ToString();
            var tr = scoreValue.transform;
            tr.DoScale(world, Vector3.one, tr.localScale * 1.2f, 0.2f).Loops(2, true);
        }

        public override void SetIcon(Texture2D icon)
        {
            throw new System.NotImplementedException();
        }
    }
}
