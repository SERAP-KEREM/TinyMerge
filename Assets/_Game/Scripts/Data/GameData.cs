using _Main._SaveSystem;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using TriInspector;

namespace _Main._Data
{
    /// <summary>
    /// ScriptableObject to manage game-wide data such as levels and current level index.
    /// </summary>
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObjectInstaller<GameData>
    {
        #region Serialized Fields

        [Header("Game Configuration")]
        [Required, PropertyTooltip("List of levels in the game.")]
        [SerializeField]
        private List<LevelConfig> _levelList;

        [HideInInspector, SerializeField, PropertyTooltip("Current level index in the game. Managed through SaveManager.")]
        private int _currentLevelIndex;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current level index.
        /// When setting, it saves the new value using SaveManager.
        /// When getting, it loads the value from SaveManager and ensures it wraps around the level list.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("The current level index, managed by SaveManager.")]
        public int CurrentLevelIndex
        {
            get
            {
                // Load the level index from SaveManager and ensure it wraps around the level list.
                _currentLevelIndex = SaveManager.LoadLevelIndex();
                return _currentLevelIndex % (_levelList?.Count ?? 1);
            }
            set
            {
                // Set the value and save it via SaveManager.
                _currentLevelIndex = value;
                SaveManager.SaveLevelIndex(_currentLevelIndex);
            }
        }

        /// <summary>
        /// Gets the configuration of the current level based on the index.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("The configuration of the current level.")]
        public LevelConfig CurrentLevel => _levelList?[CurrentLevelIndex];

        #endregion

        #region Validation

        /// <summary>
        /// Validates the integrity of the game data.
        /// Ensures that the level list is not null or empty.
        /// </summary>
        private void OnValidate()
        {
            if (_levelList == null || _levelList.Count == 0)
            {
                Debug.LogWarning("Level list is empty or null. Please assign valid levels in the inspector.", this);
            }
        }

        #endregion
    }
}