using Leopotam.EcsLite;
using UnityEngine;

namespace JimboA.Plugins.Tween
{
    [System.Serializable]
    public struct TweenMove : ITweenComponent
    {
        public Transform Transform;
        public Vector3? From;
        public Vector3 To;
        public bool IsWorld;

        public bool Handle(float t)
        {
            if (Transform == null)
            {
                TweenExtensions.WarningTweenNoSource(this);
                return false;
            }

            Vector3 from;
            if (From.HasValue)
                from = From.GetValueOrDefault();
            else
                from = Transform.position;

            var pos = Vector3.LerpUnclamped(from, To, t);
            if (IsWorld)
                Transform.position = pos;
            else
                Transform.localPosition = pos;

            return true;
        }
    }

    public static class TweenMoveExtensions
    {
        public static ref TweenSettings DoMove(this Transform transform, EcsWorld world, Vector3 from, Vector3 to, float time, bool isWorld = true)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenMove>().Add(tweenEntity);

            tween.Transform = transform;
            tween.From = from;
            tween.To = to;
            tween.IsWorld = isWorld;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoMove(this Transform transform, EcsWorld world, out int tweenEntity, 
            Vector3 from, Vector3 to, float time, bool isWorld = true)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenMove>().Add(tweenEntity);

            tween.Transform = transform;
            tween.From = from;
            tween.To = to;
            tween.IsWorld = isWorld;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoMove(this Transform transform, EcsWorld world, Vector3 to, float time, bool isWorld = true)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenMove>().Add(tweenEntity);

            tween.Transform = transform;
            tween.To = to;
            tween.IsWorld = isWorld;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoMove(this Transform transform, EcsWorld world, out int tweenEntity,
            Vector3 to, float time, bool isWorld = true)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenMove>().Add(tweenEntity);

            tween.Transform = transform;
            tween.To = to;
            tween.IsWorld = isWorld;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
    }
}