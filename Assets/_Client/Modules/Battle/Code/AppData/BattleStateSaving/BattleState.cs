using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using UnityEngine;
using Random = System.Random;

namespace Client.AppData
{
    public interface ISaveState
    {
        public void SaveState(IBoard board, EcsWorld world);
        public bool LoadState(IBoard board, EcsWorld world);
    }

    public interface IGameEventLogger
    {
        public void LogEvent<TEvent>(BattleService context, int entity, GameEventData data, EcsPool<TEvent> pool)
            where TEvent : struct, IGameEvent;

        public bool RestoreEvent(EcsWorld world, IBoard board, int turn, int current);
    }

    [Serializable]
    public sealed class BattleState : IGameEventLogger
    {
        public int CurrentLevel;
        public int RandomSeed;
        public int CyclesCount;
        public int TurnsCount;

        public Dictionary<int, List<GameEventState>> TurnEvents;
        public Dictionary<GameEvents, IGameEventLog> EventsData;

        public BattleState(int currentLevel)
        {
            CurrentLevel = currentLevel;
            EventsData = new Dictionary<GameEvents, IGameEventLog>();
            TurnEvents = new Dictionary<int, List<GameEventState>>();
            RandomSeed = Environment.TickCount;
        }

        public void LogEvent<TEvent>(BattleService context, int entity, GameEventData data, EcsPool<TEvent> pool)
            where TEvent : struct, IGameEvent
        {
            var turn = context.TurnsCount;
            var eventState = new GameEventState {Data = data, Turn = turn};
            if (EventsData.TryGetValue(data.Type, out var log))
            {
                log.AddValue(entity);
            }
            else
            {
                log = new GameEventsLog<TEvent>(pool);
                log.AddValue(entity);
                EventsData.Add(data.Type, log);
            }
            eventState.Index = log.Count - 1;
            
            if (TurnEvents.TryGetValue(turn, out var events))
            {
                events.Add(eventState);
            }
            else
            {
                events = new List<GameEventState>();
                events.Add(eventState);
                TurnEvents.Add(turn, events);
            }
        }

        public bool RestoreEvent(EcsWorld world, IBoard board, int turn, int current)
        {
            if (TurnEvents.TryGetValue(turn, out var events))
            {
                if (current == events.Count)
                    return false;
                
                var state = events[current];
                if (EventsData.TryGetValue(state.Data.Type, out var log)
                    && board.TryGetTarget(state.Data.OwnerPosition, out var ownerEntity))
                {
                    log.Restore(world, ownerEntity, state.Index);
                    return true;
                }
            }
            else
            {
                Debug.LogError($"Turn: {turn} is not registered");
                return false;
            }

            return false;
        }
    }
}