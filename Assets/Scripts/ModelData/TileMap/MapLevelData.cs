using Exceptions;
using Newtonsoft.Json;

namespace ModelData.TileMap
{
    public class MapLevelData : MapData
    {
        [JsonProperty("level")]
        public int Level { get; set; }

        public MapLevelData()
        {
            type = MapDataType.level;
        }
        
        public override void ThrowIfNotValid()
        {
            base.ThrowIfNotValid();
            
            if(Level <= 0)
                throw new InvalidMapDataException($"MapData.Level({Level} must be > 0");
        }
        

        public override bool Equals(object obj)
        {
            return base.Equals(obj) &&
                   obj is MapLevelData data &&
                   data.Level == Level;
        }

        public override string ToString()
        {
            return base.ToString() + $", Level({Level})";
        }
    }
}