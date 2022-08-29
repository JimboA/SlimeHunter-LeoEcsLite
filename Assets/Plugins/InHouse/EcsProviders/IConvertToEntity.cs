using Leopotam.EcsLite;

namespace JimboA.Plugins.EcsProviders
{
    public interface IConvertToEntity
    {
        void Convert(int entity, EcsWorld world);
    }
}
