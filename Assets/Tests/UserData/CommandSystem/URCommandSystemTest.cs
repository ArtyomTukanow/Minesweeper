using UserData.TileMap;
using UserData.TileMap.Generators;
using NUnit.Framework;
using UnityEngine;

namespace Tests.UserData.CommandSystem
{
    public class URCommandSystemTest
    {
        [Test]
        public void RedoOnceAndUndoTwiceWithoutErrors()
        {
            var userTileMap = DeterminedTileMapGenerator.TestSample();
            //Открываем (0, 0) и дважды отменяем. Повторяение не должно произойти второй раз.
            userTileMap.CommandSystem.Open(Vector2Int.zero);
            
            userTileMap.CommandSystem.Undo();
            userTileMap.CommandSystem.Undo();
            
            Assert.Pass("Twice undo completed");
        }
        
        [Test]
        public void RedoTwiceWithoutErrors()
        {
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            //Открываем (0, 0), оменяем это окрытие и дважды повторяем. Повторяение не должно произойти второй раз.
            userTileMap.CommandSystem.Open(Vector2Int.zero);
            
            userTileMap.CommandSystem.Undo();
            
            userTileMap.CommandSystem.Redo();
            userTileMap.CommandSystem.Redo();
            
            Assert.IsTrue(userTileMap[Vector2Int.zero].Opened);
        }
        
        [Test]
        public void UndoAndExecuteOtherOperation()
        {
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            //Открываем (0, 0), оменяем это окрытие и открываем (1, 1). Пробуем вернуть открытие (0, 0), не должно получиться. (0, 0) закрыта.
            userTileMap.CommandSystem.Open(Vector2Int.zero);
            userTileMap.CommandSystem.Undo();
            
            userTileMap.CommandSystem.Open(Vector2Int.one);
            userTileMap.CommandSystem.Redo();
            
            Assert.IsFalse(userTileMap[Vector2Int.zero].Opened);
        }
    }
}