using ModelData.TileMap;
using UserData.TileMap.Generators;

namespace UserData.TileMap
{
    public class UserMapLevel : UserMap
    {
        public MapLevelData LevelData => Data as MapLevelData;
        
        public UserMapLevel(MapLevelData data, TileMapGeneratorAbstract generator) : base(data, generator)
        {
        }
    }
}