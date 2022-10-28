using System.Runtime.CompilerServices;
using AOT;
using JimboA.Plugins.FrameworkExtensions;
using Leopotam.EcsLite;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Client.Battle.Simulation
{
    /* Logic to check if it's possible to move into the cell depending on the "Movable" component settings.
     Because this logic may be needed in many places(player movement, ai pathfinding etc.), I moved it to the service class. 
     It was possible to put it in the Board class, but I didn’t want to drag a bunch of pools not related to the Board.
     Same for the extension method - you need to cache the pools. Well, not necessary, but preferred.
     */
    // TODO: anyway it looks bad :) will be refactored
    public class BoardMovementHelpers
    {
        public EcsPool<Health> HpPool;
        public EcsPool<Level> LevelPool;
        public EcsPool<AttackPower> PowerPool;
        public EcsPool<Element> ElementPool;
        public IBoard Board;
        public EcsWorld World;

        public BoardMovementHelpers(EcsWorld world, IBoard board)
        {
            HpPool = world.GetPool<Health>();
            LevelPool = world.GetPool<Level>();
            PowerPool = world.GetPool<AttackPower>();
            ElementPool = world.GetPool<Element>();
            Board = board;
            World = world;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (bool isMovable, bool withSwap, bool withAttack) IsMovable(int2 to, in Movable movable,
            int entity, Elements currentElement, int currentPower)
        {
            return IsMovable(in Board.GetCellDataFromPosition(to), in movable, entity, currentElement, currentPower);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (bool isMovable, bool withSwap, bool withAttack) IsMovable(in Cell cellTo, in Movable movable,
            int entity, Elements currentElement, int currentPower)
        {
            bool isMovable = false;
            bool withSwap = false;
            bool withAttack = false;
            
            if (cellTo.Target.Unpack(World, out var targetEntity))
            {
                if (movable.CanAttackWhileMoving)
                {
                    if (CheckElement(currentElement, targetEntity, in movable)
                        && TryAttack(entity, targetEntity, currentPower))
                    {
                        isMovable = true;
                        withAttack = true;
                    }
                }
                else if(movable.CanSwap && TrySwap(entity, targetEntity))
                {
                    isMovable = true;
                    withSwap = true;
                }
            }
            else
            {
                isMovable = true;
            }

            return (isMovable, withSwap, withAttack);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckElement(Elements element, int targetEntity, in Movable movable)
        {
            if (ElementPool.TryGet(targetEntity, out var targetElement))
            {
                if (movable.CanMoveOnlyToEqualElements)
                {
                    if(element != Elements.None && element != targetElement.Type)
                        return false;
                }
                else if(!movable.AllowedElements.HasElement(targetElement.Type))
                {
                    return false;
                }
            }

            return true;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TryAttack(int entity, int targetEntity, int currentPower)
        {
            if (HpPool.TryGet(targetEntity, out var targetHp)
                && currentPower < targetHp.Value)
            {
                return false;
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool TrySwap(int entity, int targetEntity)
        {
            if(LevelPool.TryGet(entity, out var level) 
               && LevelPool.TryGet(targetEntity, out var targetLevel)
               && level.Value <= targetLevel.Value)
            {
                return false;
            }

            return true;
        }
    }
}