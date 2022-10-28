using System.Collections;
using System.Collections.Generic;
using JimboA.Plugins.EcsProviders;
using UnityEngine;

namespace Client.Battle.View.Providers
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleFxProvider : MonoProvider<ParticleFx>
    {
        private void Awake()
        {
            if (value.ParticleSystem == null)
                value.ParticleSystem = GetComponent<ParticleSystem>();
        }
    }
}
