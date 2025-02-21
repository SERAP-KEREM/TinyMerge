using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using _Game.Scripts.Management;

namespace _Game.Scripts.UI
{
    public class WinPanelUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Buttons")]
        [SerializeField] private Button _nextLevelButton;
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
            _nextLevelButton.onClick.AddListener(OnNextLevelClicked);
            _restartButton.onClick.AddListener(OnRestartClicked);
        }

        private void OnNextLevelClicked()
        {
            Hide();
            _levelManager.Next();
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
        }

        public void Hide()
        {
            _canvasGroup.DOFade(0, _fadeTime).OnComplete(() => {
                gameObject.SetActive(false);
            });
        }
    }
}