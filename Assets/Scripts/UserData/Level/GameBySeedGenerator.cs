using System;
using System.Collections.Generic;
using ModelData.Level;
using ModelData.TileMap;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UserData.Level
{
    public class GameBySeedGenerator
    {
        private readonly int level;
        private readonly int? seed;
        private readonly SeedGeneratorData model = new SeedGeneratorData();

        public GameBySeedGenerator(int level, int? seed = null)
        {
            this.level = level;
            this.seed = seed;
        }
        
        public static MapLevelData Create(int level, int? seed = null) => new GameBySeedGenerator(level, seed).CreateGameData();

        public MapLevelData CreateGameData()
        {
            var difficult = GetDifficultlyByLevel();
            var size = GetMapSizeByLevel();
            var bombs = GetBombsCountByDifficult(size.x, size.y, difficult);
            var hue = GetHueByLevel();
            var seed = this.seed ?? GetRandomSeed();
            
            var data = new MapLevelData
            {
                Width = size.x,
                Height = size.y,
                Bombs = bombs,
                Hue = hue,
                Level = level,
                Seed = seed,
            };
            
            Debug.Log($"Level: {level}, Difficult: {difficult}, {data}, color: {hue}");

            return data;
        }

        public int GetRandomSeed()
        {
            Random.InitState(level + Time.frameCount);
            return Random.Range(0, int.MaxValue);
        }
        
        public float GetDifficultlyByLevel()
        {
            //Кривая сложности. Усложняет на PeriodDiffMax каждые CurvePeriod, потом сбрасывается.
            var periodDiff = (float)(level % model.CurvePeriod) / model.CurvePeriod * model.PeriodDiffMax;
            
            //Вычисленная сложность. Считается по натуральному алгорифму + кривая сложности.
            var difficult = model.MinDifficultly + (Mathf.Log(level) / model.DifficultlyDeceleration + periodDiff) / 8;
            
            difficult = Mathf.Min(difficult, model.MaxDifficultly);
            difficult = Mathf.Max(difficult, model.MinDifficultly);

            return difficult;
        }


        private Vector2Int GetMapSizeByLevel()
        {
            Random.InitState(model.Seed + level);
            var widthDispersion = Random.Range(model.MinSizeDispersion, model.MaxSizeDispersion);
            var heightDispersion = Random.Range(model.MinSizeDispersion, model.MaxSizeDispersion);
            
            var width = 5 * Mathf.Log(level) + 5 * Mathf.Sin(level * 2);
            var height = 5 * Mathf.Log(level) + 5 * Mathf.Sin(Mathf.PI + level * 2);

            width *= widthDispersion;
            height *= heightDispersion;
            
            width = Mathf.Max(model.MinTileSize, width);
            width = Mathf.Min(model.MaxTileSize, width);
            
            height = Mathf.Max(model.MinTileSize, height);
            height = Mathf.Min(model.MaxTileSize, height);

            return new Vector2Int((int)Math.Min(width, height), (int)Math.Max(width, height));
        }
        
        private int GetBombsCountByDifficult(int width, int height, float difficulty)
        {
            var tilesCount = width * height - MapData.NEED_TILES_FOR_OPEN_BY_FIRST_TAP;
            return (int)(difficulty * tilesCount);
        }

        private float GetHueByLevel()
        {
            Random.InitState(model.Seed + level);

            return Random.Range(0f, 1f);
        }
    }
}