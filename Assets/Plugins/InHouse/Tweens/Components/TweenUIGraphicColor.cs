using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.UI;

namespace JimmboA.Plugins.Tween
{
    public struct TweenUIGraphicColor : ITweenComponent
    {
        public Graphic Graphics;
        public Color? From;
        public Color To;
        
        public bool Handle(float t)
        {
            if (Graphics == null)
                return false;
            
            Color from;
            if (From.HasValue)
                from = From.GetValueOrDefault();
            else
                from = Graphics.color;
            
            Graphics.color = Color.LerpUnclamped(from, To, t);
            return true;
        }
    }
    
    public static class TweenUIColorExtensions
    {
        public static ref TweenSettings DoColor(this Graphic graphics, EcsWorld world, Color from, Color to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenUIGraphicColor>().Add(tweenEntity);

            tween.Graphics = graphics;
            tween.From = from;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoColor(this Graphic graphics, EcsWorld world, out int tweenEntity,
            Color from, Color to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenUIGraphicColor>().Add(tweenEntity);

            tween.Graphics = graphics;
            tween.From = from;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoColor(this Graphic graphics, EcsWorld world, Color to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenUIGraphicColor>().Add(tweenEntity);

            tween.Graphics = graphics;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoColor(this Graphic graphics, EcsWorld world, out int tweenEntity,
            Color to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenUIGraphicColor>().Add(tweenEntity);

            tween.Graphics = graphics;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoFade(this Graphic graphics, EcsWorld world, float from, float to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenUIGraphicColor>().Add(tweenEntity);

            var color = graphics.color;
            tween.Graphics = graphics;
            color.a = from;
            tween.From = color;
            color.a = to;
            tween.To = color;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoFade(this Graphic graphics, EcsWorld world, out int tweenEntity,
            float from, float to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenUIGraphicColor>().Add(tweenEntity);

            var color = graphics.color;
            tween.Graphics = graphics;
            color.a = from;
            tween.From = color;
            color.a = to;
            tween.To = color;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
    }
}