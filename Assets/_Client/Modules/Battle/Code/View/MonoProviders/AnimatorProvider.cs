using JimboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(Animator))]
    public class AnimatorProvider : MonoProvider<MonoLink<Animator>>
    {
        private void Awake()
        {
            if (value.Value == null)
                value.Value = GetComponent<Animator>();
        }
    }
}