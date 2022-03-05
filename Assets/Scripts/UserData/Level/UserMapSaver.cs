using System.Collections.Generic;
using AllUtils;
using CommandSystem;
using CommandSystem.Commands;
using ModelData.TileMap;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace UserData.Level
{
    public class UserMapSaver
    {
        private const string LEVEL_DATA_FILE = "level.data";
        private const string COMMANDS_DATA_FILE = "commands.data";
        
        public static MapData LoadMapData()
        {
            var levelJson = AllUtils.Utils.ReadFile<string>(LEVEL_DATA_FILE);
            if (levelJson.IsNullOrEmpty())
                return null;
            
            var levelData = JsonConvert.DeserializeObject<JToken>(levelJson);

            MapDataType mapType = (MapDataType)levelData["t"]?.ToObject<MapDataType>();
            
            if (mapType.IsTimer())
                return levelData.ToObject<MapClassicData>();
            
            if (mapType == MapDataType.level)
                return levelData.ToObject<MapLevelData>();
            
            return levelData.ToObject<MapData>();
        }
        
        public static List<ICommand> LoadCommands()
        {
            var cmdsLines = AllUtils.Utils.ReadLines(COMMANDS_DATA_FILE);
            if (cmdsLines == null)
                return null;
            
            var cmds = new List<ICommand>();
            foreach (var cmd in cmdsLines)
                if(cmd.NotEmpty())
                    cmds.Add(UserCommandSystem.DeserializeCommand(cmd));
            return cmds;
        }
        
        public static void RemoveMapData()
        {
            AllUtils.Utils.SaveFile(LEVEL_DATA_FILE, "");
            AllUtils.Utils.WriteLines(COMMANDS_DATA_FILE, new []{""});

            PlayerPrefs.SetInt("map_tm", 0);
        }

        public static void SaveTime(long time)
        {
            PlayerPrefs.SetFloat("map_tm", time);
        }

        public static long GetTime()
        {
            return (long)PlayerPrefs.GetFloat("map_tm");
        }

        public static void SaveMapData(MapData data)
        {
            var level = JsonConvert.SerializeObject(data);
            AllUtils.Utils.SaveFile(LEVEL_DATA_FILE, level);
        }

        public static void SaveCommand(string cmd)
        {
            AllUtils.Utils.AppendToFile(COMMANDS_DATA_FILE, cmd);
        }

        public static void RemoveLastCommand()
        {
            var cmds = AllUtils.Utils.ReadLines(COMMANDS_DATA_FILE);
            var newCmds = new string[cmds.Length - 1];
            for (var i = 0; i < newCmds.Length; i++)
                newCmds[i] = cmds[i];
            AllUtils.Utils.WriteLines(COMMANDS_DATA_FILE, newCmds);
        }
    }
}