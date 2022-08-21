using System;
using Leopotam.EcsLite;
using JimmboA.Plugins;

namespace Client.Battle.Simulation
{
    public interface IProcessData
    {
    }

    [Serializable]
    public struct Process : IStateData
    {
        public StatePhase Phase { get; set; }
        public float Duration;
        public bool Paused;
        public EcsPackedEntity Target;
    }

    public struct HasActiveProcess : IEcsAutoReset<HasActiveProcess>
    {
        public FastList<int> Process;

        public void AutoReset(ref HasActiveProcess c)
        {
            if (c.Process == null)
                c.Process = new FastList<int>(4);
            else
                c.Process.Clear();
        }
    }

    public readonly struct Completed<TProcess> where TProcess : struct
    {
        public readonly int ProcessEntity;

        public Completed(int processEntity)
        {
            ProcessEntity = processEntity;
        }
    }

    public readonly struct Executing<TProcess> where TProcess : struct
    {
        public readonly int ProcessEntity;

        public Executing(int processEntity)
        {
            ProcessEntity = processEntity;
        }
    }

    public readonly struct Started<TProcess> where TProcess : struct
    {
        public readonly int ProcessEntity;

        public Started(int processEntity)
        {
            ProcessEntity = processEntity;
        }
    }

    public static class ProcessExtensions
    {
        public static ref TProcess GetProcessData<TProcess>(in this Started<TProcess> eventData, EcsPool<TProcess> pool)
            where TProcess : struct
        {
            return ref pool.Get(eventData.ProcessEntity);
        }

        public static ref TProcess GetProcessData<TProcess>(in this Executing<TProcess> eventData,
            EcsPool<TProcess> pool) where TProcess : struct
        {
            return ref pool.Get(eventData.ProcessEntity);
        }

        public static ref TProcess GetProcessData<TProcess>(in this Completed<TProcess> eventData,
            EcsPool<TProcess> pool) where TProcess : struct
        {
            return ref pool.Get(eventData.ProcessEntity);
        }
    }
}