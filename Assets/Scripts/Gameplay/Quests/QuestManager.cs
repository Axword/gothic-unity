using System;
using System.Collections.Generic;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Json;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.Gameplay.Quests
{
    /// <summary>
    /// Manages all quests, their states, and objectives.
    /// </summary>
    public class QuestManager : BaseMonoBehaviour
    {
        #region Singleton
        private static QuestManager _instance;
        public static QuestManager Instance => _instance;
        #endregion
        #region Dependencies
        [Header("Dependencies")]
        [SerializeField] private JsonDataLoader _dataLoader;
        #region Quest Data
        private Dictionary<string, QuestData> _allQuests = new Dictionary<string, QuestData>();
        private Dictionary<string, QuestInstance> _activeQuests = new Dictionary<string, QuestInstance>();
        private HashSet<string> _completedQuests = new HashSet<string>();
        private HashSet<string> _failedQuests = new HashSet<string>();
        #region Properties
        public IReadOnlyDictionary<string, QuestInstance> ActiveQuests => _activeQuests;
        public IReadOnlyCollection<string> CompletedQuests => _completedQuests;
        public IReadOnlyCollection<string> FailedQuests => _failedQuests;
        public int ActiveQuestCount => _activeQuests.Count;
        #region Events
        public event Action<QuestInstance> OnQuestStarted;
        public event Action<QuestInstance, int> OnQuestStageCompleted;
        public event Action<QuestInstance> OnQuestCompleted;
        public event Action<QuestInstance, string> OnQuestFailed;
        public event Action<string, int> OnObjectiveUpdated;
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
        }
        private void Start()
            LoadQuestData();
            ComponentLocator.Register<IQuestManager>(new QuestManagerInterface(this));
            EventBus.Subscribe<NpcKilledEvent>(OnEntityKilled);
        private void OnDestroy()
            if (_instance == this)
                _instance = null;
            ComponentLocator.Unregister<IQuestManager>(new QuestManagerInterface(this));
            EventBus.Unsubscribe<NpcKilledEvent>(OnEntityKilled);
        #region Data Loading
        private void LoadQuestData()
            // Load main quests
            var mainQuests = _dataLoader.Load<List<QuestData>>("quests/quests_main.json");
            if (mainQuests != null)
                foreach (var quest in mainQuests)
                {
                    _allQuests[quest.Id] = quest;
                }
            // Load faction quests
            var oldFactionQuests = _dataLoader.Load<List<QuestData>>("quests/quests_old_faction.json");
            if (oldFactionQuests != null)
                foreach (var quest in oldFactionQuests)
            var newFactionQuests = _dataLoader.Load<List<QuestData>>("quests/quests_new_faction.json");
            if (newFactionQuests != null)
                foreach (var quest in newFactionQuests)
            // Load side quests
            var sideQuests = _dataLoader.Load<List<QuestData>>("quests/quests_side.json");
            if (sideQuests != null)
                foreach (var quest in sideQuests)
            Debug.Log($"[QuestManager] Loaded {_allQuests.Count} quests");
        #region Quest Operations
        /// <summary>
        /// Starts a new quest.
        /// </summary>
        public bool StartQuest(string questId)
            if (!_allQuests.TryGetValue(questId, out QuestData questData))
                Debug.LogWarning($"[QuestManager] Quest {questId} not found");
                return false;
            if (_activeQuests.ContainsKey(questId))
                Debug.LogWarning($"[QuestManager] Quest {questId} already active");
            if (_completedQuests.Contains(questId))
                Debug.LogWarning($"[QuestManager] Quest {questId} already completed");
            // Check requirements
            if (!CheckRequirements(questData.requirements))
                Debug.Log($"[QuestManager] Requirements not met for {questId}");
            // Create quest instance
            QuestInstance instance = new QuestInstance
                QuestId = questId,
                CurrentStage = 0,
                ObjectivesProgress = new Dictionary<string, int>(),
                StartTime = DateTime.Now.ToString("o"),
                Data = questData
            };
            // Initialize objective progress
            foreach (var stage in questData.stages)
                foreach (var objective in stage.objectives)
                    if (!instance.ObjectivesProgress.ContainsKey(objective.target))
                    {
                        instance.ObjectivesProgress[objective.target] = 0;
                    }
            _activeQuests[questId] = instance;
            Debug.Log($"[QuestManager] Started quest: {questId}");
            EventBus.Publish(new QuestStartedEvent(questId));
            OnQuestStarted?.Invoke(instance);
            return true;
        /// Advances to the next stage of a quest.
        public bool AdvanceQuestStage(string questId)
            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            var questData = instance.Data;
            if (questData == null)
            int currentStage = instance.CurrentStage;
            if (currentStage < 0 || currentStage >= questData.stages.Count)
            var stage = questData.stages[currentStage];
            // Check if all objectives are complete
            if (!AreStageObjectivesComplete(instance, stage))
                Debug.Log($"[QuestManager] Objectives not complete for {questId} stage {currentStage}");
            // Execute stage completion actions
            ExecuteActions(stage.onComplete);
            // Move to next stage or complete quest
            instance.CurrentStage++;
            if (instance.CurrentStage >= questData.stages.Count)
                CompleteQuest(questId);
            else
                Debug.Log($"[QuestManager] {questId} advanced to stage {instance.CurrentStage}");
                EventBus.Publish(new QuestStageCompletedEvent(questId, currentStage));
                OnQuestStageCompleted?.Invoke(instance, currentStage);
        /// Completes a quest and gives rewards.
        public void CompleteQuest(string questId)
            if (questData == null) return;
            // Remove from active
            _activeQuests.Remove(questId);
            _completedQuests.Add(questId);
            // Give rewards
            GiveRewards(questData.rewards);
            Debug.Log($"[QuestManager] Completed quest: {questId}");
            EventBus.Publish(new QuestCompletedEvent(questId));
            OnQuestCompleted?.Invoke(instance);
            // Start next quest if linked
            if (!string.IsNullOrEmpty(questData.nextQuest))
                StartQuest(questData.nextQuest);
        /// Fails a quest.
        public void FailQuest(string questId, string reason = null)
            _failedQuests.Add(questId);
            // Execute fail actions
            var currentStage = questData.stages[instance.CurrentStage];
            ExecuteActions(currentStage.onFail);
            Debug.Log($"[QuestManager] Failed quest: {questId} ({reason})");
            EventBus.Publish(new QuestFailedEvent(questId, reason));
            OnQuestFailed?.Invoke(instance, reason);
        /// Abandons a quest (removes without penalty).
        public void AbandonQuest(string questId)
            if (!_activeQuests.ContainsKey(questId))
            Debug.Log($"[QuestManager] Abandoned quest: {questId}");
        #region Objective Tracking
        /// Updates progress on an objective.
        public void UpdateObjective(string questId, ObjectiveType type, string target, int count = 1)
            // Find matching objective
                    if (objective.type == type && objective.target == target)
                        // Update progress
                        if (!instance.ObjectivesProgress.ContainsKey(target))
                        {
                            instance.ObjectivesProgress[target] = 0;
                        }
                        instance.ObjectivesProgress[target] += count;
                        int current = instance.ObjectivesProgress[target];
                        int required = objective.count;
                        Debug.Log($"[QuestManager] {questId}: {target} {current}/{required}");
                        OnObjectiveUpdated?.Invoke(questId, current);
                        // Check if this stage is complete
                        if (AreStageObjectivesComplete(instance, stage))
                            AdvanceQuestStage(questId);
                        return;
        /// Checks if all objectives for a stage are complete.
        private bool AreStageObjectivesComplete(QuestInstance instance, QuestStage stage)
            foreach (var objective in stage.objectives)
                if (objective.optional) continue;
                if (!instance.ObjectivesProgress.TryGetValue(objective.target, out int progress))
                    progress = 0;
                if (progress < objective.count)
                    return false;
        /// Gets the current stage description for a quest.
        public string GetCurrentStageDescription(string questId)
                return null;
            if (questData == null || instance.CurrentStage >= questData.stages.Count)
            return questData.stages[instance.CurrentStage].descriptionKey;
        /// Gets objectives for the current stage.
        public List<(QuestObjective obj, int progress)> GetCurrentStageObjectives(string questId)
            var result = new List<(QuestObjective, int)>();
                return result;
            foreach (var objective in questData.stages[instance.CurrentStage].objectives)
                int progress = instance.ObjectivesProgress.TryGetValue(objective.target, out int p) ? p : 0;
                result.Add((objective, progress));
            return result;
        #region Helpers
        private bool CheckRequirements(QuestRequirement requirements)
            if (requirements == null) return true;
            // Check level
            if (requirements.level > 0)
                var stats = ComponentLocator.Get<IPlayerStats>();
                if (stats != null && stats.Level < requirements.level)
            // Check required quests
            foreach (var questId in requirements.quests)
                if (!_completedQuests.Contains(questId))
            // Check flags
            // (would need flag manager)
            // Check items
            foreach (var itemId in requirements.items)
                var inventory = ComponentLocator.Get<IInventorySystem>();
                if (inventory != null && !inventory.HasItem(itemId))
        private void ExecuteActions(StageActions actions)
            if (actions == null) return;
            // Add/remove flags
            // Give/remove items
            if (actions.giveItems != null)
                foreach (var itemId in actions.giveItems)
                    inventory?.AddItem(itemId);
            if (actions.removeItems != null)
                foreach (var itemId in actions.removeItems)
                    inventory?.RemoveItem(itemId);
            // Change reputation
            if (actions.changeReputation != null)
                foreach (var kvp in actions.changeReputation)
                    // Would need reputation manager
                    Debug.Log($"[QuestManager] Reputation change: {kvp.Key} {kvp.Value}");
            // XP reward
                // XP is given via rewards
        private void GiveRewards(QuestRewards rewards)
            if (rewards == null) return;
            // XP
            if (rewards.xp > 0)
                stats?.AddXp(rewards.xp);
            // Gold
            if (rewards.gold > 0)
                inventory?.AddGold(rewards.gold);
            // Items
            if (rewards.items != null)
                foreach (var itemId in rewards.items)
            // Learning points
            if (rewards.skillPoints > 0)
                // Would need to access internal method
        private void OnEntityKilled(NpcKilledEvent evt)
            // Update any kill objectives
            foreach (var kvp in _activeQuests)
                UpdateObjective(kvp.Key, ObjectiveType.Kill, evt.NpcId, 1);
        /// Checks if a quest is active.
        public bool IsQuestActive(string questId)
            return _activeQuests.ContainsKey(questId);
        /// Checks if a quest is completed.
        public bool IsQuestCompleted(string questId)
            return _completedQuests.Contains(questId);
        /// Gets a quest by ID.
        public QuestData GetQuest(string questId)
            return _allQuests.GetValueOrDefault(questId);
        #region Save/Load
        public ActiveQuestSaveData GetSaveData()
            var data = new ActiveQuestSaveData();
            
                data.quests.Add(new ActiveQuestInfo
                    id = kvp.Key,
                    stageIndex = kvp.Value.CurrentStage,
                    stageObjectives = new Dictionary<string, int>(kvp.Value.ObjectivesProgress),
                    startTime = kvp.Value.StartTime
                });
            return data;
        public void LoadSaveData(ActiveQuestSaveData data, List<string> completed, List<string> failed)
            _activeQuests.Clear();
            _completedQuests.Clear();
            _failedQuests.Clear();
            foreach (var quest in completed)
                _completedQuests.Add(quest);
            foreach (var quest in failed)
                _failedQuests.Add(quest);
            foreach (var info in data.quests)
                if (_allQuests.TryGetValue(info.id, out QuestData questData))
                    var instance = new QuestInstance
                        QuestId = info.id,
                        CurrentStage = info.stageIndex,
                        ObjectivesProgress = new Dictionary<string, int>(info.stageObjectives),
                        Data = questData
                    };
                    _activeQuests[info.id] = instance;
            Debug.Log($"[QuestManager] Loaded {_activeQuests.Count} active quests, {_completedQuests.Count} completed");
    }
    #region Quest Instance
    /// Runtime instance of an active quest.
    public class QuestInstance
        public string QuestId;
        public int CurrentStage;
        public Dictionary<string, int> ObjectivesProgress;
        public string StartTime;
        public QuestData Data;
    #endregion
    #region Interface
    public interface IQuestManager
        bool StartQuest(string questId);
        void CompleteQuest(string questId);
        void FailQuest(string questId, string reason = null);
        void UpdateObjective(string questId, ObjectiveType type, string target, int count = 1);
        bool IsQuestActive(string questId);
        bool IsQuestCompleted(string questId);
        int ActiveQuestCount { get; }
    public class QuestManagerInterface : IQuestManager
        private readonly QuestManager _manager;
        public QuestManagerInterface(QuestManager manager)
            _manager = manager;
        public bool StartQuest(string questId) => _manager.StartQuest(questId);
        public void CompleteQuest(string questId) => _manager.CompleteQuest(questId);
        public void FailQuest(string questId, string reason = null) => _manager.FailQuest(questId, reason);
        public void UpdateObjective(string questId, ObjectiveType type, string target, int count = 1) 
            => _manager.UpdateObjective(questId, type, target, count);
        public bool IsQuestActive(string questId) => _manager.IsQuestActive(questId);
        public bool IsQuestCompleted(string questId) => _manager.IsQuestCompleted(questId);
        public int ActiveQuestCount => _manager.ActiveQuestCount;
}
