using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Events;
using Zenject;
using _Main._Items;
using _Main._Data;

namespace _Main._Management
{
    /// <summary>
    /// Manages level progression, UI elements, and animations using DOTween for smooth transitions.
    /// Handles level completion, failure, item collection updates, and scene management.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Indicator Settings")]
        [Tooltip("Parent transform for item indicators.")]
        [SerializeField]
        private Transform _indicatorsParent;

        [Tooltip("Prefab for item indicators.")]
        [SerializeField]
        private ItemIndicator _indicatorPrefab;

        [Header("Effects")]
        [Tooltip("Positions where fireworks particle effects will be played.")]
        [SerializeField]
        private List<Vector3> _fireworksParticlePositions;

        [Tooltip("Key for the fireworks particle effect.")]
        [SerializeField]
        private string _fireworksParticleKey = "Fireworks";

        [Tooltip("Audio clip key for the fireworks sound.")]
        [SerializeField]
        private string _fireworksClipKey = "Fireworks";

        [Tooltip("Audio clip key for level completion sound.")]
        [SerializeField]
        private string _levelCompleteClipKey = "LevelComplete";

        [Tooltip("Audio clip key for level failure sound.")]
        [SerializeField]
        private string _levelFailClipKey = "LevelFail";

        [Tooltip("Audio clip key for background music.")]
        [SerializeField]
        private string _backgroundMusicClipKey = "BackgroundMusic";

        #endregion

        #region Private Fields

        private Dictionary<int, ItemIndicator> _itemIndicators = new Dictionary<int, ItemIndicator>();
        private Dictionary<int, int> _requiredItemCounts = new Dictionary<int, int>();
        private int _currentLevelIndex = 0;

        public UnityAction OnLevelFailed;
        public UnityAction OnLevelCompleted;

        private ParticleManager _particleManager;
        private GameData _gameData;
        private AudioManager _audioManager;
        private ITimeManager _timeManager;
        private UIManager _uIManager;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the LevelManager.
        /// </summary>
        [Inject]
        public void Construct(ParticleManager particleManager, GameData gameData, AudioManager audioManager,
            ITimeManager timeManager, UIManager uIManager)
        {
            _particleManager = particleManager;
            _gameData = gameData;
            _audioManager = audioManager;
            _timeManager = timeManager;
            _uIManager = uIManager;
        }

        #endregion

        #region Lifecycle Methods

        private void Start()
        {
            CreateItemIndicators();
            PlayBackgroundMusic();
            ApplySavedMusicVolume();
            _timeManager.OnTimerFinished += LevelFail;
        }

        #endregion

        #region Music Management

        /// <summary>
        /// Applies the saved music volume settings to all audio sources in the scene.
        /// </summary>
        private void ApplySavedMusicVolume()
        {
          //  float savedVolume = LoadManager.LoadData<float>("MusicVolume", 1f);
            float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                source.volume = savedVolume;
            }
        }

        /// <summary>
        /// Plays the background music.
        /// </summary>
        public void PlayBackgroundMusic()
        {
            if (_audioManager != null)
            {
                _audioManager.PlaySound(_backgroundMusicClipKey);
            }
        }

        #endregion

        #region Item Indicators

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

        #endregion

        #region Level Progression

        /// <summary>
        /// Marks the level as complete, shows completion UI, and plays related effects.
        /// </summary>
        public void LevelComplete()
        {
            OnLevelCompleted?.Invoke();
            PlayEffects(_fireworksParticleKey, _fireworksClipKey, _levelCompleteClipKey);
            IncreaseLevelIndex();
            _uIManager.ShowWinPanel();
            Debug.Log("Level Completed!");
        }

        /// <summary>
        /// Marks the level as failed and plays failure effects.
        /// </summary>
        public void LevelFail()
        {
            OnLevelFailed?.Invoke();
            _audioManager.PlaySound(_levelFailClipKey);
            _uIManager.ShowFailPanel();
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

        #endregion

        #region Effects

        /// <summary>
        /// Plays particle and audio effects for level completion and fireworks.
        /// </summary>
        /// <param name="particleKey">Key for the particle effect.</param>
        /// <param name="fireworksClipKey">Key for the fireworks sound clip.</param>
        /// <param name="levelCompleteClipKey">Key for the level complete sound clip.</param>
        private void PlayEffects(string particleKey, string fireworksClipKey, string levelCompleteClipKey)
        {
            foreach (var position in _fireworksParticlePositions)
            {
                _particleManager.PlayParticleAtPoint(particleKey, position);
                _audioManager.PlaySound(fireworksClipKey);
                _audioManager.PlaySound(levelCompleteClipKey);
            }
        }

        #endregion
    }
}