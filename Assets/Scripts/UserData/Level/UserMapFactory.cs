using System;
using Core;
using ModelData.TileMap;
using UserData.TileMap;
using UserData.TileMap.Generators;

namespace UserData.Level
{
    public static class UserMapFactory
    {
        public static UserMap CreateUserMap(MapDataType type)
        {
            if (type.IsTimer())
            {
                var model = new ClassicGameGenerator(type).CreateGameData();
                return CreateUserMap(model);
            }

            if (type == MapDataType.level)
            {
                return CreateUserMap(GameBySeedGenerator.Create(Game.User.Level));
            }
            
            throw new Exception("Unknown type for UserMapFactory: " + type);
        }
        
        public static UserMap CreateUserMap(int level, int? seed = null)
        {
            return CreateUserMap(GameBySeedGenerator.Create(level, seed));
        }

        public static UserMap CreateUserMap(MapData model)
        {
            if(model is MapLevelData levelModel)
                return new UserMapLevel(levelModel, new RandomTileMapGenerator());
            if(model is MapClassicData classicModel)
                return new UserMapClassic(classicModel, new RandomTileMapGenerator());
            return new UserMap(model, new RandomTileMapGenerator());
        }
        
        public static UserMap LoadUserMap()
        {
            var model = UserMapSaver.LoadMapData();
            if (model == null)
                return null;
            
            var commands = UserMapSaver.LoadCommands();
            if (commands == null)
                return null;
            
            var userMap = CreateUserMap(model);
            userMap.CommandSystem.LoadCommands(commands);
            userMap.TryUpdateState();

            return userMap;
        }

    }
}