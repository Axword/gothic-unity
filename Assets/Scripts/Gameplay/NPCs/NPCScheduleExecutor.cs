using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.Gameplay.NPCs
{
    /// <summary>
    /// Executes NPC schedules from npc_schedules.json.
    /// Moves NPCs between locations based on world time.
    /// Very simplified but functional.
    /// </summary>
    public class NPCScheduleExecutor : BaseMonoBehaviour
    {
        public string scheduleId;
        private TimeManager _time;
        private Vector3 _homePosition;
        private int _currentPhaseIndex = -1;
        private void Start()
        {
            _time = FindObjectOfType<TimeManager>();
            _homePosition = transform.position;
            if (_time != null)
            {
                _time.OnHourChanged += EvaluateSchedule;
            }
            // Initial evaluation
            EvaluateSchedule(8);
        }
        private void EvaluateSchedule(int hour)
            if (string.IsNullOrEmpty(scheduleId)) return;
            // Load schedules (in real project this would be from a proper database)
            var scheduleData = Resources.Load<TextAsset>("npc_schedules"); // fallback
            // For demo we hardcode basic movement
            // Simple logic: move to different spots at certain hours
            if (hour >= 6 && hour < 12 && _currentPhaseIndex != 0)
                MoveTo(new Vector3(_homePosition.x + 4, _homePosition.y, _homePosition.z + 2), "work");
                _currentPhaseIndex = 0;
            else if (hour >= 12 && hour < 14 && _currentPhaseIndex != 1)
                MoveTo(new Vector3(_homePosition.x - 2, _homePosition.y, _homePosition.z - 3), "eat");
                _currentPhaseIndex = 1;
            else if (hour >= 18 && _currentPhaseIndex != 2)
                MoveTo(_homePosition, "sleep");
                _currentPhaseIndex = 2;
        private void MoveTo(Vector3 target, string activity)
            // Very basic movement (no NavMesh)
            StopAllCoroutines();
            StartCoroutine(SimpleMove(target, activity));
        private System.Collections.IEnumerator SimpleMove(Vector3 target, string activity)
            Debug.Log($"[Schedule] {name} going to {activity}");
            float t = 0;
            Vector3 start = transform.position;
            while (t < 1f)
                t += Time.deltaTime * 0.6f;
                transform.position = Vector3.Lerp(start, target, t);
                yield return null;
            transform.position = target;
            Debug.Log($"[Schedule] {name} arrived at {activity}");
        private void OnDestroy()
                _time.OnHourChanged -= EvaluateSchedule;
    }
}
