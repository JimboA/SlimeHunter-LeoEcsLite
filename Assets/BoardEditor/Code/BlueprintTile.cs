using System.Collections;
using System.Collections.Generic;
using Client.AppData.Blueprints;
using Client.Battle.Simulation;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BoardEditor
{
    [CreateAssetMenu(fileName = "newBlueprintTile", menuName = "Game/BoardCreator/BlueprintTile")]
    public class BlueprintTile : Tile
    {
        public Blueprint blueprint;
    }
}
