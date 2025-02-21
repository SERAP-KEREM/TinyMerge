using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;
using _Game.Scripts.Services;
using _Game.Scripts.Data;
using _Game.Scripts.Management;

namespace _Game.Scripts.UI
{
    public class GameplayUI : MonoBehaviour
    {
        [Header("Info Texts")]
        [SerializeField] private TextMeshProUGUI _levelText;
        [SerializeField] private TextMeshProUGUI _timerText;

        [Header("Special Skill Buttons")]
        [SerializeField] private Button _itemShakerButton;
        [SerializeField] private Button _destroyTripleItemButton;
        [SerializeField] private Button _recycleItemButton;
        [SerializeField] private Button _freezeItemButton;

        [Header("Settings")]
        [SerializeField] private Button _settingsButton;

        [Header("Timer Effects")]
        [SerializeField] private Color _criticalTimeColor = Color.red;
        [SerializeField] private float _criticalTimeThreshold = 10f;

        private ITimeManager _timeManager;
        private LevelManager _levelManager;
        private GameData _gameData;
        private UIManager _uiManager;

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

        private void Start()
        {
            InitializeUI();
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void InitializeUI()
        {
            UpdateLevelText();
            SetupButtons();
        }

        private void UpdateLevelText()
        {
            _levelText.text = $"LEVEL {_gameData.CurrentLevelIndex + 1}";
        }

        private void SetupButtons()
        {
            _settingsButton.onClick.AddListener(() => _uiManager.ShowSettingsPanel());
        }

        private void SubscribeToEvents()
        {
            _timeManager.OnTimerUpdated += UpdateTimerDisplay;
        }

        private void UnsubscribeFromEvents()
        {
            if (_timeManager != null)
            {
                _timeManager.OnTimerUpdated -= UpdateTimerDisplay;
            }
        }

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

        private void ApplyCriticalTimeEffect()
        {
            _timerText.color = _criticalTimeColor;
            _timerText.transform.DOScale(Vector3.one * 1.2f, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }
}