using Exceptions;
using Libraries.RSG;
using UnityEngine;
using UserData.TileMap;

namespace CommandSystem.Commands
{
    public class CatchBombCommand : UserCommandBase, ITilesCommand
    {
        public const string TYPE = "b";
        public override string Type => TYPE;

        public Vector2Int[] BombPoses { get; private set; }
        public Vector2Int[] ChangedTiles => BombPoses;

        public CatchBombCommand(string[] data) : base(data)
        {
            
        }

        public CatchBombCommand(UserMap userMap, Vector2Int[] bombPoses) : base(userMap)
        {
            UserMap = userMap;
            BombPoses = bombPoses;
            
            foreach (var bombPos in bombPoses)
            {
                if (!userMap.ContainsTile(bombPos))
                    throw new InvalidCommandException(this, $"bombPos {bombPos} doesn't contains in tilemap");
            
                if (userMap[bombPos].Opened)
                    throw new InvalidCommandException(this, $"bombPos {bombPos} already opened");
            
                if (!userMap[bombPos].Bomb)
                    throw new InvalidCommandException(this, $"bombPos {bombPos} doesn't contains the bomb");
            }
        }

        public override void Redo()
        {
            BombPoses.Each(pos => UserMap[pos].Opened = true);
        }
        
        public override void Undo()
        {
            BombPoses.Each(pos => UserMap[pos].Opened = false);
        }
        
        
        private int TypeInArrayCount => 1;
        private int FlaggedInArrayCount => 1;
        private int BombsInArrayCount => BombPoses.Length * 2;

        private int ArrayCount => TypeInArrayCount + FlaggedInArrayCount + BombsInArrayCount;

        public override string Serialize()
        {
            object[] data = new object[ArrayCount];
            data[0] = Type;
            
            var index = 1;
            foreach (var flagPos in BombPoses)
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
            var index = 1;

            var tilesLength = (data.Length - index) / 2;
            BombPoses = new Vector2Int[tilesLength];
            for (var i = 0; i < BombPoses.Length; i++)
                BombPoses[i] = getPos();

            Vector2Int getPos()
            {
                return new Vector2Int(int.Parse(data[index++]), int.Parse(data[index++]));
            }
        }
    }
}