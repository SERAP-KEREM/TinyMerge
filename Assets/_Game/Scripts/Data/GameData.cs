using _Game.Scripts._helpers;
using _Game.Scripts.Management;
using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Data/GameData")]
    public class GameData : ScriptableObjectInstaller<GameData>
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
                _currentLevelIndex = SaveManager.LoadLevelIndex();
                return _currentLevelIndex % _levelList.Count;
            }
            set
            {
                // Set the value and save it via SaveManager
                _currentLevelIndex = value;
                SaveManager.SaveLevelIndex(_currentLevelIndex);
            }
        }

        /// <summary>
        /// Gets the configuration of the current level based on the index.
        /// </summary>
        public LevelConfig CurrentLevel => _levelList[CurrentLevelIndex];

        public override void InstallBindings()
        {
            // GameData'y? kendisinden ba?lama
            Container.Bind<GameData>()
                .FromInstance(this)
                .AsSingle();

            // Mevcut LevelConfig'i ba?lama
            Container.Bind<LevelConfig>()
                .FromInstance(CurrentLevel)
                .AsSingle();
        }
    }
}