using ModelData.TileMap;
using UserData.TileMap.Generators;
using NUnit.Framework;
using UnityEngine;
using UserData.TileMap;
using Assert = UnityEngine.Assertions.Assert;

namespace Tests.UserData.TileMap.Generators
{
    public class TileMapGeneratorTest
    {
        [Test]
        public void TestBombGeneratorByBombPlaceCount(
            [Values(10)]int width,
            [Values(10)]int height,
            [Values(0, 91)]int bombs,
            [Values(0, 5)]int startX,
            [Values(0, 9)]int startY)
        {
            var data = new MapData
            {
                Width = width,
                Height = height,
                Bombs = bombs,
                Seed = 1,
            };
            
            var startPosition = new Vector2Int(startX, startY);
            
            var generator = new RandomTileMapGenerator();
            var userMap = new UserMap(data, generator);
            generator.Generate(userMap, startPosition);
            
            var expectedBombCount = generator.GetRealBombCount();
            Assert.AreEqual(expectedBombCount, bombs, "Генерация мин не соответствует количеству из модели");
        }



        [Test]
        public void TestBombsNear()
        {
            /*
             Поле выглядит так:
            
             0111
             13*2
             *6*3
             ***2
        
             */
            var userTileMap = DeterminedTileMapGenerator.TestSample();

            AssertBombsNear(0, 0, 0);
            AssertBombsNear(1, 0, 1);
            AssertBombsNear(1, 1, 3);
            AssertBombsNear(1, 2, 6);
            AssertBombsNear(2, 2, 0);

            void AssertBombsNear(int x, int y, int bombsNear)
            {
                var actualBombsNear = userTileMap.Field[new Vector2Int(x, y)].BombsNear;
                Assert.AreEqual(bombsNear, actualBombsNear, $"Bomb near ({x},{y}) expect: {bombsNear}, actual: {actualBombsNear}");
            }
        }
    }
}