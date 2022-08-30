using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using Client.Battle.View.UI;
using JimboA.Plugins.EcsProviders;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.Battle.View
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct DeathViewData
    {
        public GameObject Fx;
        public float ShakeDuration;
        public float ShakeFrequency;
    }
    
    public sealed class DeathViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<DeathViewData, Started<DyingProcess>>> _register = default;
        private EcsFilterInject<Inc<DeathViewData, Executing<DyingProcess>, AttackHitAnimationEvent, MonoLink<Transform>>> _killed = default;
        
        private EcsPoolInject<Shake> _shakePool = default;
        private EcsPoolInject<ParticleFx> _particleFxPool = default;
        private EcsPoolInject<Element> _elementPool = default;
        private EcsPoolInject<UpdateWidgetRequest<PlayerHpWidget, int>> _widgetUpdatePool = default;
        private EcsPoolInject<KillViewRequest> _killViewPool = default;

        private EcsCustomInject<PoolContainer> _viewObjectPool = default;
        private EcsCustomInject<BattleSceneData> _sceneData = default;
        private EcsCustomInject<BattleService> _battle = default;
        

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _register.Value)
            {
                ref Started<DyingProcess>  processLink = ref _register.Pools.Inc2.Get(entity);
                _battle.Value.PauseProcess(processLink.ProcessEntity);
            }
            
            foreach (var entity in _killed.Value)
            {
                var world = systems.GetWorld();
                var objectPool = _viewObjectPool.Value;
                ref var pools = ref _killed.Pools;

                ref DeathViewData           deathView   = ref pools.Inc1.Get(entity);
                ref Executing<DyingProcess> processLink = ref pools.Inc2.Get(entity);
                ref Transform               transform   = ref pools.Inc4.Get(entity).Value;
                
                _battle.Value.UnpauseProcess(processLink.ProcessEntity);
                CreateDeathParticles(world, entity, ref deathView, transform.position, objectPool);
                ShakeCamera(ref deathView);
                UpdateHealthWidget(entity, 0);
                _killViewPool.Value.Add(entity).pooled = true;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CreateDeathParticles(EcsWorld world, int entity, ref DeathViewData deathView, Vector3 position, PoolContainer objectPool)
        {
            var viewProvider = world.CreateView(deathView.Fx, position, Quaternion.identity, objectPool);
            // temp. until there are no actual visual effects
            if (viewProvider.TryGetEntity(out var fxEntity) 
                && _particleFxPool.Value.TryGet(fxEntity, out var particleFx)
                && _elementPool.Value.TryGet(entity, out var element))
            {
                var color = element.GetElementColor();
                var main = particleFx.ParticleSystem.main;
                main.startColor = color;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShakeCamera(ref DeathViewData deathView)
        {
            var cameraProvider = _sceneData.Value.BattleCameraProvider;
            if (cameraProvider != null && cameraProvider.TryGetEntity(out var cameraEntity))
            {
                _shakePool.Value.GetOrAdd(cameraEntity) = new Shake
                {
                    Duration = deathView.ShakeDuration,
                    Frequency = deathView.ShakeFrequency,
                    Pivot = cameraProvider.transform.position
                };
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