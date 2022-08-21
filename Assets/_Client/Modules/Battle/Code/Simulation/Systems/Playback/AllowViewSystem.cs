using System.Runtime.CompilerServices;
using Client.AppData;
using JimmboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using Leopotam.EcsLite.ExtendedSystems;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public struct AllowViewEvent
    {
    }
    
    public sealed class AllowViewSystem : IEcsInitSystem, IEcsRunSystem
    {
        private EcsFilterInject<Inc<PlaybackEndEvent>> _onEndPlayback;
        private EcsPoolInject<EcsGroupSystemState> _groupStatePool = GlobalIdents.Worlds.EventWorldName;
        private EcsPoolInject<AllowViewEvent> _allowViewPool;

        private EcsCustomInject<BattleService> _battle;
        private EcsCustomInject<IBoard> _board;

        public void Init(IEcsSystems systems)
        {
            // if (_battle.Value.IsPlayback)
            // {
            //     SetViewSystemGroupState(systems.GetWorld(), false);
            //     _battle.Value.AllowView = false;
            // }
        }

        public void Run(IEcsSystems systems)
        {
            // if (_battle.Value.IsPlayback)
            // {
            //     SetViewSystemGroupState(false);
            // }
            
            foreach (var _ in _onEndPlayback.Value)
            {
                SetViewSystemGroupState(systems.GetWorld(), true);
                _battle.Value.AllowView = true;
                Debug.LogWarning($"ALLOW VIEW");
                //_allowViewPool.Value.SendEvent();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetViewSystemGroupState(EcsWorld world, bool state)
        {
            ref var stateEvent = ref _groupStatePool.Value.SendEvent();
            stateEvent.Name = GlobalIdents.Worlds.ViewGroupName;
            stateEvent.State = state;
        }
    }
}