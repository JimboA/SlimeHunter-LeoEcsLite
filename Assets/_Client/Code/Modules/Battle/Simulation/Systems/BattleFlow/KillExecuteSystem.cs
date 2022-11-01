using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class KillExecuteSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<KillRequest, GridPosition>> _toKill = default;
        private EcsFilterInject<Inc<Completed<DyingProcess>>, Exc<Player>> _killedEnemies = default;
        
        private EcsPoolInject<DyingProcess> _dyingPool = default;

        private EcsCustomInject<IBoard> _board = default;
        private EcsCustomInject<BattleService> _battle = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _killedEnemies.Value)
            {
                systems.GetWorld().DelEntity(entity);
            }

            foreach (var entity in _toKill.Value)
            {
                var gridPosPool = _toKill.Pools.Inc2;
                
                ref GridPosition gridPos     = ref gridPosPool.Get(entity);
                ref KillRequest  killRequest = ref _toKill.Pools.Inc1.Get(entity);
                
                _board.Value.ReleaseCell(gridPos.Position);
                gridPosPool.Del(entity);
                StartDyingProcess(entity, killRequest.Source);
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StartDyingProcess(int entity, EcsPackedEntity killer)
        {
            ref var dyingProcess = ref _battle.Value.StartNewProcess(_dyingPool.Value, entity);
            dyingProcess.Source = killer;
        }
    }
}