namespace UserData.TileMap.AntiPatterns
{
    public enum AntiPatternTypes
    {
         block = 0,        //блокирующая клетка. Либо край карты, либо бомба.
         anything = 9,     //не важно что за тайл
         empty = 8,        //пустой тайл
    }
}