using System.Collections.Generic;
using System.Linq;
using Exceptions;
using ModelData.TileMap;
using UnityEngine;
using UserData.TileMap.AntiPatterns;

namespace UserData.TileMap.Generators
{
    public class RandomTileMapGenerator : TileMapGeneratorAbstract
    {
        private int bombCounter;

        public int GetRealBombCount() => Field.Values.Count(tile => tile.Bomb);


        protected override void StartGenerate()
        {
            ThrowIfStartPosNotContainsInField();
            
            AddFreeOnStartPos();
            AddRandomBombs();
            FillToFree();
        }
        
        private void ThrowIfStartPosNotContainsInField()
        {
            if(!ContainsTile(StartPos))
                throw new InvalidMapDataException($"Start pos ({StartPos.x},{StartPos.y}) does not contain in ({Width},{Height}) field");
        }

        private void AddFreeOnStartPos()
        {
            for (var x = StartPos.x - 1; x <= StartPos.x + 1; x ++)
            for (var y = StartPos.y - 1; y <= StartPos.y + 1; y++)
                AddFree(new Vector2Int(x, y));
        }

        private void AddRandomBombs()
        {
            var freeTilesList = new List<Vector2Int>();
            for(var x = 0; x < Width; x ++)
            for (var y = 0; y < Height; y++)
            {
                var pos = new Vector2Int(x, y);
                if(!HasTileData(pos))
                    freeTilesList.Add(pos);
            }
            
            Random.InitState(UserMap.Data.Seed);

            while (bombCounter < Bombs)
            {
                if (freeTilesList.Count == 0)
                    throw new InvalidMapDataException("has no free tiles for bomb!");

                var index = Random.Range(0, freeTilesList.Count);
                var pos = freeTilesList[index];
                freeTilesList.RemoveAt(index);
                
                if (AntiPatternsSystem.Instance.IsAntiPattern(this, pos))
                    AddFree(pos);
                else
                    AddBomb(pos);
            }
        }

        // private void AddRandomBombNear(Vector2Int pos)
        // {
        //     var maxCycle = Width * Height;
        //     var counter = 0;
        //     while (HasTileData(pos))
        //     {
        //         if (counter > maxCycle)
        //             throw new InvalidMapDataException("has no free tiles for bomb!");
        //
        //         counter++;
        //         
        //         MoveNextTile(ref pos);
        //     }
        //     
        //     AddBomb(pos);
        // }
        //
        // private void MoveNextTile(ref Vector2Int pos)
        // {
        //     pos.x++;
        //     if (!ContainsTile(pos))
        //     {
        //         pos.x = 0;
        //         pos.y++;
        //         if (!ContainsTile(pos))
        //             pos.y = 0;
        //     }
        // }

        private void FillToFree()
        {
            for(var x = 0; x < Width; x ++)
            for (var y = 0; y < Height; y++)
            {
                var pos = new Vector2Int(x, y);
                if(!HasTileData(pos))
                    AddFree(pos);
            }
        }

        private void AddBomb(Vector2Int pos)
        {
            RemoveTileData(pos);
            
            if (!ContainsTile(pos))
                return;
            
            Field[pos] = new UserTile(UserMap, pos, true);
            bombCounter++;
        }

        private void AddFree(Vector2Int pos)
        {
            RemoveTileData(pos);
            
            if (!ContainsTile(pos))
                return;
            
            Field[pos] = new UserTile(UserMap, pos, false);
        }

        private void RemoveTileData(Vector2Int pos)
        {
            if (!HasTileData(pos))
                return;
            
            if (Field[pos].Bomb)
                bombCounter--;

            Field.Remove(pos);
        }
    }
}