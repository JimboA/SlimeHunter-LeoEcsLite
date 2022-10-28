using System.Collections.Generic;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

namespace Client.Battle.View.UI
{
    public interface IScreen
    {
        public void Init(EcsWorld world);
        public void Show(EcsWorld world);
        public void Hide(EcsWorld world);
        public void Activate(EcsWorld world);
        public void Deactivate(EcsWorld world);
        public void OrientationChange(EcsWorld world, ScreenOrientation orientation);
    }

    [System.Serializable]
    public abstract class ScreenBase : MonoBehaviour, IScreen
    {
        protected List<Selectable> Selectables = new List<Selectable>();

        public void Init(EcsWorld world)
        {
            GetComponentsInChildren<Selectable>(Selectables);
            OnInit(world);
        }

        public void Show(EcsWorld world)
        {
            gameObject.SetActive(true);
            OnShow(world);
        }

        public void Hide(EcsWorld world)
        {
            OnHide(world);
            gameObject.SetActive(false);
        }

        public void Activate(EcsWorld world)
        {
            foreach (var selectable in Selectables)
            {
                selectable.interactable = true;
            }
        }

        public void Deactivate(EcsWorld world)
        {
            foreach (var selectable in Selectables)
            {
                selectable.interactable = false;
            }
        }

        public virtual void OrientationChange(EcsWorld world, ScreenOrientation orientation)
        {
        }

        protected virtual void OnInit(EcsWorld world) {}
        protected virtual void OnShow(EcsWorld world) {}
        protected virtual void OnHide(EcsWorld world) {}
    }
    
    public static class ScreenExtensions
    {
        public static void BindScreen<TScreen>(this TScreen screen, EcsWorld world, int entity) where TScreen : ScreenBase
        {
            world.Add<MonoLink<TScreen>>(entity).Value = screen;
        }
    }

}
