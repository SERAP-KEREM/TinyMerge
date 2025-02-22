using System;
using TriInspector;
using UnityEngine;

namespace _Main._SaveSystem
{
    /// <summary>
    /// SaveData holds the data that will be saved and loaded by the SaveSystem.
    /// For now, this includes the CurrentLevelIndex, but can be expanded as the game grows.
    /// </summary>
    [Serializable]
    public class SaveData
    {
        #region Serialized Fields

        [Header("Game Progress")]
        [PropertyTooltip("The index of the current level the player is on.")]
        [SerializeField]
        private int _currentLevelIndex = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the index of the current level the player is on.
        /// </summary>
        public int CurrentLevelIndex
        {
            get => _currentLevelIndex;
            set => _currentLevelIndex = value;
        }

        #endregion
    }
}