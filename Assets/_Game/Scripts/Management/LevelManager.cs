using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using _Game.Scripts.Items;
using _Game.Scripts.Data;
using _Game.Scripts._helpers;
using Zenject; // For DOTween functionality

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages level progression, UI elements, and animations using DOTween for smooth transitions.
    /// Handles level completion, failure, item collection updates, and scene management.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        [Header("Indicator Settings")]
        [Tooltip("Parent transform for item indicators.")]
        [SerializeField] private Transform _indicatorsParent;

        [Tooltip("Prefab for item indicators.")]
        [SerializeField] private ItemIndicator _indicatorPrefab;

        [Header("Effects")]
        [Tooltip("Positions where fireworks particle effects will be played.")]
        [SerializeField] private List<Vector3> _fireworksParticlePositions;

        [Tooltip("Key for the fireworks particle effect.")]
        [SerializeField] private string _fireworksParticleKey = "Fireworks";

        [Tooltip("Audio clip key for the fireworks sound.")]
        [SerializeField] private string _fireworksClipKey = "Fireworks";

        [Tooltip("Audio clip key for level completion sound.")]
        [SerializeField] private string _levelCompleteClipKey = "LevelComplete";

        [Tooltip("Audio clip key for level failure sound.")]
        [SerializeField] private string _levelFailClipKey = "LevelFail";

        private Dictionary<int, ItemIndicator> _itemIndicators = new Dictionary<int, ItemIndicator>();
        private Dictionary<int, int> _requiredItemCounts = new Dictionary<int, int>();

        private int _currentLevelIndex = 0;

        public UnityAction OnLevelFailed;
        public UnityAction OnLevelCompleted;

        private ParticleManager _particleManager;
        private GameData _gameData;

        [Inject]
        public void Construct(ParticleManager particleManager, GameData gameData)
        {
            _particleManager = particleManager;
            _gameData = gameData;
        }

        private void Start()
        {
            CreateItemIndicators();
           // TimeManager.OnTimeFinished += LevelFail;
        }

        /// <summary>
        /// Creates and initializes item indicators based on the current level's data.
        /// </summary>
        private void CreateItemIndicators()
        {
            _itemIndicators.Clear();
            _requiredItemCounts.Clear();

            foreach (var itemData in _gameData.CurrentLevel.ItemDataList)
            {
                if (itemData.IsRequired)
                {
                    var itemIndicator = Instantiate(_indicatorPrefab, _indicatorsParent);
                    itemIndicator.SetIcon(itemData.ItemPrefab.ItemIcon);
                    itemIndicator.SetQuantity(itemData.ItemCount);

                    _itemIndicators[itemData.ItemPrefab.ItemId] = itemIndicator;
                    _requiredItemCounts[itemData.ItemPrefab.ItemId] = itemData.ItemCount;
                }
            }
        }

        /// <summary>
        /// Marks the level as complete, shows completion UI, and plays related effects.
        /// </summary>
        public void LevelComplete()
        {
            OnLevelCompleted?.Invoke();
            PlayEffects(_fireworksParticleKey, _fireworksClipKey, _levelCompleteClipKey);
            IncreaseLevelIndex();
            Debug.Log("Level Completed!");
        }

        /// <summary>
        /// Marks the level as failed and plays failure effects.
        /// </summary>
        public void LevelFail()
        {
            OnLevelFailed?.Invoke();
            //GlobalBinder.singleton.AudioManager.PlaySound(_levelFailClipKey);
            Debug.Log("Level Failed!");
        }

        /// <summary>
        /// Increases the current level index and proceeds to the next level.
        /// </summary>
        public void IncreaseLevelIndex()
        {
            _currentLevelIndex = _gameData.CurrentLevelIndex + 1;
            _gameData.CurrentLevelIndex = _currentLevelIndex;
            Debug.Log($"Increased Level Index to {_currentLevelIndex}");
        }

        /// <summary>
        /// Restarts the current level by reloading the active scene.
        /// </summary>
        public void Restart()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Debug.Log("Game Restarted");
        }

        /// <summary>
        /// Loads the next level in the build settings.
        /// </summary>
        public void Next()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene((SceneManager.GetActiveScene().buildIndex + 1) %
                SceneManager.sceneCountInBuildSettings);
            Debug.Log("Next Level");
        }

        /// <summary>
        /// Navigates to the main menu scene.
        /// </summary>
        public void Menu()
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
            Debug.Log("Navigated to Menu");
        }

        /// <summary>
        /// Updates item indicators and required item counts when an item is collected.
        /// </summary>
        /// <param name="item">The collected item.</param>
        public void UpdateItemCollection(Item item)
        {
            if (_itemIndicators.TryGetValue(item.ItemId, out var itemIndicator))
            {
                itemIndicator.DecreaseQuantity();

                if (_requiredItemCounts.ContainsKey(item.ItemId))
                {
                    _requiredItemCounts[item.ItemId]--;

                    if (_requiredItemCounts[item.ItemId] <= 0)
                    {
                        _requiredItemCounts.Remove(item.ItemId);
                        if (_requiredItemCounts.Count <= 0)
                        {
                            LevelComplete();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Plays particle and audio effects for level completion and fireworks.
        /// </summary>
        /// <param name="particleKey">Key for the particle effect.</param>
        /// <param name="fireworksClipKey">Key for the fireworks sound clip.</param>
        /// <param name="levelCompleteClipKey">Key for the level complete sound clip.</param>
        private void PlayEffects(string particleKey, string fireworksClipKey, string levelCompleteClipKey)
        {
            int fireworkCount = _fireworksParticlePositions.Count;
            for (int i = 0; i < fireworkCount; i++)
            {
                _particleManager.PlayParticleAtPoint(particleKey, _fireworksParticlePositions[i]);
                // GlobalBinder.singleton.AudioManager.PlaySound(fireworksClipKey);
                // GlobalBinder.singleton.AudioManager.PlaySound(levelCompleteClipKey);
            }
        }
    }
}
