using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;
using _Main._Management;
using _Main._Data;

namespace _Main._UI
{
    /// <summary>
    /// Manages the gameplay UI, including level information, timer display, and critical time effects.
    /// </summary>
    public class GameplayUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Info Texts")]
        [Tooltip("Text displaying the current level.")]
        [SerializeField]
        private TextMeshProUGUI _levelText;

        [Tooltip("Text displaying the remaining time.")]
        [SerializeField]
        private TextMeshProUGUI _timerText;

        [Header("Settings")]
        [Tooltip("Button to open the settings panel.")]
        [SerializeField]
        private Button _settingsButton;

        [Header("Timer Effects")]
        [Tooltip("Color applied to the timer text when time is critical.")]
        [SerializeField]
        private Color _criticalTimeColor = Color.red;

        [Tooltip("Threshold (in seconds) for applying critical time effects.")]
        [SerializeField]
        private float _criticalTimeThreshold = 10f;

        #endregion

        #region Private Fields

        private ITimeManager _timeManager;
        private LevelManager _levelManager;
        private GameData _gameData;
        private UIManager _uiManager;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the GameplayUI.
        /// </summary>
        [Inject]
        private void Construct(
            ITimeManager timeManager,
            LevelManager levelManager,
            GameData gameData,
            UIManager uiManager)
        {
            _timeManager = timeManager;
            _levelManager = levelManager;
            _gameData = gameData;
            _uiManager = uiManager;
        }

        #endregion

        #region Lifecycle Methods

        private void Start()
        {
            InitializeUI();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the UI elements and sets up button listeners.
        /// </summary>
        private void InitializeUI()
        {
            UpdateLevelText();
            SetupButtons();
        }

        /// <summary>
        /// Updates the level text to reflect the current level index.
        /// </summary>
        private void UpdateLevelText()
        {
            _levelText.text = $"LEVEL {_gameData.CurrentLevelIndex + 1}";
        }

        /// <summary>
        /// Sets up button listeners for UI interactions.
        /// </summary>
        private void SetupButtons()
        {
            if (_settingsButton != null)
            {
                _settingsButton.onClick.AddListener(() => _uiManager.ShowSettingsPanel());
            }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Subscribes to timer update events.
        /// </summary>
        private void SubscribeToEvents()
        {
            if (_timeManager != null)
            {
                _timeManager.OnTimerUpdated += UpdateTimerDisplay;
            }
        }

        /// <summary>
        /// Unsubscribes from timer update events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromEvents()
        {
            if (_timeManager != null)
            {
                _timeManager.OnTimerUpdated -= UpdateTimerDisplay;
            }
        }

        #endregion

        #region Timer Display

        /// <summary>
        /// Updates the timer display with the remaining time.
        /// Applies critical time effects if the remaining time is below the threshold.
        /// </summary>
        /// <param name="currentTime">The remaining time in seconds.</param>
        private void UpdateTimerDisplay(float currentTime)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            _timerText.text = $"{minutes:D2}:{seconds:D2}";

            if (currentTime <= _criticalTimeThreshold)
            {
                ApplyCriticalTimeEffect();
            }
        }

        /// <summary>
        /// Applies visual effects to the timer text when time is critical.
        /// </summary>
        private void ApplyCriticalTimeEffect()
        {
            _timerText.color = _criticalTimeColor;
            _timerText.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }

        #endregion
    }
}