using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace JimboA.Plugins.EcsProviders
{
    [DisallowMultipleComponent]
    public sealed class EntityProvider : MonoBehaviour
    {
        [SerializeField] internal string WorldName; 
        [SerializeField] internal string EntityID;

        internal EcsPackedEntity Entity;
        internal EcsWorld World;
        
        private bool _isInitialized;

        public EcsWorld GetWorld => World;
        public int? TryGetEntity()
        {
            if (World != null && Entity.Unpack(World, out var entity))
                return entity;

            return null;
        }
        public bool TryGetEntity(out int entity)
        {
            entity = -1;
            return World != null && Entity.Unpack(World, out entity);
        }

        internal bool Bootstrap(EcsWorld world, int entity)
        {
            if(_isInitialized)
                return false;

            Entity    = world.PackEntity(entity);
            World     = world;
            EntityID = entity.ToString();

            ActivateChildren(entity);
            ActivateMonoProviders(entity);

            _isInitialized = true;
            return true;
        }

        internal void Activate()
        {
            if(_isInitialized)
                return;

            EcsWorld world;
            if (string.IsNullOrEmpty(WorldName))
                world = WorldsHandler.GetDefaultWorld(gameObject.scene);
            else
                world = WorldsHandler.GetNamedWorld(gameObject.scene, WorldName) ?? WorldsHandler.GetNamedWorld(WorldName);
            
            DebugNoWorld(world);
            var entity = world.NewEntity();
            if(!Bootstrap(world, entity))
                world.DelEntity(entity);
        }

        private void ActivateChildren(int entity)
        {
            var tr = transform;
            var len = tr.childCount;
            for (int i = 0; i < len; i++)
            {
                var child = tr.GetChild(i);
                if (child.TryGetComponent<EntityProvider>(out var provider))
                {
                    provider.Activate();
                }

                ActivateMonoProviders(entity);
            }
        }

        private void ActivateMonoProviders(int entity)
        {
            var components = GetComponents<MonoProviderBase>();
            foreach (var component in components)
            {
                if(component is IConvertToEntity convertable)
                    convertable.Convert(entity, World);
            }
        }

        private void Start()
        {
            Activate();
        }

        private void OnDisable()
        {
            _isInitialized = false;
        }

        #region Debug

        [Conditional("DEBUG")]
        private void DebugNoWorld(EcsWorld world)
        {
            if (world == null)
            {
                Debug.LogError($"Can't find any registered world to create an entity", transform);
                throw new NullReferenceException();
            }
        }

        #endregion
    }
}

