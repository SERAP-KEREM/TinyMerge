using UnityEngine;
using System.Collections.Generic;
using Zenject;
using _Main._Items;

namespace _Main._Data
{
    /// <summary>
    /// ScriptableObject to manage level-specific configurations such as timer settings and item data.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "Data/LevelConfig")]
    public class LevelConfig : ScriptableObjectInstaller<LevelConfig>
    {
        #region Timer Settings

        [Header("Timer Settings")]
        [Tooltip("Initial time for the level in seconds.")]
        [Range(30f, 600f)]
        public float InitialTime = 180f;

        [Tooltip("Critical time threshold in seconds.")]
        [Range(0f, 20f)]
        public float CriticalTimeThreshold = 10f;

        #endregion

        #region Items

        [Header("Items")]
        [Tooltip("List of item data for creation and requirements.")]
        public List<ItemData> ItemDataList;

        #endregion

        #region Nested Classes

        /// <summary>
        /// Represents the configuration for an item in the level.
        /// </summary>
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

        #endregion

        #region Validation

        /// <summary>
        /// Validates the integrity of the level configuration.
        /// Ensures that all required fields are properly assigned.
        /// </summary>
        private void OnValidate()
        {
            if (InitialTime <= 0)
            {
                Debug.LogWarning("InitialTime must be greater than zero.", this);
            }

            if (CriticalTimeThreshold < 0)
            {
                Debug.LogWarning("CriticalTimeThreshold cannot be negative.", this);
            }

            if (ItemDataList == null || ItemDataList.Count == 0)
            {
                Debug.LogWarning("ItemDataList is empty or null. Please assign valid items in the inspector.", this);
            }
            else
            {
                foreach (var itemData in ItemDataList)
                {
                    if (itemData.ItemPrefab == null)
                    {
                        Debug.LogWarning("ItemPrefab is not assigned in one or more entries.", this);
                    }

                    if (itemData.ItemCount <= 0)
                    {
                        Debug.LogWarning("ItemCount must be greater than zero in one or more entries.", this);
                    }
                }
            }
        }

        #endregion
    }
}