using JimmboA.Plugins.EcsProviders;
using TMPro;
using UnityEngine;

namespace Client.Battle.View.UI.Providers
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextProvider : MonoProvider<MonoLink<TextMeshProUGUI>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = GetComponent<TextMeshProUGUI>();
        }
    }
}
