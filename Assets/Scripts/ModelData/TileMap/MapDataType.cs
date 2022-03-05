namespace ModelData.TileMap
{
    public enum MapDataType
    {
        level = 0, 
        beginner = 1,
        intermediate = 2,
        expert = 3,
        guru = 4,
        master = 5,
        grandmaster = 6,
        //noflag
    }

    public static class MapDataTypes
    {
        public const int MAX_TYPE = (int)MapDataType.grandmaster;

        public static bool IsTimer(this MapDataType type)
        {
            return type >= MapDataType.beginner && type <= MapDataType.grandmaster;
        }

        public static MapDataType NextTimer(ref this MapDataType type)
        {
            do type.Next(); 
            while (!type.IsTimer());
            return type;
        }

        public static MapDataType PrevTimer(ref this MapDataType type)
        {
            do type.Prev(); 
            while (!type.IsTimer());
            return type;
        }

        public static MapDataType Next(ref this MapDataType type)
        {
            var mapTypeInt = (int) type;
            mapTypeInt++;

            if (mapTypeInt > MAX_TYPE)
                mapTypeInt = 0;
            
            return type = (MapDataType)mapTypeInt;
        }

        public static MapDataType Prev(ref this MapDataType type)
        {
            var mapTypeInt = (int) type;
            mapTypeInt--;

            if (mapTypeInt < 0)
                mapTypeInt = MAX_TYPE;
            
            return type = (MapDataType)mapTypeInt;
        }
    }
}