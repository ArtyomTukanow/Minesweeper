using Exceptions;
using Newtonsoft.Json;

namespace ModelData.TileMap
{
    public class MapData
    {
        public const int NEED_TILES_FOR_OPEN_BY_FIRST_TAP = 9;
        
        [JsonProperty("t")] public MapDataType type;
        
        [JsonProperty("w")] public int Width;
        [JsonProperty("h")] public int Height;
        [JsonProperty("f")] public int Bombs;
        [JsonProperty("s")] public int Seed;
        [JsonProperty("hue")] public float Hue;

        public virtual void ThrowIfNotValid()
        {
            if(Width <=0)
                throw new InvalidMapDataException($"MapData.Width({Width} must be > 0");
            if(Height <= 0)
                throw new InvalidMapDataException($"MapData.Height({Height}) must be > 0");
            if(Bombs > Width * Height - NEED_TILES_FOR_OPEN_BY_FIRST_TAP)
                throw new InvalidMapDataException($"Bombs count({Bombs}) must be less than Width({Width}) * Height({Height}) - " + NEED_TILES_FOR_OPEN_BY_FIRST_TAP);
            if(Seed == default)
                throw new InvalidMapDataException($"MapData.Seed({Seed} must be != 0");
        }

        public override bool Equals(object obj)
        {
            return obj is MapData data 
                   && data.Width == Width 
                   && data.Height == Height 
                   && data.Bombs == Bombs &&
                   data.Seed == Seed &&
                   data.type == type;
        }

        public override string ToString()
        {
            return $"Width({Width}), Height({Height}), Bombs({Bombs}), Seed({Seed})";
        }
    }
}