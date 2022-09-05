using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace JimboA.Plugins.Tween
{
    public sealed class TweenSystem<TTweenComponent> : IEcsRunSystem where TTweenComponent : struct, ITweenComponent
    {
        private EcsFilterInject<Inc<TTweenComponent, TweenSettings>> _tweens = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var tweenEntity in _tweens.Value)
            {
                var world = systems.GetWorld();
                var pools = _tweens.Pools;

                ref TTweenComponent tween         = ref pools.Inc1.Get(tweenEntity);
                ref TweenSettings   tweenSettings = ref pools.Inc2.Get(tweenEntity);

                if (tweenSettings.IsPause)
                    continue;

                ref var duration = ref tweenSettings.CycleDuration;
                ref var direction = ref tweenSettings.Direction;
                ref var delay = ref tweenSettings.Delay;
                ref var loops = ref tweenSettings.Loops;
                ref var isComplete = ref tweenSettings.IsComplete;
                
                if (isComplete)
                {
                    world.DelEntity(tweenEntity);
                    continue;
                }

                var delta = tweenSettings.IsTimeUnscaled ? Time.unscaledDeltaTime : Time.deltaTime;

                if ((delay -= delta) > 0.0f)
                    continue;

                tweenSettings.Started = true;
                duration += delta / tweenSettings.Duration * direction;
                duration = Mathf.Clamp01(duration);

                float t;
                if (tweenSettings.UseAnimationCurve && tweenSettings.EasingCurve != null)
                    t = tweenSettings.EasingCurve.Evaluate(duration);
                else
                    t = tweenSettings.Easing.Evaluate(duration);

                if (!tween.Handle(t))
                {
	                world.DelEntity(tweenEntity);
	                continue;
                }

                if (duration >= 1 && direction > 0 
                    || duration == 0 && direction < 0)
                {
                    if (tweenSettings.IsPingPong)
                    {
                        direction *= -1;
                    }
                    
                    if (--loops == 0)
                        isComplete = true;
                }
            }
        }
    }

    // thanks Leopotam for idea :)
    public delegate float EasingHandler(float t);

    public static class TweenExtensions
    {
	    private static readonly EasingHandler[] Handlers =
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
        };
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Evaluate(this EasingType easingType, float t)
        {
            return Handlers[(int) easingType](t);
        }

        public static float Linear(float t) => t;
        public static float InQuad(float t) => t * t;
		public static float OutQuad(float t) => 1 - InQuad(1 - t);
		public static float InOutQuad(float t)
		{
			if (t < 0.5) return InQuad(t * 2) / 2;
			return 1 - InQuad((1 - t) * 2) / 2;
		}
		
		public static float InCubic(float t) => t * t * t;
		public static float OutCubic(float t) => 1 - InCubic(1 - t);
		public static float InOutCubic(float t)
		{
			if (t < 0.5) return InCubic(t * 2) / 2;
			return 1 - InCubic((1 - t) * 2) / 2;
		}
		
		public static float InQuart(float t) => t * t * t * t;
		public static float OutQuart(float t) => 1 - InQuart(1 - t);
		public static float InOutQuart(float t)
		{
			if (t < 0.5) return InQuart(t * 2) / 2;
			return 1 - InQuart((1 - t) * 2) / 2;
		}
		
		public static float InQuint(float t) => t * t * t * t * t;
		public static float OutQuint(float t) => 1 - InQuint(1 - t);
		public static float InOutQuint(float t)
		{
			if (t < 0.5) return InQuint(t * 2) / 2;
			return 1 - InQuint((1 - t) * 2) / 2;
		}
		
		public static float InSine(float t) => (float)-Math.Cos(t * Math.PI / 2);
		public static float OutSine(float t) => (float)Math.Sin(t * Math.PI / 2);
		public static float InOutSine(float t) => (float)(Math.Cos(t * Math.PI) - 1) / -2;
		public static float InExpo(float t) => (float)Math.Pow(2, 10 * (t - 1));
		public static float OutExpo(float t) => 1 - InExpo(1 - t);
		public static float InOutExpo(float t)
		{
			if (t < 0.5) return InExpo(t * 2) / 2;
			return 1 - InExpo((1 - t) * 2) / 2;
		}
		
		public static float InCirc(float t) => -((float)Math.Sqrt(1 - t * t) - 1);
		public static float OutCirc(float t) => 1 - InCirc(1 - t);
		public static float InOutCirc(float t)
		{
			if (t < 0.5) return InCirc(t * 2) / 2;
			return 1 - InCirc((1 - t) * 2) / 2;
		}
		
		public static float InElastic(float t) => 1 - OutElastic(1 - t);
		public static float OutElastic(float t)
		{
			float p = 0.3f;
			return (float)Math.Pow(2, -10 * t) * (float)Math.Sin((t - p / 4) * (2 * Math.PI) / p) + 1;
		}
		
		public static float InOutElastic(float t)
		{
			if (t < 0.5) return InElastic(t * 2) / 2;
			return 1 - InElastic((1 - t) * 2) / 2;
		}

		public static float InBack(float t)
		{
			float s = 1.70158f;
			return t * t * ((s + 1) * t - s);
		}
		
		public static float OutBack(float t) => 1 - InBack(1 - t);
		
		public static float InOutBack(float t)
		{
			if (t < 0.5) return InBack(t * 2) / 2;
			return 1 - InBack((1 - t) * 2) / 2;
		}

		public static float InBounce(float t) => 1 - OutBounce(1 - t);

		public static float OutBounce(float t)
		{
			float div = 2.75f;
			float mult = 7.5625f;

			if (t < 1 / div)
			{
				return mult * t * t;
			}
			else if (t < 2 / div)
			{
				t -= 1.5f / div;
				return mult * t * t + 0.75f;
			}
			else if (t < 2.5 / div)
			{
				t -= 2.25f / div;
				return mult * t * t + 0.9375f;
			}
			else
			{
				t -= 2.625f / div;
				return mult * t * t + 0.984375f;
			}
		}
		
		public static float InOutBounce(float t)
		{
			if (t < 0.5) return InBounce(t * 2) / 2;
			return 1 - InBounce((1 - t) * 2) / 2;
		}

		#region Debug

#if UNITY_EDITOR
	    [Conditional("DEBUG")]
	    public static void WarningTweenNoSource(ITweenComponent tween)
	    {
		    Debug.LogWarning($"Source object of {tween.GetType().Name} is null");
	    }
#endif

		#endregion
    }
}