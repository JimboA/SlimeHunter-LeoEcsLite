
using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.Serialization;

namespace JimboA.Plugins.Tween
{
    [System.Serializable]
    public enum EasingType : int
    {
        Linear,
        InQuad,
        OutQuad,
        InOutQuad,
        InCubic,
        OutCubic,
        InOutCubic,
        InQuart,
        OutQuart,
        InOutQuart,
        InQuint,
        OutQuint,
        InOutQuint,
        InSine,
        OutSine,
        InOutSine,
        InExpo,
        OutExpo,
        InOutExpo,
        InCirc,
        OutCirc,
        InOutCirc,
        InElastic,
        OutElastic,
        InOutElastic,
        InBack,
        OutBack,
        InOutBack,
        InBounce,
        OutBounce,
        InOutBounce
    }
    
    [System.Serializable]
    public struct TweenSettings : IEcsAutoReset<TweenSettings>
    {
        public int Direction;
        public int Loops;
        public float Duration;
        [HideInInspector] public float CycleDuration;
        public float Delay;
        public bool IsPause;
        public bool IsTimeUnscaled;
        public bool IsPingPong;
        public bool UseAnimationCurve;
        public EasingType Easing;
        public AnimationCurve EasingCurve;
        public bool Started;
        public bool IsComplete;
        
        public void AutoReset(ref TweenSettings c)
        {
            c.Direction = 1;
            c.Loops = 1;
            c.Duration = 0;
            c.CycleDuration = 0;
            c.Delay = 0;
            c.IsPause = false;
            c.IsTimeUnscaled = false;
            c.IsPingPong = false;
            c.UseAnimationCurve = false;
            c.Easing = EasingType.Linear;
            c.EasingCurve = null;
            c.Started = false;
            c.IsComplete = false;
        }
    }
    
    public static class TweenSettingsExtensions
    {
        public static ref TweenSettings Delay(ref this TweenSettings settings, float delay)
        {
            settings.Delay = delay;
            return ref settings;
        }
        
        public static ref TweenSettings Easing(ref this TweenSettings settings, EasingType easing)
        {
            settings.Easing = easing;
            return ref settings;
        }
        
        public static ref TweenSettings EasingCurve(ref this TweenSettings settings, AnimationCurve curve)
        {
            settings.EasingCurve = curve;
            return ref settings;
        }

        public static ref TweenSettings Loops(ref this TweenSettings settings, int loopsCount, bool pingPong)
        {
            settings.Loops = loopsCount;
            settings.IsPingPong = pingPong;
            return ref settings;
        }
        
        public static ref TweenSettings Pause(ref this TweenSettings settings)
        {
            settings.IsPause = true;
            return ref settings;
        }

        public static ref TweenSettings Flip(ref this TweenSettings settings)
        {
            settings.Direction *= -1;
           if(!settings.Started)
               settings.CycleDuration = settings.Direction < 0 ? 1 : 0;
                
            return ref settings;
        }
    }
}