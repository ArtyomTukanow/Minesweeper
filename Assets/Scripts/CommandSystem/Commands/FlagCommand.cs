using System.Collections.Generic;
using Exceptions;
using Newtonsoft.Json;
using UnityEngine;
using UserData.TileMap;

namespace CommandSystem.Commands
{
    public class FlagCommand : UserCommandBase, ITilesCommand
    {
        public const string TYPE = "f";
        public override string Type => TYPE;
        
        private Vector2Int[] flagPoses;
        private bool flaged;

        [JsonIgnore]
        public Vector2Int[] ChangedTiles => flagPoses;
        
        [JsonIgnore]
        private Dictionary<Vector2Int, bool> WasFlagged = new Dictionary<Vector2Int, bool>();
        

        public FlagCommand(string[] data) : base(data)
        {
            
        }
        
        public FlagCommand(UserMap userMap, Vector2Int[] flagPoses, bool flaged) : base(userMap)
        {
            foreach (var flagPos in flagPoses)
            {
                if (!UserMap.ContainsTile(flagPos))
                    throw new InvalidCommandException(this, $"flagPos {flagPos} doesn't contains in tilemap");
            
                if (UserMap[flagPos].Opened)
                    throw new InvalidCommandException(this, "already opened");
            }
            
            this.flaged = flaged;
            this.flagPoses = flagPoses;
        }

        public override void Redo()
        {
            foreach (var pos in flagPoses)
            {
                WasFlagged[pos] = UserMap[pos].Flagged;
                UserMap[pos].Flagged = flaged;
            }
        }

        public override void Undo()
        {
            foreach (var pos in flagPoses)
                UserMap[pos].Flagged = WasFlagged[pos];
        }
        
        
        
        private int TypeInArrayCount => 1;
        private int FlaggedInArrayCount => 1;
        private int FlagsInArrayCount => flagPoses.Length * 2;

        private int ArrayCount => TypeInArrayCount + FlaggedInArrayCount + FlagsInArrayCount;
        
        public override string Serialize()
        {
            object[] data = new object[ArrayCount];
            data[0] = Type;
            data[1] = flaged ? 1 : 0;
            
            var index = 2;
            foreach (var flagPos in flagPoses)
                AddPos(flagPos);

            return string.Join(SEPARATOR.ToString(), data);
            
            void AddPos(Vector2Int pos)
            {
                data[index++] = pos.x;
                data[index++] = pos.y;
            }
        }

        public override void Deserialize(string[] data)
        {
            flaged = data[1] == "1";
            var index = 2;

            var tilesLength = (data.Length - index) / 2;
            flagPoses = new Vector2Int[tilesLength];
            for (var i = 0; i < flagPoses.Length; i++)
                flagPoses[i] = getPos();

            Vector2Int getPos()
            {
                return new Vector2Int(int.Parse(data[index++]), int.Parse(data[index++]));
            }
        }
    }
}