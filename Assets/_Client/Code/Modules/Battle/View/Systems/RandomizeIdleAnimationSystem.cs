using System.Runtime.CompilerServices;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View
{
    public class RandomizeIdleAnimationSystem : IEcsRunSystem
    {
        private static readonly int IdleAnimation = Animator.StringToHash("IdleValue");

        private EcsFilterInject<Inc<MonoLink<Animator>>> _animated;
        
        private EcsPoolInject<SetAnimatorParameterRequest> _animRequestPool = default;

        private EcsCustomInject<RandomService> _random;
    
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _animated.Value)
            {
                
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PlayRandomIdleAnimation(int entity)
        {
            ref var request = ref _animRequestPool.Value.Add(entity);
            request.Hash = IdleAnimation;
            request.Type = AnimatorParameterType.Bool;
            request.IntValue = _random.Value.Random.Next(1, 3);
        }
    }
}