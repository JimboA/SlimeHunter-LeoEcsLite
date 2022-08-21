using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.View.UI
{
    public sealed class BattleUIInitSystem : IEcsInitSystem
    {
        private EcsFilterInject<Inc<Player, Health, Score>> _players = default;
        
        private EcsCustomInject<EcsUguiMediator> _mediator = default;
        private EcsCustomInject<ScreensStorage> _screens = default;
        private EcsCustomInject<BattleData> _battleData = default;

        public void Init(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var mediator = _mediator.Value;
            var battleScreen = mediator.battleHudScreen;
            var screens = _screens.Value;
            var winLose = _battleData.Value.CurrentLevel.WinLose;
            
            BindPlayerWidgets(world, battleScreen, winLose);
            
            screens.Add(world, battleScreen);
            screens.Add(world, mediator.winScreen);
            screens.Add(world, mediator.loseScreen);
            
            ShowBattleHud(world, screens);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void BindPlayerWidgets(EcsWorld world, BattleHudScreen battleScreen, WinLoseConditions winLose)
        {
            if (_players.Value.TryGetFirst(out var playerEntity))
            {
                ref var hp = ref _players.Pools.Inc2.Get(playerEntity);
                battleScreen.PlayerHP.BindWidget(world, playerEntity);
                battleScreen.KillScore.BindWidget(world, playerEntity);
                battleScreen.PlayerHP.OnInit(hp.Value, world);
                battleScreen.KillScore.OnInit(winLose.WinScore.Value, world);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ShowBattleHud(EcsWorld world, ScreensStorage screens)
        {
            world.ShowScreen<BattleHudScreen>();
        }
    }
}