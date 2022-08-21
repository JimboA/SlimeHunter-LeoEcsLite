using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Client.Input
{
    // we don't need to save "input" itself, rather we can save the "actions" of players that affect the battle.
    // For now it's just player's path, but in the future it could be abilities, using consumables etc
    public enum PlayerActionsTypes
    {
        Path
    }

    public interface IInputMarker
    {
        public PlayerActionsTypes ActionType { get; set; }
    }
}
