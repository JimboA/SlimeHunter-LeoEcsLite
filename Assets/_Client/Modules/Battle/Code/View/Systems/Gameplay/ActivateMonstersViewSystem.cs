using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using JimmboA.Plugins.EcsProviders;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct ActivateViewData
    { 
        public GameObject FxPrefab;
        [HideInInspector] public Transform InstantiatedFx;
    }
    
    public sealed class ActivateMonstersViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ActivateViewData, Started<ActivateProcess>, Monster,
            MonoLink<Transform>, 
            MonoLink<SpriteRenderer>>> _activated = default;
        
        private EcsPoolInject<ParticleFx> _particleFxPool = default;
        
        private EcsCustomInject<PoolContainer> _viewsObjectPool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _activated.Value)
            {
                ref var pools = ref _activated.Pools;

                ref ActivateViewData activateView = ref pools.Inc1.Get(entity);
                ref Transform        transform    = ref pools.Inc4.Get(entity).Value;
                ref SpriteRenderer   renderer     = ref pools.Inc5.Get(entity).Value;
                
                CreateActivationParticleFx(systems.GetWorld(), ref activateView, transform, renderer);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateActivationParticleFx(EcsWorld world, ref ActivateViewData activateView, Transform transform, SpriteRenderer renderer)
        {
            var viewProvider = world.CreateView(activateView.FxPrefab, Vector3.zero, Quaternion.identity, _viewsObjectPool.Value, transform);
            if (viewProvider.TryGetEntity(out var fxEntity) 
                && _particleFxPool.Value.TryGet(fxEntity, out var particleFx))
            {
                var color = renderer.color;
                var main = particleFx.ParticleSystem.main;
                main.startColor = color;
            }

            activateView.InstantiatedFx = viewProvider.transform;
        }
    }
}