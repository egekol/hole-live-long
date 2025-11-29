using System;
using UnityEngine;

namespace Core.Scripts.Gameplay.Levels
{
    [Serializable]
    public class LevelTileData
    {
        public TileType Type;
        // public Vector3 Rotation;
        public Vector2Int Coordinates;
    }
}