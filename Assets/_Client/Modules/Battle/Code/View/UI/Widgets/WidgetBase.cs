using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using UnityEngine;

namespace Client.Battle.View.UI
{
    public interface IStatView<in TValue>
    {
        public void OnInit(TValue value, EcsWorld world);
        public void OnUpdate(TValue value, EcsWorld world);
    }

    public abstract class WidgetBase : MonoBehaviour
    {
        public virtual void SetIcon(Texture2D icon){}
    }

    public static class WidgetExtensions
    {
        public static void BindWidget<TWidget>(this TWidget widget, EcsWorld world, int entity) where TWidget : WidgetBase
        {
            world.Add<MonoLink<TWidget>>(entity) = new MonoLink<TWidget>{ Value = widget};
        }
    }
}