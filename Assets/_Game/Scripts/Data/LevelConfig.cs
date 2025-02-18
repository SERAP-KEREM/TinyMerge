using UnityEngine;
using System.Collections.Generic;
using _Game.Scripts.Items;

namespace _Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Data/LevelConfig")]
    public class LevelConfig : ScriptableObject
    {
        [Header("Timer Settings")]
        [Tooltip("Initial time for the level in seconds.")]
        [Range(30f, 600f)]
        public float InitialTime = 180;

        [Tooltip("Critical time threshold in seconds.")]
        [Range(0f, 20f)]
        public float CriticalTimeThreshold = 10f;

        [Header("Items")]
        [Tooltip("List of item data for creation and requirements.")]
        public List<ItemData> ItemDataList;

        [System.Serializable]
        public class ItemData
        {
            [Header("Item Prefab Settings")]
            [Tooltip("The item prefab.")]
            public Item ItemPrefab;

            [Tooltip("The number of items to create. Adjusted to a multiple of three.")]
            public int ItemCount;

            [Tooltip("Whether this item is required.")]
            public bool IsRequired;
        }
    }
}