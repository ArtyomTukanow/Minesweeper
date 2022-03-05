using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UserData.TileMap.Generators;

namespace UserData.TileMap.AntiPatterns
{
    public class AntiPattern
    {
        private readonly List<Vector2Int> patternBombPoints = new List<Vector2Int>();
        private readonly Dictionary<Vector2Int, AntiPatternTypes> pattern = new Dictionary<Vector2Int, AntiPatternTypes>();
        
        public AntiPattern(string data)
        {
            ParseFromString(data);
        }

        private void ParseFromString(string data)
        {
            pattern.Clear();
            var lines = data.Split('\n', '\r').Where(l => l.Length > 0).ToArray();
            int y = 0, x = 0;

            try
            {
                for(y = 0; y < lines.Length; y ++)
                {
                    for(x = 0; x < lines[y].Length; x ++)
                    {
                        var type = FromChar(lines[y][x]);
                        var pos = new Vector2Int(x, y);
                        pattern[pos] = type;
                        
                        if(type == AntiPatternTypes.block)
                            patternBombPoints.Add(pos);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Error while parse pattern! Line: {y}, Column:{x}\nPattern:\n{data}\n{e.Message}");
            }
        }

        private AntiPatternTypes FromChar(char ch)
        {
            switch (ch)
            {
                case 'B': return AntiPatternTypes.block;
                case ' ': return AntiPatternTypes.empty;
                case '?': return AntiPatternTypes.anything;
                default: throw new Exception("Undefined symbol: " + ch);
            }
        }


        private TileMapGeneratorAbstract generator;
        private Vector2Int bombPos;
        private Vector2Int div;
        

        public bool IsMatch(TileMapGeneratorAbstract generator, Vector2Int bombPos)
        {
            return patternBombPoints.Any(b => IsMatch(generator, bombPos, bombPos - b));
        }

        private bool IsMatch(TileMapGeneratorAbstract generator, Vector2Int bombPos, Vector2Int div)
        {
            this.generator = generator;
            this.bombPos = bombPos;
            this.div = div;

            var result = pattern.Keys.All(IsEqualPos);
            
            return result;
        }

        private bool IsEqualPos(Vector2Int patternPos)
        {
            var tilePos = patternPos + div;
            switch (pattern[patternPos])
            {
                case AntiPatternTypes.block: return IsBlock(tilePos);
                case AntiPatternTypes.empty: return IsEmpty(tilePos);
                case AntiPatternTypes.anything: return IsAnything(tilePos);
                default: throw new Exception("Need add new pattern type here!");
            }
        }

        private bool IsBlock(Vector2Int pos)
        {
            return !generator.ContainsTile(pos) || IsBomb(pos);
        }

        private bool IsBomb(Vector2Int pos)
        {
            return pos == bombPos || generator.Field.TryGetValue(pos, out var tile) && tile.Bomb;
        }

        private bool IsEmpty(Vector2Int pos)
        {
            return !generator.Field.TryGetValue(pos, out var tile) || !tile.Bomb;
        }

        private bool IsAnything(Vector2Int pos)
        {
            return true;//generator.ContainsTile(pos);
        }
    }
}