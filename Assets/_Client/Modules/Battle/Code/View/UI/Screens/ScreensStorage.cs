using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using JimmboA.Plugins.EcsProviders;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Client.Battle.View.UI
{
    public class ScreensStorage
    {
        public Stack<ScreenBase> ActiveScreens = new Stack<ScreenBase>();
        public Dictionary<Type, int> ScreenEntityByType = new Dictionary<Type, int>();
        public Dictionary<Type, ScreenBase> ScreenByType = new Dictionary<Type, ScreenBase>();

        public void Add<TScreen>(EcsWorld world, TScreen screen) where TScreen : ScreenBase
        {
            var key = screen.GetType();
            var entity = world.NewEntity();
            screen.Init(world);
            screen.BindScreen(world, entity);
            ScreenEntityByType.Add(key, entity);
            ScreenByType.Add(key, screen);
            screen.Hide(world);
        }

        public int GetScreenEntity<TScreen>() where TScreen : MonoBehaviour, IScreen
        {
            var key = typeof(TScreen);
            if (ScreenEntityByType.TryGetValue(key, out var entity))
                return entity;

            return -1;
        }
    }
}
