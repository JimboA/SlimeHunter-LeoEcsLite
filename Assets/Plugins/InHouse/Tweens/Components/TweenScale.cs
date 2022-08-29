using Leopotam.EcsLite;
using UnityEngine;

namespace JimboA.Plugins.Tween
{
    [System.Serializable]
    public struct TweenScale : ITweenComponent
    {
        public Transform Transform;
        public Vector3 From;
        public Vector3 To;

        public bool Handle(float t)
        {
            if (Transform == null)
                return false;

            Transform.localScale = Vector3.LerpUnclamped(From, To, t);
            return true;
        }

    }
    
    public static class TweenScaleExtensions
    {
        public static ref TweenSettings DoScale(this Transform transform, EcsWorld world, Vector3 from, Vector3 to, float time)
        {
            var tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenScale>().Add(tweenEntity);

            tween.Transform = transform;
            tween.From = from;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
        
        public static ref TweenSettings DoScale(this Transform transform, EcsWorld world, out int tweenEntity,
            Vector3 from, Vector3 to, float time)
        {
            tweenEntity = world.NewEntity();
            ref var tweenSettings = ref world.GetPool<TweenSettings>().Add(tweenEntity);
            ref var tween = ref world.GetPool<TweenScale>().Add(tweenEntity);

            tween.Transform = transform;
            tween.From = from;
            tween.To = to;

            tweenSettings.Direction = 1;
            tweenSettings.Duration = time;
            tweenSettings.Loops = 1;
            tweenSettings.Easing = EasingType.Linear;
            return ref tweenSettings;
        }
    }
}