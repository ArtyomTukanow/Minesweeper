using System;
using Assets.Scripts.UI.Utils;
using UnityEngine;
using UnityEngine.Tilemaps;
using UserData.TileMap;

namespace View.Map
{
    public class TileMatrixView : MonoBehaviour
    {
        [SerializeField] public Tilemap Tilemap;
        
        public UserMap UserMap { get; private set; }

        public Vector3 CellSize => Tilemap.cellSize;
        
        
        public void SetUserData(UserMap userMap)
        {
            UserMap = userMap;

            RedrawTiles();
        }

        private void RedrawTiles()
        {
            Tilemap.ClearAllTiles();
            
            for(var x = 0; x < UserMap.Width; x ++)
            for (var y = 0; y < UserMap.Height; y++)
                Tilemap.SetTile(new Vector3Int(x, y, 0), BasePrefabs.Instance.tileNone);
        }
        
        

        public Vector3 GetCellPivotWorld(Vector2Int cell) => GetCellTopWorld(cell);

        public Vector3 GetCellCenterWorld(Vector2Int cell) =>
            Tilemap
                ? Tilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0))
                : Vector3.zero;

        public Vector3 GetCellTopWorld(Vector2Int cell)
        {
            if (!Tilemap)
                return Vector3.zero;

            Vector3 result = Tilemap.GetCellCenterWorld(new Vector3Int(cell.x, cell.y, 0));
            result.y += 0.5f * CellSize.y;
            return result;
        }

        public Vector2Int WorldToCell(Vector3 point)
        {
            point.z = 0;
            Vector3Int worldCell = Tilemap.WorldToCell(point);
            return new Vector2Int(worldCell.x, worldCell.y);
        }

        public Vector2 CellToWorld(Vector2 vector)
        {
            Vector3 result = Tilemap.CellToWorld(new Vector3Int((int)vector.x, (int)vector.y, 0));
            result.x += (vector.x - (int)vector.x) * CellSize.x;
            result.y += (vector.y - (int)vector.y) * CellSize.y;
            return result;
        }

        public Vector2 CellToWorld(Vector3Int vector)
        {
            return Tilemap.CellToWorld(vector).ToVector2();
        }

        protected (int, int, int, int) FindTilemapBounds(Tilemap tilemap)
        {
            int maxX = Int32.MinValue;
            int maxY = Int32.MinValue;
            int minX = Int32.MaxValue;
            int minY = Int32.MaxValue;

            foreach (Vector3Int tile in tilemap.cellBounds.allPositionsWithin)
            {
                maxX = Math.Max(maxX, tile.x);
                maxY = Math.Max(maxY, tile.y);
                minX = Math.Min(minX, tile.x);
                minY = Math.Min(minY, tile.y);
            }

            return (minX, maxX, minY, maxY);
        }

        public bool IsTileExist(Vector2Int tilePos) => UserMap.ContainsTile(tilePos);
    }
}