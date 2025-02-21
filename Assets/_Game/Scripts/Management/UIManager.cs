using _Game.Scripts.Management;
using UnityEngine;
using Zenject;

namespace _Game.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameplayUI _gameplayUI;
        [SerializeField] private SettingsPanelUI _settingsPanel;
        [SerializeField] private WinPanelUI _winPanel;
        [SerializeField] private FailPanelUI _failPanel;

        private LevelManager _levelManager;

        [Inject]
        private void Construct(LevelManager levelManager)
        {
            _levelManager = levelManager;
            SubscribeToEvents();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            if (_levelManager != null)
            {
                _levelManager.OnLevelCompleted += ShowWinPanel;
                _levelManager.OnLevelFailed += ShowFailPanel;
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (_levelManager != null)
            {
                _levelManager.OnLevelCompleted -= ShowWinPanel;
                _levelManager.OnLevelFailed -= ShowFailPanel;
            }
        }

        public void ShowSettingsPanel()
        {
            _settingsPanel.Show();
        }

        private void ShowWinPanel()
        {
            _winPanel.Show();
        }

        private void ShowFailPanel()
        {
            _failPanel.Show();
        }
    }
}