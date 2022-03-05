using NUnit.Framework;
using UnityEngine;
using UserData.TileMap.AntiPatterns;
using UserData.TileMap.Generators;

namespace Tests.UserData.TileMap.AntiPatterns
{
    public class AntiPatternsTest
    {
        [Test]
        public void TestAntiPatternsExists()
        {
            Assert.Positive(AntiPatternsSystem.Instance.antiPatterns.Count);
        }
        
        [Test]
        public void TestAntiPatternsTrue()
        {
            /*
             
             |
             V
             0111
             13*2
           ->*6*3
             ***2
        
             */
            var generator = DeterminedTileMapGenerator.TestSampleGenerator();

            var isAntiPattern = AntiPatternsSystem.Instance.IsAntiPattern(generator, new Vector2Int(0, 2));
            Assert.True(isAntiPattern);
        }
        
        [Test]
        public void TestAntiPatternsFalse()
        {
            /*
             
               |
               V
           ->0111
             13*2
             *6*3
             ***2
        
             */
            var generator = DeterminedTileMapGenerator.TestSampleGenerator();

            var isAntiPattern = AntiPatternsSystem.Instance.IsAntiPattern(generator, new Vector2Int(2, 0));
            Assert.False(isAntiPattern);
        }
    }
}