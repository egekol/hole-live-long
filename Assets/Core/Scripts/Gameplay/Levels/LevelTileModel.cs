using UnityEngine;

namespace Core.Scripts.Gameplay.Levels
{
    public class LevelTileModel
    {
        public int Id { get; private set; }
        public TileType Type;
        public Vector2Int Coordinates;

        public LevelTileModel(int id, TileType type, Vector2Int coordinates)
        {
            Id = id;
            Type = type;
            Coordinates = coordinates;
        }
    }
}