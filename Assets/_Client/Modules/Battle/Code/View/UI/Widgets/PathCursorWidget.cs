using System.Collections;
using System.Collections.Generic;
using Client.Battle.View.UI;
using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

namespace Client.Battle.View.UI
{
    public class PathCursorWidget : WidgetBase, IStatView<int>
    {
        [SerializeField] private TextMeshPro _powerValueText;
        
        public void OnInit(int value, EcsWorld world)
        {
            OnUpdate(value, world);
        }

        public void OnUpdate(int value, EcsWorld world)
        {
            if (value < 1)
                _powerValueText.text = "";
            else
                _powerValueText.text = value.ToString();
        }
    }
}
