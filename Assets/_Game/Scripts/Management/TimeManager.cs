using UnityEngine;
using Zenject;
using System;
using DG.Tweening;

namespace _Game.Scripts.Services
{
    public interface ITimeManager
    {
        void StartTimer(float duration);
        void StopTimer();
        void PauseTimer();
        void ResumeTimer();
        void ResetTimer();
        void AddExtraTime(float seconds);
        void FreezeTimer(float duration);
        void SetTimeScale(float scale);
        float GetCurrentTime();
        bool IsTimerRunning();

        event Action<float> OnTimerUpdated;
        event Action OnTimerStarted;
        event Action OnTimerPaused;
        event Action OnTimerResumed;
        event Action OnTimerFinished;
    }

    public class TimeManager : MonoBehaviour, ITimeManager, IInitializable
    {
        private float _currentTime;
        private float _initialDuration;
        private bool _isTimerRunning;
        private bool _isPaused;
        private Sequence _freezeSequence;

        public event Action<float> OnTimerUpdated;
        public event Action OnTimerStarted;
        public event Action OnTimerPaused;
        public event Action OnTimerResumed;
        public event Action OnTimerFinished;

        public void Initialize()
        {
            _currentTime = 0;
            _initialDuration = 0;
            _isTimerRunning = false;
            _isPaused = false;
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            if (_freezeSequence != null)
            {
                _freezeSequence.Kill();
                _freezeSequence = null;
            }
        }

        private void Update()
        {
            if (!_isTimerRunning || _isPaused) return;

            _currentTime -= Time.deltaTime;

            if (_currentTime <= 0)
            {
                HandleTimerFinished();
            }
            else
            {
                OnTimerUpdated?.Invoke(_currentTime);
            }
        }

        public void StartTimer(float duration)
        {
            if (duration <= 0)
            {
                Debug.LogError("Timer duration must be greater than 0");
                return;
            }

            _initialDuration = duration;
            _currentTime = duration;
            _isTimerRunning = true;
            _isPaused = false;

            OnTimerStarted?.Invoke();
            OnTimerUpdated?.Invoke(_currentTime);
        }

        public void StopTimer()
        {
            _isTimerRunning = false;
            _isPaused = false;
            if (_freezeSequence != null)
            {
                _freezeSequence.Kill();
                _freezeSequence = null;
            }
        }

        public void PauseTimer()
        {
            if (!_isTimerRunning || _isPaused) return;

            _isPaused = true;
            OnTimerPaused?.Invoke();
        }

        public void ResumeTimer()
        {
            if (!_isTimerRunning || !_isPaused) return;

            _isPaused = false;
            OnTimerResumed?.Invoke();
        }

        public void ResetTimer()
        {
            _currentTime = _initialDuration;
            _isTimerRunning = true;
            _isPaused = false;

            OnTimerStarted?.Invoke();
            OnTimerUpdated?.Invoke(_currentTime);
        }

        public void AddExtraTime(float seconds)
        {
            if (seconds <= 0)
            {
                Debug.LogError("Extra time must be greater than 0");
                return;
            }

            _currentTime += seconds;
            OnTimerUpdated?.Invoke(_currentTime);

            if (!_isTimerRunning)
            {
                ResumeTimer();
            }
        }

        public void FreezeTimer(float duration)
        {
            if (duration <= 0)
            {
                Debug.LogError("Freeze duration must be greater than 0");
                return;
            }

            if (_freezeSequence != null)
            {
                _freezeSequence.Kill();
            }

            _freezeSequence = DOTween.Sequence();
            _freezeSequence.AppendCallback(() => PauseTimer());
            _freezeSequence.AppendInterval(duration);
            _freezeSequence.AppendCallback(() => ResumeTimer());
            _freezeSequence.Play();
        }

        public void SetTimeScale(float scale)
        {
            if (scale < 0)
            {
                Debug.LogError("Time scale cannot be negative");
                return;
            }

            Time.timeScale = scale;
        }

        public float GetCurrentTime()
        {
            return _currentTime;
        }

        public bool IsTimerRunning()
        {
            return _isTimerRunning && !_isPaused;
        }

        private void HandleTimerFinished()
        {
            _currentTime = 0;
            _isTimerRunning = false;
            _isPaused = false;

            OnTimerUpdated?.Invoke(_currentTime);
            OnTimerFinished?.Invoke();
        }
    }
}