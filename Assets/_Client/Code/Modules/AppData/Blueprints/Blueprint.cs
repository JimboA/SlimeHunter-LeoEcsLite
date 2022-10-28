using System.Collections.Generic;
using Client.Battle.Simulation;
using Client.Battle.View;
using JimboA.Plugins.EcsProviders;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using JimboA.Plugins.ObjectPool;
using UnityEngine;

namespace Client.AppData.Blueprints
{
    [CreateAssetMenu(menuName = "Game/Blueprints/CreateNew", fileName = "NewBlueprint")]
    public class Blueprint : ScriptableObject, IModelFactory, IViewFactory
    {
        [SerializeReference] public List<ComponentProviderBase> ModelComponents;
        [SerializeReference] public List<ComponentProviderBase> ViewComponents;

        public int CreateModel(EcsWorld world)
        {
            var entity = world.NewEntity();
            foreach (var componentProvider in ModelComponents)
            {
                componentProvider.Convert(entity, world);
            }

            world.Add<ModelCreatedEvent>(entity);
            world.Add<BlueprintLink>(entity).Blueprint = this;
            return entity;
        }

        public void SetModelFor(int entity, EcsWorld world)
        {
            foreach (var componentProvider in ModelComponents)
            {
                componentProvider.Convert(entity, world);
            }

            world.Add<ModelCreatedEvent>(entity);
            world.GetOrAdd<BlueprintLink>(entity).Blueprint = this;
        }

        public int CreateView(EcsWorld world, int model, Vector3 position, PoolContainer pool = null)
        {
            foreach (var componentProvider in ViewComponents)
            {
                componentProvider.Convert(model, world);
            }
            
            var prefab = world.Get<ViewLink>(model).Prefab;
            var provider = world.CreatViewForEntity(model, prefab, position, Quaternion.identity, pool);
            if (provider.TryGetEntity(out var viewEntity))
            {
                ref var transform = ref world.Add<MonoLink<Transform>>(viewEntity);
                transform.Value = provider.transform;
                world.Add<ViewCreatedEvent>(viewEntity);
                return viewEntity;
            }

            return -1;
        }
    }
}
