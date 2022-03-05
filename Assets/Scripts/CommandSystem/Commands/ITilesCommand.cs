using UnityEngine;

namespace CommandSystem.Commands
{
    public interface ITilesCommand
    {
        Vector2Int[] ChangedTiles { get; }
    }
}