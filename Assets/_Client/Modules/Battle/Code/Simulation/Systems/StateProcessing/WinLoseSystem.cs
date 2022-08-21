using System.Runtime.CompilerServices;
using Client.AppData;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation 
{
    public sealed class WinLoseSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Player, Health, Score>> _players = default;
        private EcsPoolInject<WinLoseEvent> _winLosePool = default;
        
        private EcsCustomInject<BattleService> _battle = default;
        private EcsCustomInject<BattleData> _battleData = default;

        public void Run (IEcsSystems systems)
        {
            var battle = _battle.Value;
            if(battle.Phase != BattlePhase.Battle || battle.NextPhase.HasValue)
                return;
            
            var pools = _players.Pools;
            foreach (var entity in _players.Value)
            {
                ref Health hp    = ref pools.Inc2.Get(entity);
                ref Score  score = ref pools.Inc3.Get(entity);
                
                if (score.Value >= _battleData.Value.CurrentLevel.WinLose.WinScore.Value)
                {
                    SetWinLose(entity, true);
                    battle.NextPhase = BattlePhase.WinLose;
                    return;
                }

                if (hp.Value <= 0)
                {
                    SetWinLose(entity, false);
                    battle.NextPhase = BattlePhase.WinLose;
                    return;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetWinLose(int entity, bool isWin)
        {
            ref var winLose = ref _winLosePool.Value.Add(entity);
            winLose.IsWin = isWin;
        }
    }
}