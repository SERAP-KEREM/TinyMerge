using DG.Tweening.Core.Easing;
using SerapKeremGameTools._Game._SaveLoadSystem;
using System.Collections.Generic;
using UnityEngine;

namespace _Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Game Configuration")]
        [Tooltip("List of levels in the game.")]
        [SerializeField]
        private List<LevelConfig> _levelList;

        [Tooltip("Current level index in the game.")]
        [HideInInspector] // We are managing this through SaveManager, so it should not be editable in the inspector.
        [SerializeField]
        private int _currentLevelIndex;

        /// <summary>
        /// Property to get or set the current level index. 
        /// When setting, it will save the new value using SaveManager.
        /// When getting, it will load the value from SaveManager.
        /// </summary>
        public int CurrentLevelIndex
        {
            get
            {
                // Load the level index from SaveManager
                //_currentLevelIndex = SaveManager.LoadLevelIndex();
                return _currentLevelIndex % _levelList.Count;
            }
            set
            {
                // Set the value and save it via SaveManager
                _currentLevelIndex = value;
                //SaveManager.SaveLevelIndex(_currentLevelIndex);
            }
        }

        /// <summary>
        /// Gets the configuration of the current level based on the index.
        /// </summary>
        public LevelConfig CurrentLevel => _levelList[CurrentLevelIndex];
    }
}