using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client.Battle.View.UI
{
    public struct HideScreenRequest
    {
    }
    public struct ShowScreenRequest
    {
        public Type ScreenType;
    }

    public static class ScreenRequestsExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShowScreen<TScreen>(this EcsWorld world) where TScreen : ScreenBase
        {
            var entity = world.NewEntity();
            var pool = world.GetPool<ShowScreenRequest>();
            pool.Add(entity).ScreenType = typeof(TScreen);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ShowScreen<TScreen>(this EcsPool<ShowScreenRequest> pool) where TScreen : ScreenBase
        {
            var world = pool.GetWorld();
            var entity = world.NewEntity();
            pool.Add(entity).ScreenType = typeof(TScreen);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HideScreen<TScreen>(this EcsWorld world) where TScreen : ScreenBase
        {
            var entity = world.NewEntity();
            var pool = world.GetPool<HideScreenRequest>();
            pool.Add(entity);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HideScreen<TScreen>(this EcsPool<HideScreenRequest> pool) where TScreen : ScreenBase
        {
            var world = pool.GetWorld();
            var entity = world.NewEntity();
            pool.Add(entity);
        }
    }
    
}