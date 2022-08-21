using System;
using System.Runtime.CompilerServices;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View
{
    public sealed class SetElementViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ViewCreatedEvent, Element, Monster,
            MonoLink<SpriteRenderer>>> _mobs = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var mobEntity in _mobs.Value)
            {
                ref var pools = ref _mobs.Pools;

                ref Element        element  = ref pools.Inc2.Get(mobEntity);
                ref SpriteRenderer renderer = ref pools.Inc4.Get(mobEntity).Value;
                SetElementColor(renderer, ref element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetElementColor(SpriteRenderer renderer, ref Element element)
        {
            switch (element.Type)
            {
                case Elements.None:
                    break;
                case Elements.Fire:
                    renderer.color = Color.red;
                    break;
                case Elements.Water:
                    renderer.color = Color.blue;
                    break;
                case Elements.Ice:
                    renderer.color = Color.cyan;
                    break;
                case Elements.Electric:
                    renderer.color = Color.yellow;
                    break;
                case Elements.Earth:
                    renderer.color = Color.green;
                    break;
                case Elements.All:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(element), element, null);
            }
        }
    }
}