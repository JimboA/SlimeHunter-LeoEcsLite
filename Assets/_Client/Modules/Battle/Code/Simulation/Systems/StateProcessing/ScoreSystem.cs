using System.Runtime.CompilerServices;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public sealed class ScoreSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Monster, Completed<DyingProcess>>> _killed = default;
        private EcsFilterInject<Inc<Player, Score, Turn>> _players = default;
        
        private EcsPoolInject<Changed<Score>> _scoreChangedPool = default;
        private EcsPoolInject<DyingProcess> _dyingPool = default;

        public void Run(IEcsSystems systems)
        {
            if (!_players.Value.TryGetFirst(out var player))
                return;

            foreach (var entity in _killed.Value)
            {
                ref DyingProcess dying = ref _killed.Pools.Inc2.Get(entity).GetProcessData(_dyingPool.Value);

                if(!dying.Source.Unpack(systems.GetWorld(), out var attacker))
                    continue;
                
                if(attacker != player)
                    continue;

                ref Score score = ref _players.Pools.Inc2.Get(player);
                score.Value++;
                _scoreChangedPool.Value.Add(player);
            }
        }
    }
}