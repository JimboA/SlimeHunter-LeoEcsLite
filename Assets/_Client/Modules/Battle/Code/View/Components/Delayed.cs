using System.Runtime.CompilerServices;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;

namespace Client.Battle.Simulation
{
    public struct DelayedAdd<TFlag> where TFlag : struct
    {
        public EcsPackedEntity Target;
        public float TimeLeft;
    }
    
    public struct DelayedRemove<TFlag> where TFlag : struct
    {
        public EcsPackedEntity Target;
        public float TimeLeft;
    }

    public static class DelayedExtensions
    {
        public static void DelayedAdd<TFlag>(this EcsWorld world, int entity, float delay) where TFlag : struct
        {
            var delayedEntity = world.NewEntity();
            ref var delayed = ref world.Add<DelayedAdd<TFlag>>(delayedEntity);
            delayed.TimeLeft = delay;
            delayed.Target = world.PackEntity(entity);
        }
        
        public static void DelayedAdd<TFlag>(this EcsPool<DelayedAdd<TFlag>> pool, int entity, float delay) where TFlag : struct
        {
            var world = pool.GetWorld();
            var delayedEntity = world.NewEntity();
            ref var delayed = ref pool.Add(delayedEntity);
            delayed.TimeLeft = delay;
            delayed.Target = world.PackEntity(entity);
        }
        
        public static void DelayedRemove<TFlag>(this EcsWorld world, int entity, float delay) where TFlag : struct
        {
            var delayedEntity = world.NewEntity();
            ref var delayed = ref world.Add<DelayedRemove<TFlag>>(delayedEntity);
            delayed.TimeLeft = delay;
            delayed.Target = world.PackEntity(entity);
        }
        
        public static void DelayedRemove<TFlag>(this EcsPool<DelayedRemove<TFlag>> pool, int entity, float delay) where TFlag : struct
        {
            var world = pool.GetWorld();
            var delayedEntity = world.NewEntity();
            ref var delayed = ref pool.Add(delayedEntity);
            delayed.TimeLeft = delay;
            delayed.Target = world.PackEntity(entity);
        }
    }
}