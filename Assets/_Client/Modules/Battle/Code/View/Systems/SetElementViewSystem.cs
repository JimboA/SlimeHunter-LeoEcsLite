using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.View
{
    public sealed class SetElementViewSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<ViewCreatedEvent, Element, Skin>> _elementals = default;
        private EcsFilterInject<Inc<ViewCreatedEvent, Element, MonoLink<SpriteRenderer>>> _renderers = default;

        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _elementals.Value)
            {
                ref var pools = ref _elementals.Pools;

                ref Element element = ref pools.Inc2.Get(entity);
                ref Skin    skin    = ref pools.Inc3.Get(entity);
                SetElementSkin(in skin, in element);
            }
            
            foreach (var entity in _renderers.Value)
            {
                ref var pools = ref _renderers.Pools;

                ref Element        element  = ref pools.Inc2.Get(entity);
                ref SpriteRenderer renderer = ref pools.Inc3.Get(entity).Value;
                SetElementColor(renderer, in element);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetElementSkin(in Skin skin, in Element element)
        {
            foreach (var resolver in skin.Resolvers)
            {
                switch (element.Type)
                {
                    case Elements.None:
                        break;
                    case Elements.Fire:
                        resolver.SetCategoryAndLabel(resolver.GetCategory(), BattleIdents.Elements.Fire);
                        break;
                    case Elements.Water:
                        resolver.SetCategoryAndLabel(resolver.GetCategory(), BattleIdents.Elements.Water);
                        break;
                    case Elements.Ice:
                        resolver.SetCategoryAndLabel(resolver.GetCategory(), BattleIdents.Elements.Ice);
                        break;
                    case Elements.Electric:
                        resolver.SetCategoryAndLabel(resolver.GetCategory(), BattleIdents.Elements.Electric);
                        break;
                    case Elements.Earth:
                        resolver.SetCategoryAndLabel(resolver.GetCategory(), BattleIdents.Elements.Earth);
                        break;
                    case Elements.All:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetElementColor(SpriteRenderer renderer, in Element element)
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