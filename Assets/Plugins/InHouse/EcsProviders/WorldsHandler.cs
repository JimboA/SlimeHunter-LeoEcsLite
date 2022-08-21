using System.Collections.Generic;
using Leopotam.EcsLite;
using UnityEngine.SceneManagement;

namespace JimmboA.Plugins.EcsProviders
{
    internal static class WorldsHandler
    {
        internal static Dictionary<string, IEcsSystems> Allsystems = new Dictionary<string, IEcsSystems>();
        internal static Dictionary<string, EcsWorld>   DefaultWorlds = new Dictionary<string, EcsWorld>();

        internal static void Add(string sceneName, IEcsSystems systems, EcsWorld defaultWorld = null)
        {
            if (!Allsystems.TryGetValue(sceneName, out _))
                Allsystems.Add(sceneName, systems);

            defaultWorld ??= systems.GetWorld();
            
            if (!DefaultWorlds.TryGetValue(sceneName, out _))
                DefaultWorlds.Add(sceneName, defaultWorld);
        }
        internal static void Add(in Scene scene, IEcsSystems systems, EcsWorld defaultWorld = null)
        {
            Add(scene.name, systems, defaultWorld);
        }

        internal static EcsWorld GetDefaultWorld(in Scene scene)
        {
            if (DefaultWorlds.TryGetValue(scene.name, out var world))
                return world;

            return null;
        }

        internal static EcsWorld GetNamedWorld(in Scene scene, string worldName)
        {
            if (string.IsNullOrEmpty(worldName))
                return null;

            if (Allsystems.TryGetValue(scene.name, out var systems))
            {
                var world = systems.GetWorld(worldName);
                if (world != null)
                    return world;
            }

            return null;
        }
        
        internal static EcsWorld GetNamedWorld(string worldName)
        {
            if (string.IsNullOrEmpty(worldName))
                return null;

            foreach (var system in Allsystems.Values)
            {
                var world = system.GetWorld(worldName);
                if(world != null)
                    return world;
            }

            return null;
        }
    }
}
