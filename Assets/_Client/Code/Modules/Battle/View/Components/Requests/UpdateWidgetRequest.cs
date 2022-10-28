using Client.Battle.View.UI;
using UnityEngine;

namespace Client.Battle.View 
{
    public struct UpdateWidgetRequest<TWidget, TValue> where TWidget : MonoBehaviour
    {
        public TValue Value;
    }
}