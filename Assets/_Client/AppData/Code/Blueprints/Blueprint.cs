using System.Collections.Generic;
using Client.Battle.Simulation;
using Client.Battle.View;
using JimmboA.Plugins.EcsProviders;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using JimmboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.AppData.Blueprints
{
    [CreateAssetMenu(menuName = "Game/Blueprints/CreateNew", fileName = "NewBlueprint")]
    public class Blueprint : ScriptableObject, IModelFactory, IViewFactory
    {
        [SerializeReference] public List<ComponentProviderBase> modelComponents;
        [SerializeReference] public List<ComponentProviderBase> viewComponents;

        public int CreateModel(IEcsSystems systems)
        {
            var world = systems.GetWorld();
            var entity = world.NewEntity();
            foreach (var componentProvider in modelComponents)
            {
                componentProvider.Convert(entity, world);
            }

            world.Add<ModelCreatedEvent>(entity);
            world.Add<BlueprintLink>(entity).Blueprint = this;
            return entity;
        }

        public int CreateView(IEcsSystems systems, int model, Vector3 position, PoolContainer pool = null)
        {
            var world = systems.GetWorld();
            foreach (var componentProvider in viewComponents)
            {
                componentProvider.Convert(model, world);
            }
            
            var prefab = world.Get<ViewLinkComponent>(model).Prefab;
            var provider = world.CreatViewForEntity(model, prefab, position, Quaternion.identity, pool);
            if (provider.TryGetEntity(out var viewEntity))
            {
                ref var transform = ref world.Add<MonoLink<Transform>>(viewEntity);
                transform.Value = provider.transform;
                ref var renderer = ref world.Add<MonoLink<SpriteRenderer>>(viewEntity);
                renderer.Value = provider.GetComponent<SpriteRenderer>();
                world.Add<ViewCreatedEvent>(viewEntity);
                return viewEntity;
            }

            return -1;
        }
    }
}
