using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimmboA.Plugins.ObjectPool;

namespace Client.Battle.View
{
    public sealed class RecycleActivationViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ActivateViewData, CanAct, KillViewRequest>> _killed = default;
        private EcsCustomInject<PoolContainer> _viewsObjectPool = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _killed.Value)
            {
                ref ActivateViewData activateView = ref _killed.Pools.Inc1.Get(entity);
                if (activateView.InstantiatedFx != null)
                    _viewsObjectPool.Value.Recycle(activateView.InstantiatedFx.gameObject, true);
            }
        }
    }
}