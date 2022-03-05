using UserData.TileMap;
using UserData.TileMap.Generators;
using NUnit.Framework;
using UnityEngine;

namespace Tests.UserData.CommandSystem
{
    public class FlagCommandTest
    {
        [Test]
        public void AddFlagByTile()
        {
            /*
             Ставим флаг:
             
             #F##
             ####
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.SetFlag(new Vector2Int(1, 0), true);
            Assert.IsTrue(userTileMap[1, 0].Flagged);
        }
        
        [Test]
        public void AddFlagsByTilesAndTryUndo()
        {
            /*
             Ставим флаг:
             
             #F##
             ####
             ####
             ####
        
             */
            
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.SetFlag(new Vector2Int(0, 0), true);
            userTileMap.CommandSystem.SetFlag(new Vector2Int(1, 0), true);
            Assert.IsTrue(userTileMap[0, 0].Flagged);
            Assert.IsTrue(userTileMap[1, 0].Flagged);
            
            userTileMap.CommandSystem.Undo();
            userTileMap.CommandSystem.Undo();
            
            Assert.IsFalse(userTileMap[0, 0].Flagged);
            Assert.IsFalse(userTileMap[1, 0].Flagged);
        }

    }
}