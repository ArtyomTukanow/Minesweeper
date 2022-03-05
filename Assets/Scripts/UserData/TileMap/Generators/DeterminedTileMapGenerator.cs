using System;
using System.Collections.Generic;
using ModelData.TileMap;
using UnityEngine;

namespace UserData.TileMap.Generators
{
    public class DeterminedTileMapGenerator : TileMapGeneratorAbstract
    {
        private Dictionary<Vector2Int, UserTile> determinedField;

        public static UserMap TestSample() => TestSampleGenerator().UserMap;

        public static DeterminedTileMapGenerator TestSampleGenerator()
        {
            var data = new MapData{ Width = 4, Height = 4, Bombs = 6, Seed = 1};
            var generator = new DeterminedTileMapGenerator();

            var userMap = new UserMap(data, generator);
            /*
             Поле выглядит так:
            
             0111
             13*2
             *6*3
             ***2
             
             */
            generator.determinedField = new Dictionary<Vector2Int, UserTile>
            {
                {new Vector2Int(0, 0), new UserTile(userMap, 0, 0, false)},
                {new Vector2Int(1, 0), new UserTile(userMap, 1, 0, false)},
                {new Vector2Int(2, 0), new UserTile(userMap, 2, 0, false)},
                {new Vector2Int(3, 0), new UserTile(userMap, 3, 0, false)},
                
                {new Vector2Int(0, 1), new UserTile(userMap, 0, 1, false)},
                {new Vector2Int(1, 1), new UserTile(userMap, 1, 1, false)},
                {new Vector2Int(2, 1), new UserTile(userMap, 2, 1, true)},
                {new Vector2Int(3, 1), new UserTile(userMap, 3, 1, false)},
                
                {new Vector2Int(0, 2), new UserTile(userMap, 0, 2, true)},
                {new Vector2Int(1, 2), new UserTile(userMap, 1, 2, false)},
                {new Vector2Int(2, 2), new UserTile(userMap, 2, 2, true)},
                {new Vector2Int(3, 2), new UserTile(userMap, 3, 2, false)},
                
                {new Vector2Int(0, 3), new UserTile(userMap, 0, 3, true)},
                {new Vector2Int(1, 3), new UserTile(userMap, 1, 3, true)},
                {new Vector2Int(2, 3), new UserTile(userMap, 2, 3, true)},
                {new Vector2Int(3, 3), new UserTile(userMap, 3, 3, false)},
            };
            generator.GenerateDetermined(userMap);

            return generator;
        }

        public DeterminedTileMapGenerator()
        {
            
        }

        public DeterminedTileMapGenerator(Dictionary<Vector2Int, UserTile> field)
        {
            determinedField = field;
            StartGenerate();
            IsGenerated = true;
        }

        public void GenerateDetermined(UserMap map) => base.Generate(map, Vector2Int.zero);

        [Obsolete("Use GenerateDetermined()", true)]
        public override void Generate(UserMap map, Vector2Int startPos) =>
            throw new InvalidOperationException("Use GenerateDetermined()");
        
        protected override void StartGenerate()
        {
            foreach (var tile in determinedField)
                Field[tile.Key] = tile.Value;
        }
    }
}