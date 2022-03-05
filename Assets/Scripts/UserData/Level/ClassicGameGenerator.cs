using System;
using ModelData.Level;
using ModelData.TileMap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UserData.Level
{
    public class ClassicGameGenerator
    {
        private readonly MapDataType type;
        private readonly SeedGeneratorData model = new SeedGeneratorData();

        public ClassicGameGenerator(MapDataType type)
        {
            this.type = type;
        }
        
        public MapClassicData CreateGameData()
        {
            var size = GetMapSize();
            var hue = GetHue();
            var seed = GetRandomSeed();
            var bombs = GetBombs();
            
            var data = new MapClassicData(type)
            {
                Width = size.x,
                Height = size.y,
                Bombs = bombs,
                Hue = hue,
                Seed = seed,
            };
            
            Debug.Log($"ClassicType: {type}, {data}, color: {hue}");

            return data;
        }

        public int GetBombs()
        {
            return type switch
            {
                MapDataType.beginner => 10,
                MapDataType.intermediate => 40,
                MapDataType.expert => 99,
                MapDataType.guru => 120,
                MapDataType.master => 100,
                MapDataType.grandmaster => 200,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public int GetRandomSeed()
        {
            Random.InitState(Time.frameCount);
            return Random.Range(0, int.MaxValue);
        }

        private Vector2Int GetMapSize()
        {
            return type switch
            {
                MapDataType.beginner => new Vector2Int(9, 9),
                MapDataType.intermediate => new Vector2Int(16, 16),
                MapDataType.expert => new Vector2Int(16, 30),
                MapDataType.guru => new Vector2Int(15, 30),
                MapDataType.master => new Vector2Int(20, 20),
                MapDataType.grandmaster => new Vector2Int(15, 30),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private float GetHue()
        {
            return type switch
            {
                MapDataType.beginner => 0.36f,
                MapDataType.intermediate => 0.64f,
                MapDataType.expert => 0.5f,
                MapDataType.guru => 0.86f,
                MapDataType.master => 0.77f,
                MapDataType.grandmaster => 0.28f,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}