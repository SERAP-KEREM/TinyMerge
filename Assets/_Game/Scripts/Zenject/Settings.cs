using UnityEngine;
using Zenject;

namespace _Main.Settings
{
    public class TileManagerSettings
    {
        public Vector2Int gridSize;
        public Vector2 tileSpacing;
        public float tileHeight;
    }

    public class ItemManagerSettings
    {
        public Vector3 spawnAreaMin;
        public Vector3 spawnAreaMax;
        public float itemSpawnDelay;
    }
}