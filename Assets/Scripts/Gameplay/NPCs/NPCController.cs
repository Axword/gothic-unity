using System;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Schema;
using ZelaznaDroga.Gameplay.Dialogues;

namespace ZelaznaDroga.Gameplay.NPCs
{
    /// <summary>
    /// Basic NPC controller with schedule stub, dialogue trigger and simple AI.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class NPCController : BaseMonoBehaviour, IInteractable
    {
        [Header("Data")]
        public string npcId;
        public NpcData data;
        [Header("Runtime")]
        [SerializeField] private float detectionRange = 8f;
        private float _lastTalkTime;
        private CharacterController _cc;
        private DialogueManager _dialogueManager;
        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }
        private void Start()
            _dialogueManager = ComponentLocator.Get<DialogueManager>();
            
            // Load data if not set
            if (data == null && !string.IsNullOrEmpty(npcId))
            {
                // Would load from NpcDatabase in full impl
            }
        private void Update()
            // Very basic idle/patrol stub
            // In real impl use NavMesh + schedule
        public void Interact(GameObject interactor)
            if (Time.time - _lastTalkTime < 0.5f) return;
            _lastTalkTime = Time.time;
            if (!string.IsNullOrEmpty(data?.defaultGreeting))
                _dialogueManager?.StartDialogue(data.defaultGreeting, npcId);
            else
                Debug.Log($"[NPC] {name} says: Cześć, przybyszu. (placeholder)");
            // Trigger quest if linked
            var quests = ComponentLocator.Get<IQuestManager>();
            if (quests != null && data?.quests != null && data.quests.Count > 0)
                foreach (var q in data.quests)
                {
                    if (!quests.IsQuestActive(q) && !quests.IsQuestCompleted(q))
                    {
                        quests.StartQuest(q);
                        break;
                    }
                }
        public string GetInteractionLabel()
            return data != null ? $"Porozmawiaj z {data.nameKey}" : $"Porozmawiaj z {name}";
        // Stub schedule update
        public void UpdateSchedule(int hour)
            // Would move to markers based on npc_schedules.json
    }
}
