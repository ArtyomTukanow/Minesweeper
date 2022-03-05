using System.Collections.Generic;
using System.Linq;
using Exceptions;
using Libraries.RSG;
using Newtonsoft.Json;
using UserData.TileMap;
using UnityEngine;
using Utils;

namespace CommandSystem.Commands
{
    public class OpenCommand : UserCommandBase, ITilesCommand
    {
        public const string TYPE = "o";
        public override string Type => TYPE;
        
        private Vector2Int[] openedTiles;
        private Vector2Int startPos;

        [JsonIgnore]
        public Vector2Int[] ChangedTiles => openedTiles;

        public OpenCommand(string[] data) : base(data)
        {
            
        }

        public OpenCommand(UserMap userMap, Vector2Int[] startPoses) : base(userMap)
        {
            startPos = startPoses.FirstOrDefault();
            UserMap.TryGenerateField(startPos);
            
            foreach (var pos in startPoses)
            {
                if (!UserMap.ContainsTile(pos))
                    throw new InvalidCommandException(this, $"start pos {pos} doesn't contains in tilemap");
                
                if (UserMap[pos].Bomb)
                    throw new InvalidCommandException(this, "need use CatchBombCommand");
            
                if (UserMap[pos].Opened)
                    throw new InvalidCommandException(this, "already opened");
            }
            
            OpenedTilesCalculate(startPoses);
        }

        public override void Redo()
        {
            UserMap.TryGenerateField(new Vector2Int(startPos.x, startPos.y));
            openedTiles.Each(p => UserMap[p].Opened = true);
        }

        public override void Undo()
        {
            openedTiles.Each(p => UserMap[p].Opened = false);
        }


        private void OpenedTilesCalculate(Vector2Int[] startPoses)
        {
            var openedTilesList = new List<Vector2Int>();
            PathFinder.getWave(startPoses, NeedOpenNext, true, true);
            openedTiles = openedTilesList.ToArray();

            bool NeedOpenNext(Vector2Int pos, int iteration)
            {
                if (!UserMap.ContainsTile(pos))
                    return false;

                var tile = UserMap[pos];

                if (tile.Bomb || tile.Opened)
                    return false;
                
                openedTilesList.Add(pos);

                return !tile.HasBombsNear;
            }
        }
        
        private int TypeInArrayCount => 1;
        private int StartInArrayCount => 2;
        private int OpenedInArrayCount => openedTiles.Length * 2;

        private int ArrayCount => TypeInArrayCount + StartInArrayCount + OpenedInArrayCount;
        
        public override string Serialize()
        {
            object[] data = new object[ArrayCount];
            data[0] = Type;
            
            var index = 1;
            AddPos(startPos);
            foreach (var openedTile in openedTiles)
                AddPos(openedTile);

            return string.Join(SEPARATOR.ToString(), data);
            
            void AddPos(Vector2Int pos)
            {
                data[index++] = pos.x;
                data[index++] = pos.y;
            }
        }

        public override void Deserialize(string[] data)
        {
            var index = 1;

            startPos = getPos();

            var tilesLength = (data.Length - index) / 2;
            openedTiles = new Vector2Int[tilesLength];
            for (var i = 0; i < openedTiles.Length; i++)
                openedTiles[i] = getPos();

            Vector2Int getPos()
            {
                return new Vector2Int(int.Parse(data[index++]), int.Parse(data[index++]));
            }
        }
    }
}