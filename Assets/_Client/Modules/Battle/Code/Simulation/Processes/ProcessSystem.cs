using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class ProcessSystem<TProcess> : IEcsRunSystem where TProcess : struct, IProcessData
    {
        private EcsFilterInject<Inc<TProcess, Process>> _filter = default;
        private EcsFilterInject<Inc<Started<TProcess>>> _started = default;
        private EcsFilterInject<Inc<Completed<TProcess>, HasActiveProcess>> _completed = default;

        private EcsPoolInject<HasActiveProcess> _hasProcessPool = default;
        private EcsPoolInject<Started<TProcess>> _startedPool = default;
        private EcsPoolInject<Executing<TProcess>> _executingPool = default;
        private EcsPoolInject<Completed<TProcess>> _completedPool = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _started.Value)
            {
                _startedPool.Value.Del(entity);
            }

            foreach (var entity in _completed.Value)
            {
                ref Completed<TProcess> completed     = ref _completedPool.Value.Get(entity);
                ref HasActiveProcess    activeProcess = ref _hasProcessPool.Value.Get(entity);
                
                activeProcess.Process.Remove(completed.ProcessEntity);
                if(activeProcess.Process.Length == 0)
                    _hasProcessPool.Value.Del(entity);
                
                _completedPool.Value.Del(entity);
            }

            foreach (var entity in _filter.Value)
            {
                ref Process process = ref _filter.Pools.Inc2.Get(entity);
                var world = systems.GetWorld();

                if (process.Phase == StatePhase.Complete)
                {
                    world.DelEntity(entity);
                    continue;
                }
                
                if (!process.Target.Unpack(world, out var targetEntity))
                {
                    DebugNoTarget(entity);
                    continue;
                }

                if (process.Phase == StatePhase.OnStart)
                {
                    process.Phase = StatePhase.Process;
                    _executingPool.Value.Add(targetEntity) = new Executing<TProcess>(entity);
                }

                if(process.Paused)
                    continue;

                process.Duration -= UnityEngine.Time.deltaTime;
                if (process.Duration <= 0)
                {
                    process.Phase = StatePhase.Complete;
                    if(_executingPool.Value.Has(targetEntity))
                        _executingPool.Value.Del(targetEntity);
                    _completedPool.Value.Add(targetEntity) = new Completed<TProcess>(entity);
                }
            }
        }

        #region Debug

        [Conditional("DEBUG")]
        private void DebugNoTarget(int processEntity)
        {
            throw new Exception($"Target entity from process {typeof(TProcess).Name} is missing. ProcessEntity: {processEntity}. ");
        }

        #endregion
    }
}