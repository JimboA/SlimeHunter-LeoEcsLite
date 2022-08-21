using Leopotam.EcsLite;

namespace JimmboA.Plugins.EcsProviders
{
    public interface IConvertToEntity
    {
        void Convert(int entity, EcsWorld world);
    }
}
