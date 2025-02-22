using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using DG.Tweening;
using _Main._Management;

namespace _Main._UI
{
    /// <summary>
    /// Manages the settings panel UI, including audio settings and button interactions.
    /// </summary>
    public class SettingsPanelUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Panel")]
        [Tooltip("Canvas group for controlling the panel's visibility and fade effects.")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("Audio Settings")]
        [Tooltip("Slider for adjusting music volume.")]
        [SerializeField]
        private Slider _musicSlider;

        [Tooltip("Text displaying the current music volume percentage.")]
        [SerializeField]
        private TextMeshProUGUI _musicValueText;

        [Header("Buttons")]
        [Tooltip("Button to resume the game.")]
        [SerializeField]
        private Button _resumeButton;

        [Tooltip("Button to restart the level.")]
        [SerializeField]
        private Button _restartButton;

        [Header("Animation")]
        [Tooltip("Duration of the fade-in/fade-out animation.")]
        [SerializeField]
        private float _fadeTime = 0.3f;

        #endregion

        #region Constants

        private const string MUSIC_VOLUME_KEY = "MusicVolume";

        #endregion

        #region Private Fields

        private LevelManager _levelManager;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the SettingsPanelUI.
        /// </summary>
        [Inject]
        private void Construct(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        #endregion

        #region Lifecycle Methods

        private void Start()
        {
            InitializeUI();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the UI elements and sets up event listeners.
        /// </summary>
        private void InitializeUI()
        {
            SetupMusicSlider();
            SetupButtons();
            LoadSavedVolume();
        }

        /// <summary>
        /// Sets up the music slider and its event listener.
        /// </summary>
        private void SetupMusicSlider()
        {
            if (_musicSlider != null)
            {
                UpdateMusicValueText(_musicSlider.value);
                _musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
        }

        /// <summary>
        /// Sets up button listeners for UI interactions.
        /// </summary>
        private void SetupButtons()
        {
            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(Hide);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);
        }

        #endregion

        #region Audio Management

        /// <summary>
        /// Loads the saved music volume setting from PlayerPrefs.
        /// </summary>
        private void LoadSavedVolume()
        {
            //  float savedVolume = LoadManager.LoadData<float>(MUSIC_VOLUME_KEY, 1f);
              float savedVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 1f);
            if (_musicSlider != null)
            {
                _musicSlider.value = savedVolume;
                UpdateMusicValueText(savedVolume);
                ApplyVolumeToAudioSources(savedVolume);
            }
        }

        /// <summary>
        /// Updates the music volume when the slider value is changed.
        /// </summary>
        /// <param name="value">The new volume value (0 to 1).</param>
        private void OnMusicVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, value);
            PlayerPrefs.Save();
            UpdateMusicValueText(value);
            ApplyVolumeToAudioSources(value);
        }

        /// <summary>
        /// Updates the music volume percentage text.
        /// </summary>
        /// <param name="value">The current volume value (0 to 1).</param>
        private void UpdateMusicValueText(float value)
        {
            if (_musicValueText != null)
            {
                _musicValueText.text = $"{(value * 100):F0}%";
            }
        }

        /// <summary>
        /// Applies the volume setting to all audio sources in the scene.
        /// </summary>
        /// <param name="volume">The volume value to apply (0 to 1).</param>
        private void ApplyVolumeToAudioSources(float volume)
        {
            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (var source in audioSources)
            {
                source.volume = volume;
            }
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Handles the restart button click event.
        /// </summary>
        private void OnRestartClicked()
        {
            Hide();
            _levelManager.Restart();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the settings panel and pauses the game.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        /// <summary>
        /// Hides the settings panel and resumes the game.
        /// </summary>
        public void Hide()
        {
            Time.timeScale = 1;
            gameObject.SetActive(false);
        }

        #endregion
    }
}