using Client.AppData;
using Client.Battle.Simulation;
using UnityEngine;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using JimmboA.Plugins.FrameworkExtensions;

namespace Client.Input
{
    public sealed class BattleInputSystem : IEcsInitSystem, IEcsRunSystem, IEcsDestroySystem
    {
        private EcsFilterInject<Inc<InputTouch>>  _touches = GlobalIdents.Worlds.EventWorldName;
        
        private EcsPoolInject<InputTouch> _touchPool = GlobalIdents.Worlds.EventWorldName;
        private EcsPoolInject<InputTouchStartedEvent>  _touchStartedPool = GlobalIdents.Worlds.EventWorldName;
        private EcsPoolInject<InputTouchCanceledEvent> _touchCanceledPool = GlobalIdents.Worlds.EventWorldName;

        private EcsCustomInject<BattleService> _battle = default;
        
        private InputControls _controls;
        private InputAction   _touchPosAction;

        public void Init (IEcsSystems systems) 
        {
            _controls = new InputControls();
            _controls.Enable();
#if UNITY_EDITOR
            TouchSimulation.Enable();
#endif
            _touchPosAction = _controls.Battle.TouchPosition;
            _controls.Battle.TouchPressed.started += TouchStarted;
            _controls.Battle.TouchPressed.canceled += TouchCanceled;
        }

        public void Run(IEcsSystems systems)
        {
            if(_battle.Value.BlockInput)
                return;
            
            foreach (var entity in _touches.Value)
            {
                ref InputTouch touch = ref _touches.Pools.Inc1.Get(entity);
                touch.ScreenPosition = _touchPosAction.ReadValue<Vector2>();
            }
        }

        private void TouchStarted(InputAction.CallbackContext context)
        {
            _touchStartedPool.Value.SendEvent(out int entity);
            ref InputTouch touch = ref _touchPool.Value.Add(entity);
            touch.ScreenPosition = _touchPosAction.ReadValue<Vector2>();
        }

        private void TouchCanceled(InputAction.CallbackContext context)
        {
            foreach (var entity in _touches.Value)
            {
                _touches.Pools.Inc1.Del(entity);
            }
            _touchCanceledPool.Value.SendEvent();
        }

        public void Destroy(IEcsSystems systems)
        {
            _touchPosAction = _controls.Battle.TouchPosition;
            _controls.Battle.TouchPressed.started -= TouchStarted;
            _controls.Battle.TouchPressed.canceled -= TouchCanceled;
            _controls.Disable(); 
            _controls.Dispose(); 
            _controls = null;
            _touchPosAction = null;
        }
    }
}