using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class KillExecuteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<KillRequest, GridPosition>> _killed = default;
        private EcsFilterInject<Inc<Completed<DyingProcess>>, Exc<Player>> _dyingEnemies = default;
        
        private EcsPoolInject<DyingProcess> _dyingPool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _dyingEnemies.Value)
            {
                systems.GetWorld().DelEntity(entity);
            }

            foreach (var entity in _killed.Value)
            {
                var gridPosPool = _killed.Pools.Inc2;
                
                ref GridPosition gridPos     = ref gridPosPool.Get(entity);
                ref KillRequest  killRequest = ref _killed.Pools.Inc1.Get(entity);
                
                _board.Value.ReleaseCell(gridPos.Position);
                gridPosPool.Del(entity);
                StartDyingProcess(entity, killRequest.Source);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartDyingProcess(int entity, EcsPackedEntity killer)
        {
            ref var killed = ref _battle.Value.StartNewProcess(_dyingPool.Value, entity);
            killed.Source = killer;
        }
    }
}