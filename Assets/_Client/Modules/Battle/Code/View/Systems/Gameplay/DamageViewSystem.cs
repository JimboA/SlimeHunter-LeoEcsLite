using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using Client.Battle.View.UI;
using JimboA.Plugins.EcsProviders;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins.ObjectPool;
using JimboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct DamageViewData
    {
        public GameObject Fx;
        public float ShakeDuration;
        public float ShakeFrequency;
        public float FlickerTime;
    }

    public sealed class DamageViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<DamageViewData, Started<DamageProcess>, Health,
            MonoLink<Transform>, 
            MonoLink<SpriteRenderer>>> _register = default;
        
        private EcsFilterInject<Inc<DamageViewData, Executing<DamageProcess>, AttackHitAnimationEvent, Health,
            MonoLink<Transform>, 
            MonoLink<SpriteRenderer>>> _damaged = default;
        
        private EcsPoolInject<ParticleFx> _particleFxPool = default;
        private EcsPoolInject<Shake> _shakePool = default;
        private EcsPoolInject<UpdateWidgetRequest<PlayerHpWidget, int>> _widgetUpdatePool = default;

        private EcsCustomInject<PoolContainer> _viewsObjectPool = default;
        private EcsCustomInject<BattleSceneData> _sceneData = default;
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _register.Value)
            {
                ref Started<DamageProcess> processLink = ref _register.Pools.Inc2.Get(entity);
                _battle.Value.PauseProcess(processLink.ProcessEntity);
            }

            foreach (var entity in _damaged.Value)
            {
                var world = systems.GetWorld();
                ref var pools = ref _damaged.Pools;
                
                ref DamageViewData           damageView  = ref pools.Inc1.Get(entity);
                ref Executing<DamageProcess> processLink = ref pools.Inc2.Get(entity);
                ref Health                   hp          = ref pools.Inc4.Get(entity);
                ref Transform                transform   = ref pools.Inc5.Get(entity).Value;
                ref SpriteRenderer           renderer    = ref pools.Inc6.Get(entity).Value;

                FlickSprite(world, in damageView, renderer);
                CreateDamageParticleFx(world, in damageView, transform, renderer);
                ShakeCamera(in damageView);
                UpdateHealthWidget(entity, hp.Value);
                _battle.Value.UnpauseProcess(processLink.ProcessEntity);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlickSprite(EcsWorld world, in DamageViewData damageView, SpriteRenderer renderer)
        {
            var flickColor = renderer.color == Color.red ? Color.white : Color.red;
            renderer.DoColor(world, Color.white, flickColor, damageView.FlickerTime)
                .Loops(2, true);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateDamageParticleFx(EcsWorld world, in DamageViewData damageView, Transform transform, SpriteRenderer renderer)
        {
            var viewProvider = world.CreateView(damageView.Fx, transform.position, transform.rotation, _viewsObjectPool.Value);
            if (viewProvider.TryGetEntity(out var fxEntity) 
                && _particleFxPool.Value.TryGet(fxEntity, out var particleFx))
            {
                var color = renderer.color;
                var main = particleFx.ParticleSystem.main;
                main.startColor = color;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShakeCamera(in DamageViewData damageView)
        {
            var cameraProvider = _sceneData.Value.BattleCameraProvider;
            if (cameraProvider != null && cameraProvider.TryGetEntity(out var cameraEntity))
            {
                ref var shake = ref _shakePool.Value.GetOrAdd(cameraEntity);
                shake.Duration = damageView.ShakeDuration;
                shake.Frequency = damageView.ShakeFrequency;
                shake.Pivot = cameraProvider.transform.position;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateHealthWidget(int entity, int healthAmount)
        {
            ref var request = ref _widgetUpdatePool.Value.Add(entity);
            request.Value = healthAmount;
        }
    }
}