using Leopotam.EcsLite;
using UnityEngine;

namespace JimboA.Plugins.Tween
{
    [System.Serializable]
    public struct TweenSpriteColor : ITweenComponent
    {
        public SpriteRenderer Renderer;
        public Color? From;
        public Color To;
        
        public bool Handle(float t)
        {
            if (Renderer == null)
            {
                TweenExtensions.WarningTweenNoSource(this);
                return false;
            }
            
            Color from;
            if (From.HasValue)
                from = From.GetValueOrDefault();
            else
                from = Renderer.color;
            
            Renderer.color = Color.LerpUnclamped(from, To, t);
            return true;
        }
    }
    
    public static class TweenSpriteColorExtensions
    {
        public static ref TweenSettings DoColor(this SpriteRenderer renderer, EcsWorld world, Color from, Color to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenSpriteColor>().Add(tweenEntity);

            tween.Renderer = renderer;
            tween.From = from;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoColor(this SpriteRenderer renderer, EcsWorld world, out int tweenEntity,
            Color from, Color to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenSpriteColor>().Add(tweenEntity);

            tween.Renderer = renderer;
            tween.From = from;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoColor(this SpriteRenderer renderer, EcsWorld world, Color to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenSpriteColor>().Add(tweenEntity);

            tween.Renderer = renderer;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoColor(this SpriteRenderer renderer, EcsWorld world, out int tweenEntity,
            Color to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenSpriteColor>().Add(tweenEntity);

            tween.Renderer = renderer;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoFade(this SpriteRenderer renderer, EcsWorld world, float from, float to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenSpriteColor>().Add(tweenEntity);

            var color = renderer.color;
            tween.Renderer = renderer;
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
        
        public static ref TweenSettings DoFade(this SpriteRenderer renderer, EcsWorld world, out int tweenEntity,
            float from, float to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenSpriteColor>().Add(tweenEntity);

            var color = renderer.color;
            tween.Renderer = renderer;
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