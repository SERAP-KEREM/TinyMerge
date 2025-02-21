using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;
using _Game.Scripts.Management;

namespace _Game.Scripts.UI
{

    public class SettingsPanelUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Audio Settings")]
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private TextMeshProUGUI _musicValueText;

        [Header("Buttons")]
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _restartButton;

        [Header("Animation")]
        [SerializeField] private float _fadeTime = 0.3f;

        private LevelManager _levelManager;

        [Inject]
        private void Construct(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        private void Start()
        {
            InitializeUI();
            Hide();
        }

        private void InitializeUI()
        {
            SetupMusicSlider();
            SetupButtons();
        }

        private void SetupMusicSlider()
        {
            UpdateMusicValueText(_musicSlider.value);
            _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        private void SetupButtons()
        {
            _resumeButton.onClick.AddListener(Hide);
            _restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnMusicVolumeChanged(float value)
        {
            UpdateMusicValueText(value);
        }

        private void UpdateMusicValueText(float value)
        {
            _musicValueText.text = $"{Mathf.RoundToInt(value * 100)}%";
        }

        private void OnRestartClicked()
        {
            Hide();
            _levelManager.Restart();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.DOFade(1, _fadeTime);
            Time.timeScale = 0;
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0, _fadeTime).OnComplete(() => {
                gameObject.SetActive(false);
                Time.timeScale = 1;
            });
        }
    }
}