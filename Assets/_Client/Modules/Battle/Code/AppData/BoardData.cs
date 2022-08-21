using System.Collections;
using System.Collections.Generic;
using Client.Input;
using Leopotam.EcsLite;
using UnityEngine;
using System;
using Client.Battle.Simulation;
using Unity.Mathematics;
using UnityEngine.Serialization;

namespace Client.AppData
{
    [Serializable]
    public struct BoardPiece
    {
        public int2       Position;
        public Elements   Element;
        public string     PrefabName;
    }
    
    // used in the level editor (in progress)
    [Serializable]
    public sealed class BoardData
    {
        public int Columns;
        public int Rows;
        public int ZOffset;
        
        [HideInInspector] public List<BoardPiece> Pieces;
        [HideInInspector] public List<BoardPiece> Monsters;
        [HideInInspector] public List<BoardPiece> Blocks;
        [HideInInspector] public List<BoardPiece> Items;
    }
}