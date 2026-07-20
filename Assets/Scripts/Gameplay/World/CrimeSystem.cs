using System;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Interaction;

namespace ZelaznaDroga.Gameplay.World
{
    /// <summary>
    /// Basic crime system.
    /// - Detects theft from chests/NPCs when witnessed.
    /// - Escalates wanted level.
    /// - NPCs react based on line of sight.
    /// </summary>
    public class CrimeSystem : BaseMonoBehaviour
    {
        public static CrimeSystem Instance { get; private set; }
        public int WantedLevel { get; private set; } = 0; // 0-5
        private float _lastCrimeTime;
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        public void ReportCrime(GameObject criminal, GameObject witness, string crimeType)
            if (criminal == null || witness == null) return;
            // Simple LOS check
            Vector3 dir = (criminal.transform.position - witness.transform.position);
            if (dir.magnitude > 15f) return;
            RaycastHit hit;
            if (Physics.Raycast(witness.transform.position + Vector3.up, dir.normalized, out hit, dir.magnitude))
            {
                if (hit.transform != criminal.transform) return; // blocked
            }
            int severity = 1;
            if (crimeType.Contains("theft") || crimeType.Contains("chest")) severity = 2;
            if (crimeType.Contains("murder")) severity = 4;
            WantedLevel = Mathf.Clamp(WantedLevel + severity, 0, 5);
            _lastCrimeTime = Time.time;
            Debug.Log($"[Crime] {witness.name} witnessed {crimeType}! Wanted level: {WantedLevel}");
            // Make witness react
            var npc = witness.GetComponent<NPCs.NPCController>();
            if (npc != null)
                // Simple: alert nearby guards
                if (WantedLevel >= 2)
                {
                    Debug.Log("[Crime] ALARM! Straż wezwana!");
                    // In real game: spawn guards or change NPC state
                }
            // Global consequence
            var rep = ComponentLocator.Get<Progression.IReputationSystem>();
            if (rep != null)
                rep.ChangeReputation("OldOrder", -severity * 3);
        public void ClearWanted()
            WantedLevel = 0;
        private void Update()
            // Wanted level slowly decays if no crime for a while
            if (WantedLevel > 0 && Time.time - _lastCrimeTime > 120f)
                WantedLevel = Mathf.Max(0, WantedLevel - 1);
                _lastCrimeTime = Time.time;
    }
}
