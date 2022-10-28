using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View.UI
{
    public sealed class UpdateWidgetSystem<TWidget, TValue> : IEcsRunSystem where TWidget : MonoBehaviour, IStatView<TValue>
    {
        private EcsFilterInject<Inc<MonoLink<TWidget>, UpdateWidgetRequest<TWidget, TValue>>> _widgetsToUpdate = default;

        public void Run (IEcsSystems systems) 
        {
            foreach (var entity in _widgetsToUpdate.Value)
            {
                ref var pools = ref _widgetsToUpdate.Pools;

                ref TWidget                              widget  = ref pools.Inc1.Get(entity).Value;
                ref UpdateWidgetRequest<TWidget, TValue> request = ref pools.Inc2.Get(entity);

                widget.OnUpdate(request.Value, systems.GetWorld());
            }
        }
    }
}