using System;

namespace UserData.TileMap
{
    public interface IMapStateViewModel
    {
        Action<UserMapState> UpdateState { set; }
        void SetUserMap(UserMap userMap);
    }
}