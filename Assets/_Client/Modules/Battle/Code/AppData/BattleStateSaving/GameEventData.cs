using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

namespace Client.AppData
{
    // we don't need to save "input" itself, rather we can save the "actions" of players that affect the battle.
    [Serializable]
    public enum GameEvents
    {
        SingleAttack,
        Move
    }

    public interface IGameEvent
    {
        public GameEventData EventData { get; set; }
    }

    [Serializable]
    public struct GameEventState
    {
        public int Index;
        public int Turn;
        public GameEventData Data;
    }
    
    [Serializable]
    public struct GameEventData
    {
        public int2 OwnerPosition;
        public int2 TargetPosition;
        public GameEvents Type;

        public GameEventData(int2 ownerPosition, int2 targetPosition, GameEvents type)
        {
            OwnerPosition = ownerPosition;
            TargetPosition = targetPosition;
            Type = type;
        }
    }
    
    public static class GameEventsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TEvent RaiseGameEvent<TEvent>(this EcsPool<TEvent> pool, int entity, GameEventData data) where TEvent : struct, IGameEvent
        {
            ref var gameEvent = ref pool.Add(entity);
            gameEvent.EventData = data;
            return ref gameEvent;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TEvent RaiseGameEvent<TEvent>(this EcsWorld world, int entity, GameEventData data) where TEvent : struct, IGameEvent
        {
            return ref world.GetPool<TEvent>().RaiseGameEvent(entity, data);
        }
    }
}