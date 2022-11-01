using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Client.AppData;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public enum BattlePhase
    {
        Battle,
        Collapse,
        WinLose,
    }

    /// <summary>
    /// Shared battle data and service logic. There are many ways to represent single data in ECS.
    /// We can make a single BattleData component, store data there, set it on a special "battle" entity and receive it through a filter (you can write an extension),
    /// or make a component - link for other entities. But for our case, I'll just make a class that will be injected into systems
    /// </summary>
    public class BattleService : ISaveState
    {
        public BattleState State;
        public bool BlockInput;
        public bool AllowView;
        public bool IsPlayback;
        public int CyclesCount;
        public int TurnsCount;

        private EcsWorld _world;
        private EcsPool<HasActiveProcess> _activeProcessPool;
        private EcsPool<Process> _processPool;
        private EcsFilter _acting;
        private string _saveName;
        private int _currentLevel;

        public BattlePhase Phase { get; private set; }
        public BattlePhase? NextPhase { get; set; }
        public EcsWorld World => _world;
        
        public BattleService(EcsWorld world, BattleState state)
        {
            State = state;
            _world = world;
            _processPool = world.GetPool<Process>();
            _activeProcessPool = world.GetPool<HasActiveProcess>();
            _acting = world.Filter<HasActiveProcess>().End();
            _saveName = $"Level{_currentLevel}BattleSave.save";
        }
    
        public bool TryChangeState()
        {
            if (IsAnyoneActing() || !NextPhase.HasValue || NextPhase.Value == Phase)
                return false;
    
            Phase = NextPhase.Value;
            NextPhase = null;
            return true;
        }

        public bool IsAnyoneActing() => _acting.GetEntitiesCount() > 0;

        #region ProcessesAPI

        public ref TProcess StartNewProcess<TProcess>(EcsPool<TProcess> pool, int entity) where TProcess : struct, IProcessData
        {
            var world = pool.GetWorld();
            var processEntity = world.NewEntity();
            ref var process = ref _processPool.Add(processEntity);
            process.Phase = StatePhase.OnStart;
            process.Target = world.PackEntity(entity);
            ref var activeProc = ref _activeProcessPool.GetOrAdd(entity);
            activeProc.Process.Add(processEntity);

            // Well... I haven't figured out how to cache "Started<>" event pool yet.
            world.GetPool<Started<TProcess>>().Add(entity) = new Started<TProcess>(processEntity);
            return ref pool.Add(processEntity);
        }

        public ref TProcess StartNewProcess<TProcess>(EcsWorld world, int entity) where TProcess : struct, IProcessData
        {
            var pool = world.GetPool<TProcess>();
            return ref StartNewProcess(pool, entity);
        }
        
        public ref TProcess StartNewProcess<TProcess>(int entity) where TProcess : struct, IProcessData
        {
            return ref StartNewProcess(_world.GetPool<TProcess>(), entity);
        }

        public void PauseProcess(int processEntity)
        {
            _processPool.Get(processEntity).Paused = true;
        }
        
        public void UnpauseProcess(int processEntity)
        {
            _processPool.Get(processEntity).Paused = false;
        }
        
        public void SetDurationToProcess(int processEntity, float duration)
        {
            _processPool.Get(processEntity).Duration = duration;
        }

        #endregion
        
        #region SaveLoad

        // TODO: move to separate class maybe
        public void SaveState(IBoard board, EcsWorld world)
        {
            State.CyclesCount = CyclesCount;
            State.TurnsCount = TurnsCount;
            var formatter = new BinaryFormatter();
            var savePath = GlobalIdents.AppData.SavePath;
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var fullPath = $"{savePath}/{_saveName}";
            var file = File.Create(fullPath);
            formatter.Serialize(file, State);
            file.Close();
            Debug.Log($"State saved at path: {fullPath}");
        }

        public bool LoadState(IBoard board, EcsWorld world)
        {
            var fullPath = $"{GlobalIdents.AppData.SavePath}/{_saveName}";
            if (!File.Exists(fullPath))
            {
                Debug.LogWarning($"Can't load state at path: {fullPath}. File not exists");
                return false;
            }

            var formatter = new BinaryFormatter();
            var file = File.Open(fullPath, FileMode.Open);

            try
            {
                State = (BattleState)formatter.Deserialize(file);
                file.Close();
            }
            catch (Exception e)
            {
                Debug.LogError($"Can't deserialize file to {nameof(BattleState)} type. Exception: {e.Message}");
                file.Close();
            }
            return true;
        }

        #endregion
    }
}
