using System;
using System.Runtime.CompilerServices;
using Leopotam.EcsLite;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace JimmboA.Plugins.FrameworkExtensions
{
    #region Interfaces

    // Used for EcsLite.Di to auto - inject filters and pools in services
    public interface IEcsService : IEcsSystem
    {
    }

    #endregion

    public delegate int EcsFilterReorderHandler (int entity);
    public static class EcsExtensions
    {
        #region HandleComponents

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this EcsWorld world, int entity) where T : struct
        {
            return ref world.GetPool<T>().Add(entity);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T Add<T>(this IEcsSystems systems, int entity) where T : struct
        {
            return ref Add<T>(systems.GetWorld(), entity);
        }

        [MethodImpl (MethodImplOptions.AggressiveInlining)]
        public static ref T Get<T>(this EcsWorld world, int entity) where T : struct
        {
            return ref world.GetPool<T>().Get(entity);
        }

        public static ref T Get<T>(this IEcsSystems systems, int entity) where T : struct
        {
            return ref Get<T>(systems.GetWorld(), entity);
        }

        public static ref T GetOrAdd<T>(this EcsWorld world, int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            if (pool.Has(entity))
                return ref pool.Get(entity);
            
            return ref pool.Add(entity);
        }
        
        public static ref T GetOrAdd<T>(this IEcsSystems systems, int entity) where T : struct
        {
            return ref GetOrAdd<T>(systems.GetWorld(), entity);
        }
        
        public static ref T GetOrAdd<T>(this EcsPool<T> pool, int entity) where T : struct
        {
            if (pool.Has(entity))
                return ref pool.Get(entity);
            
            return ref pool.Add(entity);
        }

        public static bool TryGet<T>(this EcsWorld world, int entity, out T component) where T : struct
        {
            if (world.GetPool<T>().Has(entity))
            {
                ref var c = ref world.GetPool<T>().Get(entity);
                component = c;
                return true;
            }
            component = default;
            return false;
        }

        public static bool TryGet<T>(this IEcsSystems systems, int entity, out T component) where T : struct
        {
            return TryGet<T>(systems.GetWorld(), entity, out component);
        }
        
        public static bool TryGet<T>(this EcsPool<T> pool, int entity, out T component) where T : struct
        {
            if (pool.Has(entity))
            {
                component = pool.Get(entity);
                return true;
            }
            component = default;
            return false;
        }

        #endregion

        #region EventMessages

        public static ref T SendEvent<T>(this EcsWorld world) where T : struct
        {
            var pool = world.GetPool<T>();
            var entity = world.NewEntity();
            return ref pool.Add(entity);
        }
        
        public static ref T SendEvent<T>(this EcsWorld world, out int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            entity = world.NewEntity();
            return ref pool.Add(entity);
        }
        
        public static void SendEvent<T>(this EcsWorld world, in T source) where T : struct
        {
            var pool = world.GetPool<T>();
            var entity = world.NewEntity();
            ref var value = ref pool.Add(entity);
            value = source;
        }
        
        public static void SendEvent<T>(this EcsWorld world, in T source, out int entity) where T : struct
        {
            var pool = world.GetPool<T>();
            entity = world.NewEntity();
            ref var value = ref pool.Add(entity);
            value = source;
        }
        
        public static ref T SendEvent<T>(this EcsPool<T> pool) where T : struct
        {
            var world = pool.GetWorld();
            var entity = world.NewEntity();
            return ref pool.Add(entity);
        }
        
        public static ref T SendEvent<T>(this EcsPool<T> pool, out int entity) where T : struct
        {
            var world = pool.GetWorld();
            entity = world.NewEntity();
            return ref pool.Add(entity);
        }
        
        public static void SendEvent<T>(this EcsPool<T> pool, in T source) where T : struct
        {
            var world = pool.GetWorld();
            var entity = world.NewEntity();
            ref var value = ref pool.Add(entity);
            value = source;
        }
        
        public static void SendEvent<T>(this EcsPool<T> pool, in T source, out int entity) where T : struct
        {
            var world = pool.GetWorld();
            entity = world.NewEntity();
            ref var value = ref pool.Add(entity);
            value = source;
        }

        #endregion

        #region Filters

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryGetFirst(this EcsFilter filter, out int entity)
        {
            if (filter.GetEntitiesCount() > 0)
            {
                entity = filter.GetRawEntities()[0];
                return true;
            }

            entity = -1;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(this EcsFilter filter, int entity)
        {
            return filter.GetSparseIndex()[entity] != 0;
        }
        
        // ----------------------------------------------------------------------------
        // Sortable filters. Code taken from Leopotam's "extended filters" library.
        // No longer available on GitHub, but he doesn't mind :)
        // !No thread-safe!
        // ----------------------------------------------------------------------------
        private static int[] _filterSortPool = new int[512];

        public static EcsFilter Reorder(this EcsFilter filter, EcsFilterReorderHandler cb) 
        {
            var count = filter.GetEntitiesCount ();
            if (count > 1) 
            {
                var entities = filter.GetRawEntities ();
                if (_filterSortPool.Length < entities.Length)
                    Array.Resize (ref _filterSortPool, entities.Length);
                
                for (int i = 0; i < count; i++) 
                {
                    _filterSortPool[i] = cb(entities[i]);
                }
                
                Array.Sort (_filterSortPool, entities, 0, count);
                var sparseIndex = filter.GetSparseIndex ();
                for (int i = 0; i < count; i++) 
                {
                    sparseIndex[entities[i]] = i + 1;
                }
            }
            
            return filter;
        }

        #endregion

        #region Native

        public static NativeWrappedEcsPool<TComponent> WrapToNative<TComponent>(this EcsPool<TComponent> pool) where TComponent : unmanaged
        {
            var wrap = new NativeWrappedEcsPool<TComponent>();
            wrap.Dense = NativeHelpers.WrapToNative(pool.GetRawDenseItems()).Array;
            wrap.Sparse = NativeHelpers.WrapToNative(pool.GetRawSparseItems()).Array;
            return wrap;
        }

        #endregion
    }
// -------------------------------------------------------------------------------------
// The MIT License
// Unity Jobs support for LeoECS Lite https://github.com/Leopotam/ecslite-threads-unity
// Copyright (c) 2021 Leopotam <leopotam@gmail.com>
// -------------------------------------------------------------------------------------
    public static class NativeHelpers 
    {
        public static unsafe NativeWrappedData<T> WrapToNative<T> (T[] managedData) where T : unmanaged 
        {
            fixed (void* ptr = managedData) 
            {
#if UNITY_EDITOR
                var nativeData = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (ptr, managedData.Length, Allocator.None);
                var sh = AtomicSafetyHandle.Create ();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle (ref nativeData, sh);
                return new NativeWrappedData<T> { Array = nativeData, SafetyHandle = sh };
#else
                return new NativeWrappedData<T> { Array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T> (ptr, managedData.Length, Allocator.None) };
#endif
            }
        }

#if UNITY_EDITOR
        public static void UnwrapFromNative<T1> (NativeWrappedData<T1> sh) where T1 : unmanaged 
        {
            AtomicSafetyHandle.CheckDeallocateAndThrow (sh.SafetyHandle);
            AtomicSafetyHandle.Release (sh.SafetyHandle);
        }
#endif
        public struct NativeWrappedData<TT> where TT : unmanaged 
        {
            public NativeArray<TT> Array;
#if UNITY_EDITOR
            public AtomicSafetyHandle SafetyHandle;
#endif
        }
    }

    public struct NativeWrappedEcsPool<TComponent> where TComponent : unmanaged
    {
        public NativeArray<TComponent> Dense;
        public NativeArray<int> Sparse;
    }
}
