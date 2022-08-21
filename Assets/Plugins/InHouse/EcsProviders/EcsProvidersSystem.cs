using Leopotam.EcsLite;
using UnityEngine.SceneManagement;

namespace JimmboA.Plugins.EcsProviders 
{
    // TODO: rework
    internal sealed class EcsProvidersSystem : IEcsPreInitSystem, IEcsDestroySystem
    {
        private EcsWorld _defaultWorld;
        private Scene _scene;

        public EcsProvidersSystem(Scene scene, EcsWorld defaultWorld = null)
        {
            _defaultWorld = defaultWorld;
            _scene = scene;
        }
        
        public void PreInit(IEcsSystems systems)
        {
            _defaultWorld ??= systems.GetWorld();
            WorldsHandler.Add(_scene, systems, _defaultWorld);
        }

        public void Destroy(IEcsSystems systems)
        {
            var key = _scene.name;
            WorldsHandler.Allsystems.Remove(key);
            WorldsHandler.DefaultWorlds.Remove(key);
        }
    }
}