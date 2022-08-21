using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.ObjectPool;

namespace Client.Battle.View 
{
    public sealed class AutoDestroyParticleFxSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ParticleFx>> _particles;
        private EcsCustomInject<PoolContainer> _viewsObjectPool;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _particles.Value)
            {
                var particleSystemPool = _particles.Pools.Inc1;
                ref ParticleFx particleFx = ref particleSystemPool.Get(entity);

                if (particleFx.ParticleSystem != null && !particleFx.ParticleSystem.isPlaying)
                {
                    _viewsObjectPool.Value.Recycle(particleFx.ParticleSystem.gameObject, true);
                    particleSystemPool.Del(entity);
                }
            }
        }
    }
}