using Leopotam.EcsLite;
using UnityEngine;

namespace JimboA.Plugins.EcsProviders
{
    public abstract class MonoProvider<T> : MonoProviderBase, IConvertToEntity where T : struct 
    {
        [SerializeField] protected T value;
        private bool _isInitialized;
        
        void IConvertToEntity.Convert(int entity, EcsWorld world)
        {
            if(_isInitialized)
                return;
            
            var pool = world.GetPool<T>();
            if (pool.Has(entity))
                pool.Del(entity);
            
            pool.Add(entity) = value;
            _isInitialized = true;
        }

        private void OnDisable()
        {
            _isInitialized = false;
        }
    }
}

