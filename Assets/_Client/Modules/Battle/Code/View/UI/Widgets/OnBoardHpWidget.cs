using Leopotam.EcsLite;
using TMPro;
using UnityEngine;

namespace Client.Battle.View.UI
{
    [RequireComponent(typeof(TextMeshPro))]
    public class OnBoardHpWidget : WidgetBase, IStatView<int>
    {
        [SerializeField] private TextMeshPro _healthValue;
        
        public void OnInit(int value, EcsWorld world)
        {
            if (_healthValue == null)
                _healthValue = GetComponent<TextMeshPro>();
            
            OnUpdate(value, world);
        }

        public void OnUpdate(int value, EcsWorld world)
        {
            if (value < 2)
            {
                gameObject.SetActive(false);
                _healthValue.text = "";
                return;
            }
            
            _healthValue.text = value.ToString();
        }
    }
}
