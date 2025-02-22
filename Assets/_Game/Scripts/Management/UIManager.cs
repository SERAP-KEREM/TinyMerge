using _Main._UI;
using DG.Tweening;
using System.Collections;
using TriInspector;
using UnityEngine;

namespace _Main._Management
{
    /// <summary>
    /// Manages the UI elements in the game, including panels, freeze screens, and animations.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Gameplay UI")]
        [PropertyTooltip("The main gameplay UI.")]
        [SerializeField]
        private GameplayUI _gameplayUI;

        [PropertyTooltip("The settings panel UI.")]
        [SerializeField]
        private SettingsPanelUI _settingsPanel;

        [PropertyTooltip("The win panel UI.")]
        [SerializeField]
        private WinPanelUI _winPanel;

        [PropertyTooltip("The fail panel UI.")]
        [SerializeField]
        private FailPanelUI _failPanel;

        [Header("Freeze Screen")]
        [PropertyTooltip("Canvas group for the freeze screen effect.")]
        [SerializeField]
        private CanvasGroup _freezeScreenCanvasGroup;

        #endregion

        #region Public Methods

        /// <summary>
        /// Shows the settings panel.
        /// </summary>
        public void ShowSettingsPanel()
        {
            _settingsPanel.Show();
        }

        /// <summary>
        /// Shows the win panel.
        /// </summary>
        public void ShowWinPanel()
        {
            _winPanel.Show();
        }

        /// <summary>
        /// Shows the fail panel.
        /// </summary>
        public void ShowFailPanel()
        {
            _failPanel.Show();
        }

        /// <summary>
        /// Activates the freeze screen effect with fade-in and fade-out animations.
        /// </summary>
        /// <param name="duration">Total duration of the freeze effect.</param>
        /// <param name="fadeInDuration">Duration of the fade-in animation.</param>
        /// <param name="fadeOutDuration">Duration of the fade-out animation.</param>
        public void ActivateFreezeScreen(float duration, float fadeInDuration = 1f, float fadeOutDuration = 1f)
        {
            if (_freezeScreenCanvasGroup == null)
            {
                Debug.LogWarning("Freeze screen canvas group is not assigned!");
                return;
            }

            StartCoroutine(FreezeScreenRoutine(duration, fadeInDuration, fadeOutDuration));
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Coroutine to handle the freeze screen effect with fade-in and fade-out animations.
        /// </summary>
        /// <param name="duration">Total duration of the freeze effect.</param>
        /// <param name="fadeInDuration">Duration of the fade-in animation.</param>
        /// <param name="fadeOutDuration">Duration of the fade-out animation.</param>
        private IEnumerator FreezeScreenRoutine(float duration, float fadeInDuration, float fadeOutDuration)
        {
            // Fade in
            _freezeScreenCanvasGroup.gameObject.SetActive(true);
            _freezeScreenCanvasGroup.alpha = 0f;
            _freezeScreenCanvasGroup.DOFade(1f, fadeInDuration);

            // Wait for the specified duration minus fade durations
            yield return new WaitForSeconds(duration - fadeInDuration - fadeOutDuration);

            // Fade out
            _freezeScreenCanvasGroup.DOFade(0f, fadeOutDuration).OnComplete(() =>
            {
                _freezeScreenCanvasGroup.gameObject.SetActive(false);
            });
        }

        #endregion
    }
}