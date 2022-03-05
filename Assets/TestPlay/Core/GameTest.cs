using Controller.Map;
using Core;
using NUnit.Framework;

public class GameTest
{
    [Test]
    public void GameTestCreateMap()
    {
        Game.Create();
        MapController.Create(1);
    }
}
