using System.Collections.Generic;
using UnityEngine;

namespace JimmboA.Plugins.ObjectPool
{
    public class PoolContainer
    {
        private Dictionary<int, Stack<GameObject>> _cachedObjects;
        private Dictionary<int, int> _cachedIds;
        private int _capacity;

        public PoolContainer(int capacity)
        {
            _capacity = capacity;
            _cachedObjects = new Dictionary<int, Stack<GameObject>>(capacity, FastComparable.Default);
            _cachedIds = new Dictionary<int, int>(capacity, FastComparable.Default);
        }

        public void Register(GameObject prefab)
        {
            var key = prefab.GetInstanceID();
            if (!_cachedObjects.TryGetValue(key, out _))
                _cachedObjects.Add(key, new Stack<GameObject>());
        }

        internal void RegisterAndAdd(GameObject prefab, GameObject obj)
        {
            var key = prefab.GetInstanceID();
            if (!_cachedObjects.TryGetValue(key, out _))
                _cachedObjects.Add(key, new Stack<GameObject>());
            _cachedIds.Add(obj.GetInstanceID(), key);
        }

        public GameObject Spawn(GameObject prefab, Transform parent = null)
        {
            var key = prefab.GetInstanceID();
            var stacked = _cachedObjects.TryGetValue(key, out var objs);

            if (stacked && objs.Count > 0)
            {
                var obj = objs.Pop();
                var transform = obj.transform;
                if (transform.parent != parent)
                    transform.SetParent(parent);

                transform.gameObject.SetActive(true);
                return obj;
            }

            if (!stacked)
            {
                _cachedObjects.Add(key, new Stack<GameObject>(_capacity));
            }

            var createdPrefab = Object.Instantiate(prefab, parent);
            var createdKey = createdPrefab.GetInstanceID();

            _cachedIds.Add(createdKey, key);
            return createdPrefab;
        }

        public bool Spawn(GameObject prefab, out GameObject obj, Transform parent = null)
        {
            var key = prefab.GetInstanceID();
            var stacked = _cachedObjects.TryGetValue(key, out var objs);
            
            if (stacked && objs.Count > 0)
            {
                obj = objs.Pop();
                var transform = obj.transform;
                if (transform.parent != parent)
                    transform.SetParent(parent);

                transform.gameObject.SetActive(true);
                return true;
            }

            if (!stacked)
            {
                _cachedObjects.Add(key, new Stack<GameObject>(_capacity));
            }

            var createdPrefab = Object.Instantiate(prefab, parent);
            var createdKey = createdPrefab.GetInstanceID();

            _cachedIds.Add(createdKey, key);
            obj = createdPrefab;
            return false;
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var key = prefab.GetInstanceID();
            var stacked = _cachedObjects.TryGetValue(key, out var objs);

            if (stacked && objs.Count > 0)
            {
                var obj = objs.Pop();
                var transform = obj.transform;
                if (transform.parent != parent)
                    transform.SetParent(parent);
                
                transform.localPosition = position;
                transform.localRotation = rotation;
                transform.gameObject.SetActive(true);
                return obj;
            }

            if (!stacked)
            {
                _cachedObjects.Add(key, new Stack<GameObject>(512));
            }

            if (parent != null)
            {
                position = parent.TransformPoint(position);
                rotation *= parent.rotation;
            }

            var createdPrefab = Object.Instantiate(prefab, position, rotation, parent);
            var createdKey = createdPrefab.GetInstanceID();

            _cachedIds.Add(createdKey, key);
            return createdPrefab;
        }
        
        public void Recycle(GameObject go, bool resetParent = false)
        {
            if(resetParent)
                go.transform.SetParent(null);
            
            go.SetActive(false);
            _cachedObjects[_cachedIds[go.GetInstanceID()]].Push(go);
        }
    }
}