using System.Runtime.CompilerServices;
using Client.AppData;
using Client.Battle.Simulation;
using JimmboA.Plugins.EcsProviders;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Input
{
    [System.Serializable]
    [GenerateDataProvider]
    public struct InputReceiver : IEcsAutoReset<InputReceiver>
    {
        public EcsPackedEntity Selected;
        public RaycastHit2D[] Hits;

        public void AutoReset(ref InputReceiver c)
        {
            c.Selected = new EcsPackedEntity();
            c.Hits = new RaycastHit2D[1];
        }
    }

    public sealed class TargetSelectingSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private EcsFilterInject<Inc<InputReceiver, Turn>> _players = default;
        private EcsFilterInject<Inc<InputTouch>> _touches = GlobalIdents.Worlds.EventWorldName;
        
        private EcsPoolInject<IsInteractable> _poolInteractable = default;
        private EcsPoolInject<Selected> _poolSelected = default;
        private EcsPoolInject<AddTargetRequest> _poolAddTArget = default;

        private EcsCustomInject<BattleSceneData> _sceneData = default;
        private EcsCustomInject<BattleService> _context = default;

        // local cache
        private Camera _camera;

        public void Init(IEcsSystems systems)
        {
            _camera = _sceneData.Value.BattleCameraProvider.GetComponent<Camera>();
        }

        public void Run(IEcsSystems systems)
        {
            if(_context.Value.BlockInput || _context.Value.Phase != BattlePhase.Battle) 
                return;

            foreach (var playerEntity in _players.Value)
            {
                ref var pools = ref _players.Pools;
                ref Turn turn = ref pools.Inc2.Get(playerEntity);
                if(turn.Phase != StatePhase.OnStart)
                    return;

                ref InputReceiver inputReceiver = ref pools.Inc1.Get(playerEntity);
                
                foreach (var touchEntity in _touches.Value)
                {
                    var world = systems.GetWorld();
                    ref InputTouch touch = ref _touches.Pools.Inc1.Get(touchEntity);
                    if(!TryGetInteractableEntity(touch.ScreenPosition, out var interactable, inputReceiver.Hits))
                        continue;

                    if (!inputReceiver.Selected.Unpack(world, out var selected))
                    {
                        Select(world, interactable, ref inputReceiver);
                    }
                    else if(selected != interactable)
                    {
                        UnSelect(world, ref inputReceiver);
                        Select(world, interactable, ref inputReceiver);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Select(EcsWorld world, int entity, ref InputReceiver inputReceiver)
        {
            _poolSelected.Value.Add(entity);
            _poolAddTArget.Value.Add(entity);
            inputReceiver.Selected = world.PackEntity(entity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnSelect(EcsWorld world, ref InputReceiver inputReceiver)
        {
            if (inputReceiver.Selected.Unpack(world, out int entity))
            {
                _poolSelected.Value.Del(entity);
                inputReceiver.Selected = default;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryGetInteractableEntity(Vector2 pos, out int entity, RaycastHit2D[] hits)
        {
            hits ??= new RaycastHit2D[1]; 
            entity = -1;
            if (Physics2D.RaycastNonAlloc(_camera.ScreenToWorldPoint(pos), Vector2.zero,
                hits, 100) > 0)
            {
                var provider = hits[0].collider.GetComponent<EntityProvider>();
                if (provider == null) 
                    return false;
                
                if (!provider.TryGetEntity(out entity))
                    return false;

                if (_poolInteractable.Value.Has(entity))
                    return true;
            }

            return false;
        }

        public void Destroy(IEcsSystems systems)
        {
            _camera = null;
        }
    }
}