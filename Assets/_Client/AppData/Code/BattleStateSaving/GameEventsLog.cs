using System;
using System.Collections.Generic;
using Leopotam.EcsLite;

namespace Client.AppData
{
    public interface IGameEventLog
    {
        public void AddValue(int entity);
        public void SetPool(IEcsPool pool);
        public void Restore(EcsWorld world, int entity, int index);

        public int Count { get; }
    }
    
    [Serializable]
    public sealed class GameEventsLog<TEvent> : IGameEventLog where TEvent : struct, IGameEvent
    {
        public List<TEvent> List;
        [NonSerialized] public EcsPool<TEvent> Pool;

        public int Count => List.Count;

        public GameEventsLog(EcsPool<TEvent> pool, int cap = 64)
        {
            List = new List<TEvent>(cap);
            Pool = pool;
        }

        public void AddValue(int entity)
        {
            var data = Pool.Get(entity);
            List.Add(data);
        }

        public void SetPool(IEcsPool pool)
        {
            Pool = (EcsPool<TEvent>)pool;
        }

        public void Restore(EcsWorld world, int entity, int index)
        {
            Pool ??= world.GetPool<TEvent>();
            Pool.Add(entity) = List[index];
        }
    }
}