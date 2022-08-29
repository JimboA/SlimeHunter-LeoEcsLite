using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View
{
    public enum AnimatorParameterType
    {
        Int,
        Float,
        Bool
    }
    
    public struct SetAnimatorParameterRequest
    {
        public AnimatorParameterType Type;
        public int Hash;
        public int IntValue;
        public float FloatValue;
        public bool BoolValue;
    }

    // TODO: add animator state component and animation events processing
    public class AnimationSystem : IEcsRunSystem
    {
        // In our case, there is no need to set several parameters on one entity per frame.
        // But if such functionality is needed, then it is better to make SetParameterRequest an entity - event
        private EcsFilterInject<Inc<MonoLink<Animator>, SetAnimatorParameterRequest>> _animated = default;
    
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _animated.Value)
            {
                var pools = _animated.Pools;

                ref Animator                    animator = ref pools.Inc1.Get(entity).Value;
                ref SetAnimatorParameterRequest request  = ref pools.Inc2.Get(entity);
                
                SetParameter(animator, in request);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetParameter(Animator animator, in SetAnimatorParameterRequest request)
        {
            switch (request.Type)
            {
                case AnimatorParameterType.Int:
                    animator.SetInteger(request.Hash, request.IntValue);
                    break;
                case AnimatorParameterType.Float:
                    animator.SetFloat(request.Hash, request.FloatValue);
                    break;
                case AnimatorParameterType.Bool:
                    animator.SetBool(request.Hash, request.BoolValue);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}