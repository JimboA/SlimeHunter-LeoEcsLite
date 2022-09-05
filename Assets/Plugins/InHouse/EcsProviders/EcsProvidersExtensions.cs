using System;
using System.Diagnostics;
using Leopotam.EcsLite;
using JimboA.Plugins.ObjectPool;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace JimboA.Plugins.EcsProviders
{
    public static class EcsProvidersExtensions
    {
        #region Global

        public static IEcsSystems AddEcsProviders(this IEcsSystems systems, Scene scene, EcsWorld defaultWorld = null)
        {
            return systems.Add(new EcsProvidersSystem(scene, defaultWorld));
        }

        /// <summary>
        /// use this for activate providers in additive scene that doesn't have a startup. Call before scene loading.
        /// </summary>
        /// <param name="sceneName"></param>
        /// <param name="systems"></param>
        /// <param name="defaultWorld"></param>
        public static void ActivateProviders(string sceneName, IEcsSystems systems, EcsWorld defaultWorld = null)
        {
            WorldsHandler.Add(sceneName, systems, defaultWorld);
        }

        #endregion
        
        #region Creating

        public static EntityProvider CreateView(this EcsWorld world, GameObject prefab,
            Vector3 position, Quaternion rotation, PoolContainer pool = null, Transform parent = null)
        {
            var entity = world.NewEntity();
            GameObject obj;
            if (pool != null)
                obj = pool.Spawn(prefab, position, rotation, parent);
            else
                obj = Object.Instantiate(prefab, position, rotation, parent);
            
            var provider = obj.GetComponent<EntityProvider>();
            DebugNoProvider(obj, provider);
            if(!provider.Bootstrap(world, entity))
                world.DelEntity(entity);
            return provider;
        }
        
        public static EntityProvider CreatViewForEntity(this EcsWorld world, int entity, GameObject prefab,
            Vector3 position, Quaternion rotation, PoolContainer pool = null, Transform parent = null)
        {
            GameObject obj;
            if (pool != null)
                obj = pool.Spawn(prefab, position, rotation, parent);
            else
                obj = Object.Instantiate(prefab, position, rotation, parent);
            
            var provider = obj.GetComponent<EntityProvider>();
            DebugNoProvider(obj, provider);
            provider.Bootstrap(world, entity);
            return provider;
        }

        #endregion

        #region Debug

        [Conditional("DEBUG")]
        private static void DebugNoProvider(GameObject prefab, object provider)
        {
            if (provider == null)
            {
                throw new NullReferenceException($"prefab {prefab} has no EntityProvider component");
            }
        }

        #endregion
    }
}
