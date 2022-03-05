using System.Collections.Generic;
using System.Linq;
using Core.AssetsManager;
using UnityEngine;
using UserData.TileMap.Generators;

namespace UserData.TileMap.AntiPatterns
{
    public class AntiPatternsSystem
    {
        private const string PATH = "static/antipatterns";

        public readonly List<AntiPattern> antiPatterns = new List<AntiPattern>();

        private static AntiPatternsSystem instance;
        public static AntiPatternsSystem Instance => instance ??= new AntiPatternsSystem();

        public AntiPatternsSystem()
        {
            var asset = AssetsLoader.LoadSync<TextAsset>(PATH);
            var data = asset.text;
            foreach (var antiPatternData in data.Split('&'))
            {
                if(antiPatternData.Contains("//") || antiPatternData.Length <= 0)
                    continue;
                antiPatterns.Add(new AntiPattern(antiPatternData));
            }
        }

        public bool IsAntiPattern(TileMapGeneratorAbstract generator, Vector2Int bombPos)
        {
            return antiPatterns.Any(a => a.IsMatch(generator, bombPos));
        }
    }
}