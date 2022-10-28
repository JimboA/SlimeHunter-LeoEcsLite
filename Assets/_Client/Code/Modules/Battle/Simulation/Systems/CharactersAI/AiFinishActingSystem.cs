using Client.Input;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;

namespace Client.Battle.Simulation
{
    public sealed class AiFinishActingSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, CanAct>,  Exc<InputReceiver>> _ai = default;
        
        public void Run(IEcsSystems systems)
        {
            foreach (var entity in _ai.Value)
            {
                ref Turn turn = ref _ai.Pools.Inc1.Get(entity);
                if (turn.Phase == StatePhase.OnStart)
                    turn.Phase = StatePhase.Complete;
            }
        }
    }
}