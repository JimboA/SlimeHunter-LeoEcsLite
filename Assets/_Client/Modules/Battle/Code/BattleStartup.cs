using System.Collections.Generic;
using Client.AppData;
using Client.Battle.Simulation;
using Client.Input;
using Client.Input.Ugui;
using Client.Battle.View;
using Client.Battle.View.UI;
using JimboA.Plugins.EcsProviders;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using Leopotam.EcsLite.Unity.Ugui;
using JimboA.Plugins.ObjectPool;
using JimboA.Plugins.Tween;
using UnityEngine;

namespace Client.Battle
{
    public sealed class BattleStartup : MonoBehaviour
    {
        // for tests
        public bool LoadAtStart;
        // TODO: implement RestoreViewSystem first
        //public bool AllowView;
        public int  PlaybackSpeed;

        [SerializeField] private BattleData _battleData;
        [SerializeField] private BattleSceneData _sceneData;
        [SerializeField] private EcsUguiEmitter _uguiEmitter;
        [SerializeField] private EcsUguiMediator _uguiMediator;

        private EcsWorld _world;
        private EcsWorld _globalEventsWorld;
        private EcsSystems _systems;
        private IBoard _board;
        private BoardMovementHelpers _boardHelpers;
        private RandomService _random;
        private BattleService _battle;
        private ScreensStorage _screens;
        private GridPathFinding _pathFinding;
        private PoolContainer _viewsObjectPool;

        private void Awake()
        {
            Application.targetFrameRate = 60;

            // TODO: add configs to worlds
            _world             = new EcsWorld();
            _globalEventsWorld = new EcsWorld();
            _board             = new Board(_world, _battleData.CurrentLevel.Board);
            _boardHelpers      = new BoardMovementHelpers(_world, _board);
            _battle            = new BattleService(_world, new BattleState(_battleData.currentLevelId));
            _battle.AllowView  = true;

            // for tests
            if (LoadAtStart && _battle.LoadState(_board, _world))
            {
                _battle.IsPlayback = true;
                _battle.BlockInput = true;
            }

            _random          = new RandomService(_battle.State.RandomSeed);
            _pathFinding     = new GridPathFinding();
            _screens         = new ScreensStorage();
            _viewsObjectPool = new PoolContainer(512);
            _systems         = new EcsSystems(_world);

            _systems
                .AddWorld(_globalEventsWorld, GlobalIdents.Worlds.EventWorldName)
                .AddEcsProviders(gameObject.scene, _world)
                
                .AddGroup(GlobalIdents.Worlds.SimulationGroupName, true,
                    GlobalIdents.Worlds.EventWorldName,
                    SimulationSystemGroup())
                
                .AddGroup(GlobalIdents.Worlds.ViewGroupName, _battle.AllowView,
                    GlobalIdents.Worlds.EventWorldName,
                    ViewSystemGroup())

                //---Cleanup events------------------------------------------------------

                // global events
                .DelHere<NewBattleCycleEvent>(GlobalIdents.Worlds.EventWorldName)
                .DelHere<BattleStateChangedEvent>(GlobalIdents.Worlds.EventWorldName)
                .DelHere<InputTouchStartedEvent>(GlobalIdents.Worlds.EventWorldName)
                .DelHere<InputTouchCanceledEvent>(GlobalIdents.Worlds.EventWorldName)

                // events belonging to an entity
                .DelHere<ModelCreatedEvent>()
                .DelHere<ViewCreatedEvent>()
                .DelHere<AttackHitAnimationEvent>()
                .DelHere<PlaybackEndEvent>()
                
                .DelHere<AddTargetRequest>()
                .DelHere<MoveToCellRequest>()
                .DelHere<SingleAttackRequest>()
                .DelHere<DamageRequest>()
                .DelHere<KillRequest>()
                .DelHere<KillViewRequest>()
                .DelHere<ShowScreenRequest>()
                .DelHere<HideScreenRequest>()
                
                .DelHere<Changed<Path>>()
                .DelHere<Changed<Score>>()
#if UNITY_EDITOR
                .Add(new Leopotam.EcsLite.UnityEditor.EcsWorldDebugSystem())
#endif
                .Inject(
                    _battleData,
                    _sceneData,
                    _uguiMediator,
                    _battle,
                    _board,
                    _boardHelpers,
                    _pathFinding,
                    _random,
                    _screens,
                    _viewsObjectPool)
                .InjectUgui(_uguiEmitter)
                .Init();
        }

        private IEcsSystem[] SimulationSystemGroup()
        {
            var systemGroup = new List<IEcsSystem>(64);
            systemGroup
                //---Initialization-------------------------------------------
                
                // game actions registration
                .AddToGroup(new ProcessSystem<ActivateProcess>())
                .AddToGroup(new ProcessSystem<SingleAttackProcess>())
                .AddToGroup(new ProcessSystem<MoveProcess>())
                .AddToGroup(new ProcessSystem<DamageProcess>())
                .AddToGroup(new ProcessSystem<DyingProcess>())
                .AddToGroup(new ProcessSystem<FallingProcess>())
                
                // board setup
                .AddToGroup(new BoardInitSystem())

                //---Playback systems----------------------------------------------------
                .AddToGroup(new PlaybackEndSystem())
                .AddToGroup(new PlayerPlaybackSystem())
                
                //---Player Input--------------------------------------------------------
                .AddToGroup(new BattleInputSystem())
                .AddToGroup(new TargetSelectingSystem())
                
                // target selecting checks
                .AddToGroup(new CheckTargetAlreadyHasSystem())
                .AddToGroup(new AddTargetToPathSystem())
                .AddToGroup(new PathCursorSystem())
                .AddToGroup(new ClearPathOnNewCycleSystem())

                //---State processing----------------------------------------------------
                .AddToGroup(new BattleStateSystem())
                .AddToGroup(new ScoreSystem())
                .AddToGroup(new WinLoseSystem())

                //---Battle flow---------------------------------------------------------

                // preparation 
                .AddToGroup(new ActivateMonstersSystem())
                .AddToGroup(new SetTurnSystem())
                .AddToGroup(new ResetPowerSystem())
                .AddToGroup(new MovableSettingsSystem())

                // monsters AI
                /* For now monsters AI are represented by simple hard-code systems. It's ok, but pretty rigid.
                 In future updates it will be replaced with behaviour tree editor and one "AISystem". Maybe :)
                 */
                .AddToGroup(new AiAttackPerformSystem())
                .AddToGroup(new AiFollowByPlayerSystem())
                .AddToGroup(new AiFinishActingSystem())

                // sending requests to execute battle actions
                .AddToGroup(new PathExecuteSystem())
                .AddToGroup(new PathComboApplySystem())
                .AddToGroup(new TryMoveSystem())

                // battle actions executing
                .AddToGroup(new SingleAttackExecuteSystem())
                .AddToGroup(new DamageExecuteSystem())
                .AddToGroup(new KillExecuteSystem())
                .AddToGroup(new MoveToCellExecuteSystem())

                //---Collapse flow-------------------------------------------------------
                .AddToGroup(new CollapseSystem())
                .AddToGroup(new SpawnPiecesSystem())

                //---Common--------------------------------------------------------------
                .AddToGroup(new ElementsSetupSystem());

            return systemGroup.ToArray();
        }

        private IEcsSystem[] ViewSystemGroup()
        {
            var systemGroup = new List<IEcsSystem>(64);
            systemGroup

                //---Initialization------------------------------------------------------
                .AddToGroup(new CameraSetupSystem())
                .AddToGroup(new BattleUIInitSystem())

                //---Update--------------------------------------------------------------

                // timers
                .AddToGroup(new DelayedOperationsSystem<AttackHitAnimationEvent>())

                // tweening
                .AddToGroup(new TweenSystem<TweenMove>())
                .AddToGroup(new TweenSystem<TweenScale>())
                .AddToGroup(new TweenSystem<TweenSpriteColor>())
                .AddToGroup(new TweenSystem<TweenUIGraphicColor>())

                // gameplay view systems
                .AddToGroup(new SpawnBoardPiecesViewSystem())
                .AddToGroup(new ActivateMonstersViewSystem())
                .AddToGroup(new SingleAttackViewSystem())
                .AddToGroup(new AttackWhileMovingViewSystem())
                .AddToGroup(new DamageViewSystem())
                .AddToGroup(new MoveViewSystem())
                .AddToGroup(new CollapseViewSystem())
                .AddToGroup(new DeathViewSystem())
                .AddToGroup(new ScoreViewSystem())
                .AddToGroup(new WinLoseViewSystem())
                .AddToGroup(new RecycleActivationViewSystem())
                .AddToGroup(new PathViewSystem())
                .AddToGroup(new PathCursorViewSystem())
                .AddToGroup(new SetElementViewSystem())

                // UI
                .AddToGroup(new HealthWidgetSetupSystem())
                .AddToGroup(new UpdateWidgetSystem<PlayerHpWidget, int>())
                .DelHere<UpdateWidgetRequest<PlayerHpWidget, int>>(_world)
                .AddToGroup(new UpdateWidgetSystem<KillScoreWidget, int>())
                .DelHere<UpdateWidgetRequest<KillScoreWidget, int>>(_world)
                .AddToGroup(new UpdateWidgetSystem<OnBoardHpWidget, int>())
                .DelHere<UpdateWidgetRequest<OnBoardHpWidget, int>>(_world)
                .AddToGroup(new ShowScreenSystem())
                .AddToGroup(new HideScreensSystem())
                
                // ugui events callbacks
                .AddToGroup(new GoButtonClickEventSystem())
                .AddToGroup(new SaveButtonClickEventSystem())
                .AddToGroup(new RetryButtonClickEventSystem())

                // common view
                .AddToGroup(new AnimationSystem())
                .DelHere<SetAnimatorParameterRequest>(_world)
                .AddToGroup(new AutoDestroyParticleFxSystem())
                .AddToGroup(new ShakeSystem())
                .AddToGroup(new KillViewSystem());

            return systemGroup.ToArray();
        }

        private void Playback(int cyclesPerFrame)
        {
            for (int i = 0; i < cyclesPerFrame; i++)
            {
                _systems?.Run();
            }
        }

        private void Update()
        {
            if (_battle.IsPlayback)
            {
                Playback(PlaybackSpeed);
            }
            else
            {
                _systems?.Run();
            }
        }

        private void OnDestroy()
        {
            if (_systems != null)
            {
                _systems.GetWorld(GlobalIdents.Worlds.EventWorldName).Destroy();
                _systems.GetWorld().Destroy();
                _systems.Destroy();
                _systems = null;
            }
        }
    }

    public static class EcsSystemGroupExtensions
    {
        public static List<IEcsSystem> AddToGroup(this List<IEcsSystem> systemGroup, IEcsSystem system)
        {
            systemGroup.Add(system);
            return systemGroup;
        }
        
        public static List<IEcsSystem> DelHere<TComponent>(this List<IEcsSystem> systemGroup, EcsWorld world) where TComponent : struct
        {
            systemGroup.Add(new DelHereSystem<TComponent>(world));
            return systemGroup;
        }
    }
}