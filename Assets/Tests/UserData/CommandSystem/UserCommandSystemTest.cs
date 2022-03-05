using UserData.TileMap;
using UserData.TileMap.Generators;
using NUnit.Framework;
using UnityEngine;

namespace Tests.UserData.CommandSystem
{
    public class UserCommandSystemTest
    {
        [Test]
        public void UndoRedoByOpenTileTest()
        {
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.Open(Vector2Int.zero);
            Assert.IsTrue(userTileMap[0, 0].Opened);
            Assert.IsTrue(userTileMap[0, 1].Opened);
            
            userTileMap.CommandSystem.Undo();
            Assert.IsFalse(userTileMap[0, 0].Opened);
            Assert.IsFalse(userTileMap[0, 1].Opened);
            
            userTileMap.CommandSystem.Redo();
            Assert.IsTrue(userTileMap[0, 0].Opened);
            Assert.IsTrue(userTileMap[0, 1].Opened);
        }
        
        [Test]
        public void UndoRedoByFlaggingTileTest()
        {
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.SetFlag(Vector2Int.zero, true);
            Assert.IsTrue(userTileMap[0, 0].Flagged);
            
            userTileMap.CommandSystem.Undo();
            Assert.IsFalse(userTileMap[0, 0].Flagged);
            
            userTileMap.CommandSystem.Redo();
            Assert.IsTrue(userTileMap[0, 0].Flagged);
        }
        
        [Test]
        public void UndoRedoByCatchBombTest()
        {
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            userTileMap.CommandSystem.CatchBomb(new Vector2Int(2, 1));
            Assert.IsTrue(userTileMap[2, 1].Opened);
            
            userTileMap.CommandSystem.Undo();
            Assert.IsFalse(userTileMap[2, 1].Opened);
            
            userTileMap.CommandSystem.Redo();
            Assert.IsTrue(userTileMap[2, 1].Opened);
        }
    }
}