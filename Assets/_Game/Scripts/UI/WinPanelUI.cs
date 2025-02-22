using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using _Main._Management;

namespace _Main._UI
{
    /// <summary>
    /// Manages the win panel UI, including showing/hiding animations and handling button interactions.
    /// </summary>
    public class WinPanelUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Panel")]
        [Tooltip("Canvas group for controlling the panel's visibility and fade effects.")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        [Tooltip("Button to proceed to the next level.")]
        [SerializeField]
        private Button _nextLevelButton;

        [Tooltip("Button to restart the current level.")]
        [SerializeField]
        private Button _restartButton;

        [Header("Animation")]
        [Tooltip("Duration of the fade-in/fade-out animation.")]
        [SerializeField]
        private float _fadeTime = 0.3f;

        #endregion

        #region Private Fields

        private LevelManager _levelManager;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the WinPanelUI.
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
            // Optionally hide the panel on start
            // Hide();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the UI elements and sets up button listeners.
        /// </summary>
        private void InitializeUI()
        {
            if (_nextLevelButton != null)
                _nextLevelButton.onClick.AddListener(OnNextLevelClicked);

            if (_restartButton != null)
                _restartButton.onClick.AddListener(OnRestartClicked);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the win panel with a fade-in animation.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;
            _canvasGroup.DOFade(1f, _fadeTime);
        }

        /// <summary>
        /// Hides the win panel with a fade-out animation.
        /// </summary>
        public void Hide()
        {
            _canvasGroup.DOFade(0f, _fadeTime).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
        }

        #endregion

        #region Button Handlers

        /// <summary>
        /// Handles the "Next Level" button click event.
        /// </summary>
        private void OnNextLevelClicked()
        {
            Hide();
            _levelManager.Next();
        }

        /// <summary>
        /// Handles the "Restart" button click event.
        /// </summary>
        private void OnRestartClicked()
        {
            Hide();
            _levelManager.Restart();
        }

        #endregion
    }
}