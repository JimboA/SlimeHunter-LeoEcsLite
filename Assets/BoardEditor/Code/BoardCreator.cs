using System;
using System.Collections.Generic;
using Client.AppData;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BoardEditor
{
    /// <summary>
    /// Battle board creator. Generates a board from unity tilemap (tilemap is used in Editor only). Not finished yet.
    /// </summary>
    public class BoardCreator : MonoBehaviour
    {
        [SerializeField] private LevelData _curretLevel;
        [SerializeField] private Grid _grid;
        [SerializeField] private Tilemap _cells;
        [SerializeField] private Tilemap _items;
        [SerializeField] private Tilemap _characters;
        [SerializeField] private Tilemap _blocks;

        public void GenerateBoard()
        {
            var data = _curretLevel.Board;
            SetBoardPieces(_cells, data.Cells);
            SetBoardPieces(_characters, data.Characters);
            SetBoardPieces(_items, data.Items);
            SetBoardPieces(_blocks, data.Blocks);
        }

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        private void OnDrawGizmos()
        {
            DrawBoardBounds();
        }

        private void SetBoardPieces(Tilemap tilemap, List<BoardPiece> pieces)
        {
            tilemap.CompressBounds();
            pieces.Clear();
            BoundsInt bounds = tilemap.cellBounds;
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    var tile = tilemap.GetTile(pos) as BlueprintTile;
                    if (tile == null)
                    {
                        throw new Exception($"tile {tile} is not correct type. Should be {typeof(BlueprintTile)}");
                    }
                    BoardPiece piece;
                    piece.Position      = new int2(pos.x, pos.y);
                    piece.Name          = tile.blueprint.name;
                    piece.WorldPosition = pos;
                    pieces.Add(piece);
                }
            }
        }

        public bool CheckBoardBounds(Vector3Int position)
        {
            var data = _curretLevel.Board;
            return !(position.x < 0 || position.x > data.Columns - 1 || position.y < 0 || position.y > data.Rows - 1);
        }

        private void DrawBoardBounds()
        {
            var data = _curretLevel.Board;
            var columns = data.Columns;
            var rows = data.Rows;
            
            Vector3Int size = new Vector3Int(columns, rows, 0);
            Vector3 center = new Vector3(columns * 0.5f , rows * 0.5f, 0) + _grid.transform.position;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(center, size);
            Gizmos.color = Color.white;
        }
        
    }
}