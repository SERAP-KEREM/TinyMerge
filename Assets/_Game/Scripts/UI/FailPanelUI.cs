using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using _Main._Management;

namespace _Main._UI
{
    /// <summary>
    /// Manages the fail panel UI, including showing/hiding animations and handling button interactions.
    /// </summary>
    public class FailPanelUI : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Panel")]
        [Tooltip("Canvas group for controlling the panel's visibility and fade effects.")]
        [SerializeField]
        private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        [Tooltip("Button to restart the level.")]
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
        /// Injects dependencies required for the FailPanelUI.
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
        /// Initializes the UI elements and sets up button listeners.
        /// </summary>
        private void InitializeUI()
        {
            if (_restartButton != null)
            {
                _restartButton.onClick.AddListener(OnRestartClicked);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the fail panel with a fade-in animation.
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
           
        }

        /// <summary>
        /// Hides the fail panel with a fade-out animation.
        /// </summary>
        public void Hide()
        { gameObject.SetActive(false);
            
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
    }
}