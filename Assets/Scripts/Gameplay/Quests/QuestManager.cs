using System;
using System.Collections.Generic;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
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

        #endregion

        #region Quest Data

        private Dictionary<string, QuestData> _allQuests = new Dictionary<string, QuestData>();
        private Dictionary<string, QuestInstance> _activeQuests = new Dictionary<string, QuestInstance>();
        private HashSet<string> _completedQuests = new HashSet<string>();
        private HashSet<string> _failedQuests = new HashSet<string>();

        #endregion

        #region Properties

        public IReadOnlyDictionary<string, QuestInstance> ActiveQuests => _activeQuests;
        public IReadOnlyCollection<string> CompletedQuests => _completedQuests;
        public IReadOnlyCollection<string> FailedQuests => _failedQuests;
        public int ActiveQuestCount => _activeQuests.Count;

        #endregion

        #region Events

        public event Action<QuestInstance> OnQuestStarted;
        public event Action<QuestInstance, int> OnQuestStageCompleted;
        public event Action<QuestInstance> OnQuestCompleted;
        public event Action<QuestInstance, string> OnQuestFailed;
        public event Action<string, int> OnObjectiveUpdated;

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
        }

        private void Start()
        {
            LoadQuestData();
            ComponentLocator.Register<IQuestManager>(new QuestManagerInterface(this));
            EventBus.Subscribe<NpcKilledEvent>(OnEntityKilled);
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            ComponentLocator.Unregister<IQuestManager>(new QuestManagerInterface(this));
            EventBus.Unsubscribe<NpcKilledEvent>(OnEntityKilled);
        }

        #endregion

        #region Data Loading

        private void LoadQuestData()
        {
            // Load main quests
            var mainQuests = _dataLoader.Load<List<QuestData>>("quests/quests_main.json");
            if (mainQuests != null)
            {
                foreach (var quest in mainQuests)
                {
                    _allQuests[quest.Id] = quest;
                }
            }

            // Load faction quests
            var oldFactionQuests = _dataLoader.Load<List<QuestData>>("quests/quests_old_faction.json");
            if (oldFactionQuests != null)
            {
                foreach (var quest in oldFactionQuests)
                {
                    _allQuests[quest.Id] = quest;
                }
            }

            var newFactionQuests = _dataLoader.Load<List<QuestData>>("quests/quests_new_faction.json");
            if (newFactionQuests != null)
            {
                foreach (var quest in newFactionQuests)
                {
                    _allQuests[quest.Id] = quest;
                }
            }

            // Load side quests
            var sideQuests = _dataLoader.Load<List<QuestData>>("quests/quests_side.json");
            if (sideQuests != null)
            {
                foreach (var quest in sideQuests)
                {
                    _allQuests[quest.Id] = quest;
                }
            }

            Debug.Log($"[QuestManager] Loaded {_allQuests.Count} quests");
        }

        #endregion

        #region Quest Operations

        /// <summary>
        /// Starts a new quest.
        /// </summary>
        public bool StartQuest(string questId)
        {
            if (!_allQuests.TryGetValue(questId, out QuestData questData))
            {
                Debug.LogWarning($"[QuestManager] Quest {questId} not found");
                return false;
            }

            if (_activeQuests.ContainsKey(questId))
            {
                Debug.LogWarning($"[QuestManager] Quest {questId} already active");
                return false;
            }

            if (_completedQuests.Contains(questId))
            {
                Debug.LogWarning($"[QuestManager] Quest {questId} already completed");
                return false;
            }

            // Check requirements
            if (!CheckRequirements(questData.requirements))
            {
                Debug.Log($"[QuestManager] Requirements not met for {questId}");
                return false;
            }

            // Create quest instance
            QuestInstance instance = new QuestInstance
            {
                QuestId = questId,
                CurrentStage = 0,
                ObjectivesProgress = new Dictionary<string, int>(),
                StartTime = DateTime.Now.ToString("o"),
                Data = questData
            };

            // Initialize objective progress
            foreach (var stage in questData.stages)
            {
                foreach (var objective in stage.objectives)
                {
                    if (!instance.ObjectivesProgress.ContainsKey(objective.target))
                    {
                        instance.ObjectivesProgress[objective.target] = 0;
                    }
                }
            }

            _activeQuests[questId] = instance;

            Debug.Log($"[QuestManager] Started quest: {questId}");
            EventBus.Publish(new QuestStartedEvent(questId));
            OnQuestStarted?.Invoke(instance);

            return true;
        }

        /// <summary>
        /// Advances to the next stage of a quest.
        /// </summary>
        public bool AdvanceQuestStage(string questId)
        {
            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            {
                return false;
            }

            var questData = instance.Data;
            if (questData == null)
            {
                return false;
            }

            int currentStage = instance.CurrentStage;
            if (currentStage < 0 || currentStage >= questData.stages.Count)
            {
                return false;
            }

            var stage = questData.stages[currentStage];

            // Check if all objectives are complete
            if (!AreStageObjectivesComplete(instance, stage))
            {
                Debug.Log($"[QuestManager] Objectives not complete for {questId} stage {currentStage}");
                return false;
            }

            // Execute stage completion actions
            ExecuteActions(stage.onComplete);

            // Move to next stage or complete quest
            instance.CurrentStage++;

            if (instance.CurrentStage >= questData.stages.Count)
            {
                CompleteQuest(questId);
            }
            else
            {
                Debug.Log($"[QuestManager] {questId} advanced to stage {instance.CurrentStage}");
                EventBus.Publish(new QuestStageCompletedEvent(questId, currentStage));
                OnQuestStageCompleted?.Invoke(instance, currentStage);
            }

            return true;
        }

        /// <summary>
        /// Completes a quest and gives rewards.
        /// </summary>
        public void CompleteQuest(string questId)
        {
            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            {
                return;
            }

            var questData = instance.Data;
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
            {
                StartQuest(questData.nextQuest);
            }
        }

        /// <summary>
        /// Fails a quest.
        /// </summary>
        public void FailQuest(string questId, string reason = null)
        {
            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            {
                return;
            }

            var questData = instance.Data;
            if (questData == null) return;

            _activeQuests.Remove(questId);
            _failedQuests.Add(questId);

            // Execute fail actions
            var currentStage = questData.stages[instance.CurrentStage];
            ExecuteActions(currentStage.onFail);

            Debug.Log($"[QuestManager] Failed quest: {questId} ({reason})");
            EventBus.Publish(new QuestFailedEvent(questId, reason));
            OnQuestFailed?.Invoke(instance, reason);
        }

        /// <summary>
        /// Abandons a quest (removes without penalty).
        /// </summary>
        public void AbandonQuest(string questId)
        {
            if (!_activeQuests.ContainsKey(questId))
            {
                return;
            }

            _activeQuests.Remove(questId);
            Debug.Log($"[QuestManager] Abandoned quest: {questId}");
        }

        #endregion

        #region Objective Tracking

        /// <summary>
        /// Updates progress on an objective.
        /// </summary>
        public void UpdateObjective(string questId, ObjectiveType type, string target, int count = 1)
        {
            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            {
                return;
            }

            var questData = instance.Data;
            if (questData == null) return;

            // Find matching objective
            foreach (var stage in questData.stages)
            {
                foreach (var objective in stage.objectives)
                {
                    if (objective.type == type && objective.target == target)
                    {
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
                        {
                            AdvanceQuestStage(questId);
                        }

                        return;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if all objectives for a stage are complete.
        /// </summary>
        private bool AreStageObjectivesComplete(QuestInstance instance, QuestStage stage)
        {
            foreach (var objective in stage.objectives)
            {
                if (objective.optional) continue;

                if (!instance.ObjectivesProgress.TryGetValue(objective.target, out int progress))
                {
                    progress = 0;
                }

                if (progress < objective.count)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Gets the current stage description for a quest.
        /// </summary>
        public string GetCurrentStageDescription(string questId)
        {
            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            {
                return null;
            }

            var questData = instance.Data;
            if (questData == null || instance.CurrentStage >= questData.stages.Count)
            {
                return null;
            }

            return questData.stages[instance.CurrentStage].descriptionKey;
        }

        /// <summary>
        /// Gets objectives for the current stage.
        /// </summary>
        public List<(QuestObjective obj, int progress)> GetCurrentStageObjectives(string questId)
        {
            var result = new List<(QuestObjective, int)>();

            if (!_activeQuests.TryGetValue(questId, out QuestInstance instance))
            {
                return result;
            }

            var questData = instance.Data;
            if (questData == null || instance.CurrentStage >= questData.stages.Count)
            {
                return result;
            }

            foreach (var objective in questData.stages[instance.CurrentStage].objectives)
            {
                int progress = instance.ObjectivesProgress.TryGetValue(objective.target, out int p) ? p : 0;
                result.Add((objective, progress));
            }

            return result;
        }

        #endregion

        #region Helpers

        private bool CheckRequirements(QuestRequirement requirements)
        {
            if (requirements == null) return true;

            // Check level
            if (requirements.level > 0)
            {
                var stats = ComponentLocator.Get<IPlayerStats>();
                if (stats != null && stats.Level < requirements.level)
                {
                    return false;
                }
            }

            // Check required quests
            foreach (var questId in requirements.quests)
            {
                if (!_completedQuests.Contains(questId))
                {
                    return false;
                }
            }

            // Check flags
            // (would need flag manager)

            // Check items
            foreach (var itemId in requirements.items)
            {
                var inventory = ComponentLocator.Get<IInventorySystem>();
                if (inventory != null && !inventory.HasItem(itemId))
                {
                    return false;
                }
            }

            return true;
        }

        private void ExecuteActions(StageActions actions)
        {
            if (actions == null) return;

            // Add/remove flags
            // (would need flag manager)

            // Give/remove items
            if (actions.giveItems != null)
            {
                var inventory = ComponentLocator.Get<IInventorySystem>();
                foreach (var itemId in actions.giveItems)
                {
                    inventory?.AddItem(itemId);
                }
            }

            if (actions.removeItems != null)
            {
                var inventory = ComponentLocator.Get<IInventorySystem>();
                foreach (var itemId in actions.removeItems)
                {
                    inventory?.RemoveItem(itemId);
                }
            }

            // Change reputation
            if (actions.changeReputation != null)
            {
                foreach (var kvp in actions.changeReputation)
                {
                    // Would need reputation manager
                    Debug.Log($"[QuestManager] Reputation change: {kvp.Key} {kvp.Value}");
                }
            }

            // XP reward
            if (actions.giveItems != null)
            {
                // XP is given via rewards
            }
        }

        private void GiveRewards(QuestRewards rewards)
        {
            if (rewards == null) return;

            // XP
            if (rewards.xp > 0)
            {
                var stats = ComponentLocator.Get<IPlayerStats>();
                stats?.AddXp(rewards.xp);
            }

            // Gold
            if (rewards.gold > 0)
            {
                var inventory = ComponentLocator.Get<IInventorySystem>();
                inventory?.AddGold(rewards.gold);
            }

            // Items
            if (rewards.items != null)
            {
                var inventory = ComponentLocator.Get<IInventorySystem>();
                foreach (var itemId in rewards.items)
                {
                    inventory?.AddItem(itemId);
                }
            }

            // Learning points
            if (rewards.skillPoints > 0)
            {
                var stats = ComponentLocator.Get<IPlayerStats>();
                // Would need to access internal method
            }
        }

        private void OnEntityKilled(NpcKilledEvent evt)
        {
            // Update any kill objectives
            foreach (var kvp in _activeQuests)
            {
                UpdateObjective(kvp.Key, ObjectiveType.Kill, evt.NpcId, 1);
            }
        }

        /// <summary>
        /// Checks if a quest is active.
        /// </summary>
        public bool IsQuestActive(string questId)
        {
            return _activeQuests.ContainsKey(questId);
        }

        /// <summary>
        /// Checks if a quest is completed.
        /// </summary>
        public bool IsQuestCompleted(string questId)
        {
            return _completedQuests.Contains(questId);
        }

        /// <summary>
        /// Gets a quest by ID.
        /// </summary>
        public QuestData GetQuest(string questId)
        {
            return _allQuests.GetValueOrDefault(questId);
        }

        #endregion

        #region Save/Load

        public ActiveQuestSaveData GetSaveData()
        {
            var data = new ActiveQuestSaveData();
            
            foreach (var kvp in _activeQuests)
            {
                data.quests.Add(new ActiveQuestInfo
                {
                    id = kvp.Key,
                    stageIndex = kvp.Value.CurrentStage,
                    stageObjectives = new Dictionary<string, int>(kvp.Value.ObjectivesProgress),
                    startTime = kvp.Value.StartTime
                });
            }

            return data;
        }

        public void LoadSaveData(ActiveQuestSaveData data, List<string> completed, List<string> failed)
        {
            _activeQuests.Clear();
            _completedQuests.Clear();
            _failedQuests.Clear();

            foreach (var quest in completed)
            {
                _completedQuests.Add(quest);
            }

            foreach (var quest in failed)
            {
                _failedQuests.Add(quest);
            }

            foreach (var info in data.quests)
            {
                if (_allQuests.TryGetValue(info.id, out QuestData questData))
                {
                    var instance = new QuestInstance
                    {
                        QuestId = info.id,
                        CurrentStage = info.stageIndex,
                        ObjectivesProgress = new Dictionary<string, int>(info.stageObjectives),
                        Data = questData
                    };
                    _activeQuests[info.id] = instance;
                }
            }

            Debug.Log($"[QuestManager] Loaded {_activeQuests.Count} active quests, {_completedQuests.Count} completed");
        }

        #endregion
    }

    #region Quest Instance

    /// <summary>
    /// Runtime instance of an active quest.
    /// </summary>
    public class QuestInstance
    {
        public string QuestId;
        public int CurrentStage;
        public Dictionary<string, int> ObjectivesProgress;
        public string StartTime;
        public QuestData Data;
    }

    #endregion

    #region Interface

    public interface IQuestManager
    {
        bool StartQuest(string questId);
        void CompleteQuest(string questId);
        void FailQuest(string questId, string reason = null);
        void UpdateObjective(string questId, ObjectiveType type, string target, int count = 1);
        bool IsQuestActive(string questId);
        bool IsQuestCompleted(string questId);
        int ActiveQuestCount { get; }
    }

    public class QuestManagerInterface : IQuestManager
    {
        private readonly QuestManager _manager;

        public QuestManagerInterface(QuestManager manager)
        {
            _manager = manager;
        }

        public bool StartQuest(string questId) => _manager.StartQuest(questId);
        public void CompleteQuest(string questId) => _manager.CompleteQuest(questId);
        public void FailQuest(string questId, string reason = null) => _manager.FailQuest(questId, reason);
        public void UpdateObjective(string questId, ObjectiveType type, string target, int count = 1) 
            => _manager.UpdateObjective(questId, type, target, count);
        public bool IsQuestActive(string questId) => _manager.IsQuestActive(questId);
        public bool IsQuestCompleted(string questId) => _manager.IsQuestCompleted(questId);
        public int ActiveQuestCount => _manager.ActiveQuestCount;
    }

    #endregion
}
