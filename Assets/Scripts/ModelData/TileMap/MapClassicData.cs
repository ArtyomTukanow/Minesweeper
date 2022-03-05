namespace ModelData.TileMap
{
    public class MapClassicData : MapData
    {
        public MapClassicData(MapDataType type)
        {
            this.type = type;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) &&
                   obj is MapClassicData;
        }

        public override string ToString()
        {
            return base.ToString() + $", ClassicType({type})";
        }
    }
}