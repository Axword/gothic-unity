using System;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.World
{
    /// <summary>
    /// World time manager: day/night cycle, hours.
    /// </summary>
    public class TimeManager : BaseMonoBehaviour
    {
        [Header("Time Settings")]
        [SerializeField] private float timeScale = 1f; // game minutes per real second
        [SerializeField] private int startHour = 8;
        private float _currentTime; // in hours (0-24)
        private int _currentHour;
        private int _currentMinute;
        public int CurrentHour => _currentHour;
        public float CurrentTime => _currentTime;
        public event Action<int> OnHourChanged;
        public event Action<float> OnTimeChanged;
        protected override void Awake()
        {
            base.Awake();
            _currentTime = startHour;
            ComponentLocator.Register<ITimeManager>(new TimeManagerInterface(this));
        }
        private void Update()
            float deltaHours = (Time.deltaTime * timeScale) / 60f; // assuming timeScale = real seconds per game hour? adjust
            _currentTime += deltaHours * 60f / 60f; // simplified
            // Better: 1 real second = X game minutes
            _currentTime += (Time.deltaTime / 60f) * 20f; // ~20 game minutes per real second for fast day
            if (_currentTime >= 24f) _currentTime -= 24f;
            int newHour = Mathf.FloorToInt(_currentTime);
            if (newHour != _currentHour)
            {
                _currentHour = newHour;
                OnHourChanged?.Invoke(_currentHour);
                Debug.Log($"[Time] Hour: {_currentHour}:00");
            }
            OnTimeChanged?.Invoke(_currentTime);
        public void SetTime(int hour)
            _currentTime = hour;
            _currentHour = hour;
        public void AccelerateTime(float factor)
            timeScale = factor;
    }
    public interface ITimeManager
        int CurrentHour { get; }
        float CurrentTime { get; }
    public class TimeManagerInterface : ITimeManager
        private readonly TimeManager _tm;
        public TimeManagerInterface(TimeManager tm) { _tm = tm; }
        public int CurrentHour => _tm.CurrentHour;
        public float CurrentTime => _tm.CurrentTime;
}
