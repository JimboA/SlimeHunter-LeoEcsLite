using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Mathematics;

namespace Client.AppData
{
    [Serializable]
    public struct BoardPiece
    {
        public int2    Position;
        public Vector3 WorldPosition;
        public string  Name;
    }
    
    [Serializable]
    public sealed class BoardData
    {
        public int Columns;
        public int Rows;
        public int ZOffset;
        
        public List<BoardPiece> Cells;
        public List<BoardPiece> Characters;
        public List<BoardPiece> Blocks;
        public List<BoardPiece> Items;
    }
}