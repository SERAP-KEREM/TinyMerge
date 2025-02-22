using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;
using _Main._Data;
using TriInspector;

namespace _Main._Management
{
    /// <summary>
    /// Manages the game timer, including starting, stopping, freezing, and resetting the timer.
    /// Implements the ITimeManager interface and integrates with Zenject for dependency injection.
    /// </summary>
    public interface ITimeManager
    {
        void StartTimer(float timeInSeconds);
        void StopTimer();
        void ResetTimer();
        void AddExtraTime(float extraTimeInSeconds);
        void FreezeTimer(float duration);
        void SetTimeScale(float scale);

        event UnityAction<float> OnTimerUpdated;
        event UnityAction OnTimerFinished;
    }

    public class TimeManager : MonoBehaviour, ITimeManager, IInitializable
    {
        #region Serialized Fields

        [Header("Timer Settings")]
        [PropertyTooltip("Update interval for the timer (in seconds).")]
        private float _updateInterval = 0.1f;

        [PropertyTooltip("Threshold for critical time (in seconds).")]
        private float _criticalTimeThreshold = 10f;

        #endregion

        #region Private Fields

        private float _currentLevelTime;
        private bool _isTimerRunning;
        private GameData _gameData;

        #endregion

        #region Events

        public event UnityAction<float> OnTimerUpdated;
        public event UnityAction OnTimerFinished;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the TimeManager.
        /// </summary>
        [Inject]
        public void Construct(GameData gameData)
        {
            _gameData = gameData;
            _criticalTimeThreshold = _gameData.CurrentLevel.CriticalTimeThreshold;
        }

        #endregion

        #region Lifecycle Methods

        private void Start()
        {
            Initialize();
        }

        private void OnDestroy()
        {
            DOTween.Kill(this);
            CancelInvoke();
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the timer with the initial time from the current level configuration.
        /// </summary>
        public void Initialize()
        {
            StartTimer(_gameData.CurrentLevel.InitialTime);
        }

        #endregion

        #region Timer Management

        /// <summary>
        /// Starts the timer with the specified duration.
        /// </summary>
        /// <param name="timeInSeconds">The duration of the timer in seconds.</param>
        public void StartTimer(float timeInSeconds)
        {
            _currentLevelTime = timeInSeconds;
            _isTimerRunning = true;
            OnTimerUpdated?.Invoke(_currentLevelTime);
            ScheduleTimerUpdate();
        }

        /// <summary>
        /// Schedules the timer update using InvokeRepeating.
        /// </summary>
        private void ScheduleTimerUpdate()
        {
            CancelInvoke(nameof(UpdateTimer));
            InvokeRepeating(nameof(UpdateTimer), _updateInterval, _updateInterval);
        }

        /// <summary>
        /// Updates the timer by decrementing the remaining time and invoking events.
        /// </summary>
        private void UpdateTimer()
        {
            if (!_isTimerRunning) return;

            _currentLevelTime -= _updateInterval;

            if (_currentLevelTime <= 0)
            {
                HandleTimeExpired();
            }
            else
            {
                OnTimerUpdated?.Invoke(_currentLevelTime);
            }
        }

        /// <summary>
        /// Handles the logic when the timer expires.
        /// </summary>
        private void HandleTimeExpired()
        {
            _currentLevelTime = 0;
            _isTimerRunning = false;
            CancelInvoke(nameof(UpdateTimer));
            OnTimerFinished?.Invoke();
        }

        /// <summary>
        /// Adds extra time to the current timer.
        /// </summary>
        /// <param name="extraTimeInSeconds">The amount of extra time to add (in seconds).</param>
        public void AddExtraTime(float extraTimeInSeconds)
        {
            _currentLevelTime += extraTimeInSeconds;

            if (!_isTimerRunning)
            {
                ResumeTimer();
            }

            OnTimerUpdated?.Invoke(_currentLevelTime);
        }

        /// <summary>
        /// Resumes the timer after it has been stopped or frozen.
        /// </summary>
        private void ResumeTimer()
        {
            _isTimerRunning = true;
            ScheduleTimerUpdate();
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void StopTimer()
        {
            _isTimerRunning = false;
            CancelInvoke(nameof(UpdateTimer));
        }

        /// <summary>
        /// Resets the timer to the initial time from the current level configuration.
        /// </summary>
        public void ResetTimer()
        {
            StopTimer();
            StartTimer(_gameData.CurrentLevel.InitialTime);
        }

        /// <summary>
        /// Sets the global time scale for the game.
        /// </summary>
        /// <param name="scale">The time scale value to apply.</param>
        public void SetTimeScale(float scale)
        {
            Time.timeScale = scale;
        }

        /// <summary>
        /// Freezes the timer for a specified duration.
        /// </summary>
        /// <param name="duration">The duration to freeze the timer (in seconds).</param>
        public void FreezeTimer(float duration)
        {
            if (!_isTimerRunning) return;

            StopTimer();

            DOVirtual.DelayedCall(duration, () =>
            {
                if (_currentLevelTime > 0)
                {
                    ResumeTimer();
                }
            }).SetId(this);
        }

        #endregion
    }
}