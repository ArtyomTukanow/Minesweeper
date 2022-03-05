namespace ModelData.Level
{
    public class SeedGeneratorData
    {
        public int Seed { get; } = 123615341;
        
        public int CurvePeriod { get; } = 5;
        public float PeriodDiffMax { get; } = 0.15f;

        public int MinTileSize { get; } = 10;
        public int MaxTileSize { get; } = 50;

        private float SizeDispersion { get; } = 0.2f;
        public float MinSizeDispersion => 1 - SizeDispersion;
        public float MaxSizeDispersion => 1 + SizeDispersion;

        public float MinDifficultly { get; } = 0.4f / 8;
        public float MaxDifficultly { get; } = 4f / 8;
        
        public float DifficultlyDeceleration { get; } = 5;
    }
}