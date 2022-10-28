using System.Runtime.CompilerServices;
using Client.Input;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Leopotam.EcsLite.Di;
using UnityEngine;

namespace Client.Battle.Simulation
{
    public sealed class PathCursorSystem : IEcsRunSystem
    {
        private EcsFilterInject<Inc<Turn, InputReceiver, Path, Changed<Path>, PathCursor, PathComboAttackModifier>> _onChangedPath = default;
        private EcsFilterInject<Inc<Turn, InputReceiver, Path, PathCursor, PathComboAttackModifier>, Exc<HasActiveProcess>> _actors = default;
        private EcsFilterInject<Inc<AddTargetRequest, GridPosition>> _onAddTarget = default;
        
        private EcsPoolInject<Health> _hpPool = default;
        
        private EcsCustomInject<IBoard> _board = default;
    
        public void Run(IEcsSystems systems)
        {
            ResetPowerOnTurnComplete();
            OnChangePath();
            OnAddTargetToPath();
        }

        private void ResetPowerOnTurnComplete()
        {
            foreach (var entity in _actors.Value)
            {
                ref var pools = ref _actors.Pools;

                ref Turn       turn   = ref pools.Inc1.Get(entity);
                ref PathCursor cursor = ref pools.Inc4.Get(entity);

                if (turn.Phase == StatePhase.Complete)
                {
                    cursor.CurrentPower = 0;
                    cursor.CurrentPathIndex = 0;
                }
            }
        }

        private void OnChangePath()
        {
            foreach (var entity in _onChangedPath.Value)
            {
                ref var pools = ref _onChangedPath.Pools;

                ref Path       path   = ref pools.Inc3.Get(entity);
                ref PathCursor cursor = ref pools.Inc5.Get(entity);
                
                var len = path.Positions.Length;
                if (len - 1 < cursor.CurrentPathIndex)
                {
                    cursor.CurrentPathIndex = len - 1;
                    RecalculatePower(_board.Value, in path, ref cursor);
                }
            }
        }
        
        private void OnAddTargetToPath()
        {
            foreach (var targetEntity in _onAddTarget.Value)
            {
                foreach (var actorEntity in _actors.Value)
                {
                    ref var actorsPools = ref _actors.Pools;
                    
                    ref Path       path   = ref actorsPools.Inc3.Get(actorEntity);
                    ref PathCursor cursor = ref actorsPools.Inc4.Get(actorEntity);
                    
                    cursor.CurrentPathIndex = path.Positions.Length - 1;
                    SetCurrentPower(ref cursor, targetEntity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecalculatePower(IBoard board, in Path path, ref PathCursor cursor)
        {
            var positions = path.Positions;
            cursor.CurrentPower = 0;

            for (int i = 0; i < positions.Length; i++)
            {
                var pos = positions[i];
                if (board.TryGetTarget(pos, out var targetEntity))
                {
                    SetCurrentPower(ref cursor, targetEntity);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SetCurrentPower(ref PathCursor cursor, int targetEntity)
        {
            if (_hpPool.Value.TryGet(targetEntity, out var targetHp) && targetHp.Value > 1)
                cursor.CurrentPower -= targetHp.Value;
            else
                cursor.CurrentPower++;
        }
    }
}