using System.Collections.Generic;
using System.Security;
using ModelData.TileMap;
using UnityEngine;
using Utils;

namespace UserData.TileMap.Generators
{
    public abstract class TileMapGeneratorAbstract
    {
        public Dictionary<Vector2Int, UserTile> Field { get; } = new Dictionary<Vector2Int, UserTile>();
        
        public bool IsGenerated { get; protected set; }
        public Vector2Int StartPos { get; private set; }
        
        protected UserMap UserMap;

        protected int Width => UserMap.Width;
        protected int Height => UserMap.Height;
        protected int Bombs => UserMap.Bombs;

        public virtual void Generate(UserMap map, Vector2Int startPos)
        {
            ThrowIfGenerated();
            StartPos = startPos;
            UserMap = map;
            
            StartGenerate();
            FillTilesNumbers();
            IsGenerated = true;
        }

        private void ThrowIfGenerated()
        {
            if(IsGenerated)
                throw new VerificationException("Already generated");
        }

        protected abstract void StartGenerate();
        


        protected bool HasTileData(Vector2Int pos) => Field.ContainsKey(pos);
        public bool ContainsTile(Vector2Int pos) => pos.x >= 0 && pos.y >= 0 && pos.x < Width && pos.y < Height;



        private void FillTilesNumbers()
        {
            for (var x = 0; x < Width; x++)
            for (var y = 0; y < Height; y++)
            {
                var pos = new Vector2Int(x, y);
                if(Field[pos].Bomb)
                    continue;
                var bombsNear = GetTileBombsNear(pos);
                Field[pos].SetBombsNear(bombsNear);
            }
        }

        private int GetTileBombsNear(Vector2Int tilePos)
        {
            var count = 0;
            PathFinder.getWave(tilePos, (pos, iteration) =>
            {
                if (Field.ContainsKey(pos) && Field[pos].Bomb)
                    count++;
                return false;
            }, true);
            return count;
        }
    }
}