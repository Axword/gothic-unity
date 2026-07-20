using System;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.World
{
    /// <summary>
    /// Manages in-game time, day/night cycle, and world state.
    /// </summary>
    public class TimeManager : BaseMonoBehaviour
    {
        #region Singleton

        private static TimeManager _instance;
        public static TimeManager Instance => _instance;

        #endregion

        #region Time Settings

        [Header("Time Settings")]
        [SerializeField] private int _startingHour = 8;
        [SerializeField] private int _startingDay = 1;
        [SerializeField] private float _secondsPerGameHour = GameConstants.DAY_MINUTE_DURATION;
        [SerializeField] private float _timeScale = 1f;
        [SerializeField] private bool _paused;

        [Header("Day/Night Settings")]
        [SerializeField] private Gradient _ambientLightGradient;
        [SerializeField] private Gradient _directionalLightGradient;
        [SerializeField] private float _nightIntensity = 0.3f;
        [SerializeField] private float _dayIntensity = 1.2f;

        [Header("References")]
        [SerializeField] private Light _sunLight;

        #endregion

        #region State

        private float _gameTime; // Total game time in seconds
        private int _currentHour;
        private int _currentMinute;
        private int _currentDay = 1;
        private TimeOfDay _currentTimeOfDay;

        #endregion

        #region Properties

        public int CurrentHour => _currentHour;
        public int CurrentMinute => _currentMinute;
        public int CurrentDay => _currentDay;
        public float GameTime => _gameTime;
        public float TimeScale => _timeScale;
        public bool IsPaused => _paused;
        public TimeOfDay CurrentTimeOfDay => _currentTimeOfDay;

        public float DayProgress => (float)_currentHour / 24f;
        public bool IsNight => _currentHour >= 20 || _currentHour < 6;

        #endregion

        #region Events

        public event Action<int, TimeOfDay> OnTimeOfDayChanged;
        public event Action<int> OnHourChanged;
        public event Action OnDayChanged;
        public event Action<float> OnTimeScaleChanged;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);

            _gameTime = _startingHour * _secondsPerGameHour;
            UpdateTime();
        }

        private void Start()
        {
            ComponentLocator.Register<ITimeManager>(new TimeManagerInterface(this));
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            ComponentLocator.Unregister<ITimeManager>(new TimeManagerInterface(this));
        }

        private void Update()
        {
            if (_paused) return;

            AdvanceTime(Time.deltaTime);
        }

        #endregion

        #region Time Control

        /// <summary>
        /// Advances game time.
        /// </summary>
        public void AdvanceTime(float deltaTime)
        {
            float previousHour = _currentHour;
            int previousDay = _currentDay;

            _gameTime += deltaTime * _timeScale * _secondsPerGameHour;

            // Calculate hour and day
            int totalHours = Mathf.FloorToInt(_gameTime);
            _currentDay = 1 + (totalHours / 24);
            _currentHour = totalHours % 24;
            _currentMinute = Mathf.FloorToInt((_gameTime % 1) * 60);

            // Update time of day
            TimeOfDay previousTod = _currentTimeOfDay;
            _currentTimeOfDay = GetTimeOfDay(_currentHour);

            // Fire events
            if (_currentHour != previousHour)
            {
                OnHourChanged?.Invoke(_currentHour);
                
                if (_currentTimeOfDay != previousTod)
                {
                    EventBus.Publish(new TimeOfDayChangedEvent(_currentHour, _currentTimeOfDay));
                    OnTimeOfDayChanged?.Invoke(_currentHour, _currentTimeOfDay);
                }
            }

            if (_currentDay != previousDay)
            {
                OnDayChanged?.Invoke();
            }

            // Update lighting
            UpdateLighting();
        }

        /// <summary>
        /// Sets the time scale.
        /// </summary>
        public void SetTimeScale(float scale)
        {
            _timeScale = Mathf.Max(0, scale);
            OnTimeScaleChanged?.Invoke(_timeScale);
        }

        /// <summary>
        /// Pauses time.
        /// </summary>
        public void Pause()
        {
            _paused = true;
        }

        /// <summary>
        /// Resumes time.
        /// </summary>
        public void Resume()
        {
            _paused = false;
        }

        /// <summary>
        /// Toggles pause.
        /// </summary>
        public void TogglePause()
        {
            _paused = !_paused;
        }

        /// <summary>
        /// Sets the current hour (for testing/skip).
        /// </summary>
        public void SetTime(int hour, int minute = 0)
        {
            _currentHour = Mathf.Clamp(hour, 0, 23);
            _currentMinute = Mathf.Clamp(minute, 0, 59);
            _gameTime = (_currentDay - 1) * 24 * _secondsPerGameHour + 
                        _currentHour * _secondsPerGameHour + 
                        (_currentMinute / 60f) * _secondsPerGameHour;
            
            UpdateTime();
            UpdateLighting();
        }

        /// <summary>
        /// Advances to next day.
        /// </summary>
        public void NextDay()
        {
            SetTime(6, 0); // Dawn of next day
        }

        /// <summary>
        /// Waits until specified hour (used for sleeping).
        /// </summary>
        public void WaitUntil(int hour)
        {
            if (_currentHour >= hour)
            {
                SetTime(hour);
            }
            else
            {
                SetTimeScale(GameConstants.FAST_TIME_SCALE);
                // Would need coroutine or update loop check
            }
        }

        #endregion

        #region Private Methods

        private void UpdateTime()
        {
            _currentTimeOfDay = GetTimeOfDay(_currentHour);
        }

        private TimeOfDay GetTimeOfDay(int hour)
        {
            if (hour >= 5 && hour < 7) return TimeOfDay.Dawn;
            if (hour >= 7 && hour < 12) return TimeOfDay.Morning;
            if (hour >= 12 && hour < 14) return TimeOfDay.Noon;
            if (hour >= 14 && hour < 18) return TimeOfDay.Afternoon;
            if (hour >= 18 && hour < 20) return TimeOfDay.Dusk;
            if (hour >= 20 || hour < 5) return TimeOfDay.Night;
            if (hour == 5) return TimeOfDay.Night;
            return TimeOfDay.Night;
        }

        private void UpdateLighting()
        {
            if (_sunLight == null) return;

            float dayProgress = DayProgress;

            // Update sun rotation based on time
            float sunAngle = (dayProgress * 360f) - 90f;
            _sunLight.transform.rotation = Quaternion.Euler(sunAngle, -30f, 0f);

            // Update sun intensity
            float sunHeight = Mathf.Sin(dayProgress * Mathf.PI);
            float intensity = Mathf.Lerp(_nightIntensity, _dayIntensity, sunHeight);
            _sunLight.intensity = intensity;

            // Update sun color
            _sunLight.color = _directionalLightGradient.Evaluate(dayProgress);

            // Update ambient light
            RenderSettings.ambientLight = _ambientLightGradient.Evaluate(dayProgress);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Gets a formatted time string.
        /// </summary>
        public string GetFormattedTime()
        {
            return $"{_currentHour:D2}:{_currentMinute:D2}";
        }

        /// <summary>
        /// Gets a formatted date string.
        /// </summary>
        public string GetFormattedDate()
        {
            return $"Dzień {_currentDay}";
        }

        /// <summary>
        /// Gets full formatted datetime.
        /// </summary>
        public string GetFormattedDateTime()
        {
            return $"{GetFormattedDate()}, {GetFormattedTime()}";
        }

        /// <summary>
        /// Checks if it's a specific time of day.
        /// </summary>
        public bool IsTimeOfDay(TimeOfDay timeOfDay)
        {
            return _currentTimeOfDay == timeOfDay;
        }

        #endregion

        #region Save/Load

        public TimeSaveData GetSaveData()
        {
            return new TimeSaveData
            {
                gameTime = _gameTime,
                currentHour = _currentHour,
                currentMinute = _currentMinute,
                currentDay = _currentDay,
                timeScale = _timeScale
            };
        }

        public void LoadSaveData(TimeSaveData data)
        {
            _gameTime = data.gameTime;
            _currentHour = data.currentHour;
            _currentMinute = data.currentMinute;
            _currentDay = data.currentDay;
            _timeScale = data.timeScale;
            _paused = false;

            UpdateTime();
            UpdateLighting();
        }

        #endregion
    }

    [Serializable]
    public class TimeSaveData
    {
        public float gameTime;
        public int currentHour;
        public int currentMinute;
        public int currentDay;
        public float timeScale;
    }

    #region Interface

    public interface ITimeManager
    {
        int CurrentHour { get; }
        int CurrentMinute { get; }
        int CurrentDay { get; }
        TimeOfDay CurrentTimeOfDay { get; }
        bool IsNight { get; }
        bool IsPaused { get; }

        void Pause();
        void Resume();
        void SetTime(int hour, int minute = 0);
        void SetTimeScale(float scale);
        string GetFormattedTime();
    }

    public class TimeManagerInterface : ITimeManager
    {
        private readonly TimeManager _manager;

        public TimeManagerInterface(TimeManager manager)
        {
            _manager = manager;
        }

        public int CurrentHour => _manager.CurrentHour;
        public int CurrentMinute => _manager.CurrentMinute;
        public int CurrentDay => _manager.CurrentDay;
        public TimeOfDay CurrentTimeOfDay => _manager.CurrentTimeOfDay;
        public bool IsNight => _manager.IsNight;
        public bool IsPaused => _manager.IsPaused;

        public void Pause() => _manager.Pause();
        public void Resume() => _manager.Resume();
        public void SetTime(int hour, int minute = 0) => _manager.SetTime(hour, minute);
        public void SetTimeScale(float scale) => _manager.SetTimeScale(scale);
        public string GetFormattedTime() => _manager.GetFormattedTime();
    }

    #endregion
}
