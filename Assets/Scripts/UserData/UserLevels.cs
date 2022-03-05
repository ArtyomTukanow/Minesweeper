using System;
using Core;
using ModelData.TileMap;

namespace UserData
{
    public class UserLevels
    {
        public int GetLockLevel(MapDataType mapType)
        {
            return mapType switch
            {
                MapDataType.level => 0,
                MapDataType.beginner => 0,
                MapDataType.intermediate => 20,
                MapDataType.expert => 80,
                MapDataType.guru => 200,
                MapDataType.master => 400,
                MapDataType.grandmaster => 800,
                _ => throw new ArgumentOutOfRangeException(nameof(mapType), mapType, null)
            };
        }
        
        public bool IsLock(MapDataType mapType)
        {
            if (Game.User.Settings.Vip)
                return false;
            return Game.User.Level <= GetLockLevel(mapType);
        }
    }
}