using ModelData.TileMap;
using UserData.Level;
using UserData.TileMap.Generators;

namespace UserData.TileMap
{
    public class UserMapClassic : UserMap
    {
        public MapClassicData ClassicData => Data as MapClassicData;
        
        public UserLevelTimer Timer { get; private set; }
        
        public UserMapClassic(MapClassicData data, TileMapGeneratorAbstract generator) : base(data, generator)
        {
            Timer = new UserLevelTimer(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            Timer?.Dispose();
        }
    }
}