using System;
using ModelData.TileMap;
using UserData.Level;
using NUnit.Framework;

namespace Tests.UserData.Level
{
    public class GameBySeedGeneratorTest
    {
        [Test]
        public void GenerateLevelsWithoutExceptions()
        {
            var level = 0;
            MapData model = null;
            try
            {
                while (level++ < 1000)
                {
                    model = new GameBySeedGenerator(level).CreateGameData();
                    model.ThrowIfNotValid();
                }
            }
            catch (Exception e)
            {
                Assert.Fail($"Level: {level}\nModelData: {model}\nException: {e.Message}");
                throw e;
            }
        }
    }
}