using System;
using UnityEngine;
using UserData.TileMap;

namespace ModelData.TileMap
{
    public class UserTile
    {
        public bool Bomb { get; private set; }
        public Vector2Int Pos { get; }
        public int BombsNear { get; private set; }
        public bool Opened { get; set; }
        
        public bool IsOpened => (Opened || UserMap.State == UserMapState.Complete && !Bomb);
        public bool IsFlagged => (Flagged || UserMap.State == UserMapState.Complete && Bomb);

        private bool flagged;
        public bool Flagged 
        { 
            get => !Opened && flagged;
            set => flagged = value;
        }

        public bool HasBombsNear => BombsNear != default;

        public UserMap UserMap { get; }

        public UserTile(UserMap userMap, Vector2Int pos, bool bomb)
        {
            UserMap = userMap;
            Pos = pos;
            Bomb = bomb;
        }

        public UserTile(UserMap userMap, int x, int y, bool bomb)
        {
            UserMap = userMap;
            Pos = new Vector2Int(x, y);
            Bomb = bomb;
        }

        public void SetBombsNear(int number)
        {
            if(Bomb)
                throw new ArgumentException(ToString() + " Already has bomb");
            if(BombsNear != default)
                throw new ArgumentException(ToString() + " Already has bombs near");

            BombsNear = number;
        }

        public override string ToString()
        {
            return $"[{Pos.x}, {Pos.y}], Bomb:{Bomb}, Number:{BombsNear}";
        }
    }
}