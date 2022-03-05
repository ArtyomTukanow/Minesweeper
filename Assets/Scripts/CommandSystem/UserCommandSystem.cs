using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AllUtils;
using CommandSystem.Commands;
using Mobile;
using UnityEngine;
using UserData.Level;
using UserData.TileMap;

namespace CommandSystem
{
    public class UserCommandSystem : CommandSystem
    {
        private const long SHORT_VIBRATE = 50;
        private const long LONG_VIBRATE = 200;
        
        public bool NeedSaveCommands = true;
        
        private readonly UserMap userMap;

        public UserCommandSystem(UserMap userMap)
        {
            this.userMap = userMap;
        }

        public void OpenAll()
        {
            var points = userMap.Field.Values
                .Where(t => !t.Opened && !t.Bomb)
                .Select(t => t.Pos)
                .ToArray();
            RedoCommand(new OpenCommand(userMap, points));
        }

        public void Open(params Vector2Int[] points) => RedoCommand(new OpenCommand(userMap, points));
        public void Open(Vector2Int point) => RedoCommand(new OpenCommand(userMap, new []{point}));

        public void SetFlags(params Vector2Int[] points) => RedoCommand(new FlagCommand(userMap, points, true));
        public void SetFlag(Vector2Int point, bool flagged) => RedoCommand(new FlagCommand(userMap, new []{point}, flagged));
        
        public void CatchBombs(params Vector2Int[] points) => RedoCommand(new CatchBombCommand(userMap, points));
        public void CatchBomb(Vector2Int point) => RedoCommand(new CatchBombCommand(userMap, new []{point}));


        
        public void OnClickTile(Vector2Int pos)
        {
            if (!userMap.ContainsTile(pos))
                return;
            
            if (!userMap.IsGenerated)
            {
                Open(pos);
                return;
            }

            var tile = userMap[pos];
            
            if(tile.Flagged)
                return;

            if (tile.Bomb)
            {
                CatchBomb(pos);
                Vibration.Vibrate(LONG_VIBRATE);
            }
            else if (!tile.Opened)
                Open(pos);
            else
                CleverOpenByPos(pos);
        }

        public void OnLongClick(Vector2Int pos)
        {
            if (!userMap.ContainsTile(pos))
                return;
            
            if (!userMap.IsGenerated)
            {
                Open(pos);
                return;
            }
            
            if(userMap[pos] == null)
                throw new InvalidDataException("Generator not generated a pos: " + pos);

            if (!userMap[pos].Opened)
            {
                SetFlag(pos, !userMap[pos].Flagged);
                Vibration.Vibrate(SHORT_VIBRATE);
            }
        }

        private void CleverOpenByPos(Vector2Int pos)
        {
            var flaggedPoints = userMap.GetNearPoints(pos).Where(p => userMap[p]?.Flagged == true).ToList();
            var bombsPoints = userMap.GetNearPoints(pos).Where(p => userMap[p]?.Bomb == true).ToList();
            var notOpenedPoints = userMap.GetNearPoints(pos).Where(p => userMap[p]?.Opened == false).ToList();

            var allBombsNearFound = flaggedPoints.AreEquals(bombsPoints);
            if (allBombsNearFound)
            {
                var needOpenPoints = userMap.GetNearPoints(pos).Where(p => userMap[p] != null && !userMap[p].Opened && !userMap[p].Bomb).ToArray();
                if(needOpenPoints.Length > 0)
                    Open(needOpenPoints);
                return;
            }

            var allPointsIsBombs = notOpenedPoints.Count == bombsPoints.Count;
            if (allPointsIsBombs)
            {
                SetFlags(notOpenedPoints.ToArray());
                return;
            }

            var wrongFoundBombs = flaggedPoints.Count == bombsPoints.Count;
            if (wrongFoundBombs)
            {
                var bombs = bombsPoints.Where(p => userMap[p]?.Flagged == false).ToArray();
                CatchBombs(bombs);
            }
        }

        protected override void UndoCommand(ICommand cmd)
        {
            base.UndoCommand(cmd);
            if(NeedSaveCommands)
                UserMapSaver.RemoveLastCommand();
        }

        protected override void RedoCommand(ICommand cmd, bool clearUndo = true, bool invokeUpdate = true)
        {
            base.RedoCommand(cmd, clearUndo, invokeUpdate);
            if(NeedSaveCommands && cmd is UserCommandBase userCmd)
                UserMapSaver.SaveCommand(SerializeCommand(userCmd));
        }
        
        
        
        
        
        

        public static ICommand DeserializeCommand(string cmd)
        {
            var tmp = cmd.Split(UserCommandBase.SEPARATOR);

            try
            {
                return CreateUserCommandByType(tmp);
            }
            catch (Exception e)
            {
                Debug.LogError("Can't parse command: " + cmd);
                throw e;
            }
        }

        public static string SerializeCommand(UserCommandBase cmd) => cmd.Serialize();



        public void LoadCommands(List<ICommand> cmds)
        {
            foreach (var cmd in cmds)
            {
                if (cmd is IUserCommand userCommand)
                    userCommand.UserMap = userMap;
                base.RedoCommand(cmd, invokeUpdate: false);
            }
        }

        private static UserCommandBase CreateUserCommandByType(string[] data)
        {
            var type = data[0];
            switch (type)
            {
                case CatchBombCommand.TYPE: return new CatchBombCommand(data);
                case FlagCommand.TYPE:      return new FlagCommand(data);
                case OpenCommand.TYPE:      return new OpenCommand(data);
                default: throw new Exception("Undefined type: " + type);
            }
        }
    }
}