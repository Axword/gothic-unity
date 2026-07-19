using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZelaznaDroga.Core.Utilities
{
    /// <summary>
    /// Simple scheduler for delayed and periodic tasks.
    /// </summary>
    public class Scheduler : MonoBehaviour
    {
        private static Scheduler _instance;
        public static Scheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[Scheduler]");
                    _instance = go.AddComponent<Scheduler>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }

        private class ScheduledTask
        {
            public string Id;
            public float Delay;
            public float RemainingTime;
            public Action Callback;
            public bool IsRepeating;
            public float RepeatInterval;
            public bool IsValid = true;
        }

        private List<ScheduledTask> _tasks = new List<ScheduledTask>();
        private List<ScheduledTask> _toRemove = new List<ScheduledTask>();

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            for (int i = 0; i < _tasks.Count; i++)
            {
                ScheduledTask task = _tasks[i];
                if (!task.IsValid) continue;

                task.RemainingTime -= deltaTime;

                if (task.RemainingTime <= 0)
                {
                    try
                    {
                        task.Callback?.Invoke();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Scheduler: Error executing task {task.Id}: {ex}");
                    }

                    if (task.IsRepeating && task.IsValid)
                    {
                        task.RemainingTime = task.RepeatInterval;
                    }
                    else
                    {
                        task.IsValid = false;
                        _toRemove.Add(task);
                    }
                }
            }

            // Remove completed non-repeating tasks
            for (int i = 0; i < _toRemove.Count; i++)
            {
                _tasks.Remove(_toRemove[i]);
            }
            _toRemove.Clear();
        }

        /// <summary>
        /// Schedules a one-time delayed task.
        /// </summary>
        public string Schedule(float delay, Action callback, string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            ScheduledTask task = new ScheduledTask
            {
                Id = id,
                Delay = delay,
                RemainingTime = delay,
                Callback = callback,
                IsRepeating = false
            };

            _tasks.Add(task);
            return id;
        }

        /// <summary>
        /// Schedules a repeating task.
        /// </summary>
        public string ScheduleRepeating(float interval, Action callback, string id = null)
        {
            if (string.IsNullOrEmpty(id))
            {
                id = Guid.NewGuid().ToString();
            }

            ScheduledTask task = new ScheduledTask
            {
                Id = id,
                Delay = interval,
                RemainingTime = interval,
                Callback = callback,
                IsRepeating = true,
                RepeatInterval = interval
            };

            _tasks.Add(task);
            return id;
        }

        /// <summary>
        /// Cancels a scheduled task by ID.
        /// </summary>
        public bool Cancel(string id)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].Id == id)
                {
                    _tasks[i].IsValid = false;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if a task is scheduled.
        /// </summary>
        public bool IsScheduled(string id)
        {
            for (int i = 0; i < _tasks.Count; i++)
            {
                if (_tasks[i].Id == id && _tasks[i].IsValid)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Clears all scheduled tasks.
        /// </summary>
        public void ClearAll()
        {
            _tasks.Clear();
            _toRemove.Clear();
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
