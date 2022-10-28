using Client.Battle.Simulation;
using Client.Battle.View.UI;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using JimboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.Battle.View
{
    public sealed class RestoreViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<PlaybackEndEvent>> _onEndPlayback;
        private EcsFilterInject<Inc<BlueprintLink, GridPosition>, Exc<ViewLink>> _views;

        private EcsCustomInject<BattleService> _battle;
        private EcsCustomInject<IBoard> _board;
        private EcsCustomInject<PoolContainer> _viewsObjectPool;

        public void Run(IEcsSystems systems)
        {
            foreach (var eventEntity in _onEndPlayback.Value)
            {
                var battle = _battle.Value;
                var board = _board.Value;
                var world = systems.GetWorld();
                
                Debug.LogWarning($"TRY RESTORE VIEW");
                if(!battle.AllowView)
                    return;

                Debug.LogWarning($"RESTORE VIEW");
                foreach (var entity in _views.Value)
                {
                    ref var pools = ref _views.Pools;
                    ref var blueprint = ref pools.Inc1.Get(entity).Blueprint;
                    ref var gridPos = ref pools.Inc2.Get(entity);
                    ref var cell = ref board.GetCellDataFromPosition(gridPos.Position);

                    blueprint.CreateView(world, entity, cell.WorldPosition, _viewsObjectPool.Value);
                }
            }
        }
    }
}