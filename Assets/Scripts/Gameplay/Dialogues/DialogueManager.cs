using System;
using System.Collections.Generic;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Json;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.Gameplay.Dialogues
{
    /// <summary>
    /// Manages dialogue trees, conversations, and NPC interactions.
    /// </summary>
    public class DialogueManager : BaseMonoBehaviour
    {
        #region Singleton

        private static DialogueManager _instance;
        public static DialogueManager Instance => _instance;

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        [SerializeField] private JsonDataLoader _dataLoader;

        #endregion

        #region Dialogue State

        private Dictionary<string, DialogueTree> _dialogues = new Dictionary<string, DialogueTree>();
        private DialogueTree _currentDialogue;
        private DialogueNode _currentNode;
        private string _currentNpcId;
        private bool _isInDialogue;
        private Stack<string> _nodeHistory = new Stack<string>();

        #endregion

        #region Properties

        public bool IsInDialogue => _isInDialogue;
        public DialogueNode CurrentNode => _currentNode;
        public string CurrentNpcId => _currentNpcId;

        #endregion

        #region Events

        public event Action<DialogueTree, string> OnDialogueStarted;
        public event Action<DialogueNode> OnNodeChanged;
        public event Action OnDialogueEnded;

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
            LoadAllDialogues();
            ComponentLocator.Register<IDialogueManager>(new DialogueManagerInterface(this));
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
            ComponentLocator.Unregister<IDialogueManager>(new DialogueManagerInterface(this));
        }

        #endregion

        #region Data Loading

        private void LoadAllDialogues()
        {
            // Load all dialogue files
            string[] folders = { "dialogues" };
            
            foreach (var folder in folders)
            {
                var dialogues = _dataLoader.LoadAll<DialogueTree>(folder);
                foreach (var dialogue in dialogues)
                {
                    if (!string.IsNullOrEmpty(dialogue.Id))
                    {
                        _dialogues[dialogue.Id] = dialogue;
                    }
                }
            }

            Debug.Log($"[DialogueManager] Loaded {_dialogues.Count} dialogues");
        }

        #endregion

        #region Dialogue Control

        /// <summary>
        /// Starts a dialogue with an NPC.
        /// </summary>
        public bool StartDialogue(string dialogueId, string npcId)
        {
            if (_isInDialogue)
            {
                Debug.LogWarning("[DialogueManager] Already in dialogue");
                return false;
            }

            if (!_dialogues.TryGetValue(dialogueId, out DialogueTree tree))
            {
                Debug.LogWarning($"[DialogueManager] Dialogue {dialogueId} not found");
                return false;
            }

            _currentDialogue = tree;
            _currentNpcId = npcId;
            _isInDialogue = true;
            _nodeHistory.Clear();

            // Find root node
            if (string.IsNullOrEmpty(tree.rootNodeId) || !tree.nodes.TryGetValue(tree.rootNodeId, out DialogueNode rootNode))
            {
                Debug.LogError($"[DialogueManager] Root node not found for dialogue {dialogueId}");
                return false;
            }

            _currentNode = rootNode;

            // Execute node enter actions
            ExecuteActions(_currentNode.onEnter);

            Debug.Log($"[DialogueManager] Started dialogue {dialogueId} with NPC {npcId}");
            EventBus.Publish(new DialogueStartedEvent(dialogueId, npcId));
            OnDialogueStarted?.Invoke(_currentDialogue, _currentNpcId);
            OnNodeChanged?.Invoke(_currentNode);

            return true;
        }

        /// <summary>
        /// Selects a dialogue choice.
        /// </summary>
        public void SelectChoice(int choiceIndex)
        {
            if (!_isInDialogue || _currentNode == null)
            {
                return;
            }

            if (choiceIndex < 0 || choiceIndex >= _currentNode.choices.Count)
            {
                Debug.LogWarning($"[DialogueManager] Invalid choice index {choiceIndex}");
                return;
            }

            DialogueChoice choice = _currentNode.choices[choiceIndex];

            // Check conditions
            if (!CheckConditions(choice.conditions))
            {
                // Use fail node if available
                if (!string.IsNullOrEmpty(choice.failNode))
                {
                    GoToNode(choice.failNode);
                }
                return;
            }

            // Execute choice actions
            ExecuteActions(choice.actions);

            // Move to next node
            if (!string.IsNullOrEmpty(choice.nextNode))
            {
                GoToNode(choice.nextNode);
            }
            else
            {
                EndDialogue();
            }
        }

        /// <summary>
        /// Navigates to a specific node.
        /// </summary>
        private void GoToNode(string nodeId)
        {
            if (_currentDialogue == null || !_currentDialogue.nodes.TryGetValue(nodeId, out DialogueNode node))
            {
                Debug.LogError($"[DialogueManager] Node {nodeId} not found");
                EndDialogue();
                return;
            }

            // Track history for back functionality
            _nodeHistory.Push(_currentNode.nodeId);

            // Execute exit actions on current node
            ExecuteActions(_currentNode.onExit);

            _currentNode = node;

            // Execute enter actions on new node
            ExecuteActions(_currentNode.onEnter);

            Debug.Log($"[DialogueManager] Moved to node {nodeId}");
            OnNodeChanged?.Invoke(_currentNode);

            // Auto-advance if no choices (text-only node)
            if (_currentNode.choices.Count == 0)
            {
                // This would be handled by UI - wait for player click
            }
        }

        /// <summary>
        /// Ends the current dialogue.
        /// </summary>
        public void EndDialogue()
        {
            if (!_isInDialogue) return;

            // Execute exit actions
            ExecuteActions(_currentNode?.onExit);

            Debug.Log($"[DialogueManager] Ended dialogue with {_currentNpcId}");
            
            string dialogueId = _currentDialogue?.Id;
            _currentDialogue = null;
            _currentNode = null;
            _currentNpcId = null;
            _isInDialogue = false;
            _nodeHistory.Clear();

            EventBus.Publish(new DialogueEndedEvent(dialogueId, true));
            OnDialogueEnded?.Invoke();
        }

        /// <summary>
        /// Goes back to previous node.
        /// </summary>
        public bool GoBack()
        {
            if (_nodeHistory.Count == 0) return false;

            string previousNodeId = _nodeHistory.Pop();
            
            if (_currentDialogue != null && _currentDialogue.nodes.TryGetValue(previousNodeId, out DialogueNode node))
            {
                _currentNode = node;
                OnNodeChanged?.Invoke(_currentNode);
                return true;
            }

            return false;
        }

        #endregion

        #region Conditions and Actions

        private bool CheckConditions(List<DialogueCondition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
            {
                return true;
            }

            foreach (var condition in conditions)
            {
                if (!CheckSingleCondition(condition))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckSingleCondition(DialogueCondition condition)
        {
            switch (condition.type)
            {
                case ConditionType.HasItem:
                {
                    var inventory = ComponentLocator.Get<IInventorySystem>();
                    return inventory != null && inventory.HasItem(condition.itemId);
                }

                case ConditionType.HasFlag:
                {
                    // Would need flag manager
                    return true; // Placeholder
                }

                case ConditionType.QuestActive:
                {
                    var quests = ComponentLocator.Get<IQuestManager>();
                    return quests != null && quests.IsQuestActive(condition.questId);
                }

                case ConditionType.QuestComplete:
                {
                    var quests = ComponentLocator.Get<IQuestManager>();
                    return quests != null && quests.IsQuestCompleted(condition.questId);
                }

                case ConditionType.LevelGte:
                {
                    var stats = ComponentLocator.Get<IPlayerStats>();
                    return stats != null && stats.Level >= condition.value;
                }

                case ConditionType.ReputationGte:
                {
                    // Would need reputation manager
                    return true; // Placeholder
                }

                case ConditionType.Random:
                {
                    return UnityEngine.Random.value * 100 <= condition.probability;
                }

                case ConditionType.TimeOfDay:
                {
                    var time = ComponentLocator.Get<ITimeManager>();
                    if (time != null)
                    {
                        int hour = time.CurrentHour;
                        return hour >= condition.value; // Simplified
                    }
                    return true;
                }

                default:
                    return true;
            }
        }

        private void ExecuteActions(List<DialogueAction> actions)
        {
            if (actions == null) return;

            foreach (var action in actions)
            {
                ExecuteSingleAction(action);
            }
        }

        private void ExecuteSingleAction(DialogueAction action)
        {
            switch (action.type)
            {
                case ActionType.StartQuest:
                {
                    var quests = ComponentLocator.Get<IQuestManager>();
                    quests?.StartQuest(action.questId);
                    break;
                }

                case ActionType.CompleteQuestStage:
                {
                    // Would need direct quest manager access
                    break;
                }

                case ActionType.FailQuest:
                {
                    var quests = ComponentLocator.Get<IQuestManager>();
                    quests?.FailQuest(action.questId);
                    break;
                }

                case ActionType.SetFlag:
                {
                    // Would need flag manager
                    Debug.Log($"[DialogueManager] Set flag: {action.flag}");
                    break;
                }

                case ActionType.ClearFlag:
                {
                    Debug.Log($"[DialogueManager] Clear flag: {action.flag}");
                    break;
                }

                case ActionType.GiveItem:
                {
                    var inventory = ComponentLocator.Get<IInventorySystem>();
                    inventory?.AddItem(action.itemId, action.count > 0 ? action.count : 1);
                    Debug.Log($"[DialogueManager] Gave item: {action.itemId}");
                    break;
                }

                case ActionType.RemoveItem:
                {
                    var inventory = ComponentLocator.Get<IInventorySystem>();
                    inventory?.RemoveItem(action.itemId, action.count > 0 ? action.count : 1);
                    Debug.Log($"[DialogueManager] Removed item: {action.itemId}");
                    break;
                }

                case ActionType.ChangeRelation:
                {
                    // Would need NPC relation manager
                    Debug.Log($"[DialogueManager] Changed relation by: {action.value}");
                    break;
                }

                case ActionType.ChangeReputation:
                {
                    Debug.Log($"[DialogueManager] Reputation change: {action.value}");
                    break;
                }

                case ActionType.Teleport:
                {
                    var player = ComponentLocator.Get<IPlayerController>();
                    player?.Teleport(action.position.ToVector3());
                    break;
                }

                case ActionType.UnlockTrainer:
                {
                    Debug.Log($"[DialogueManager] Unlocked trainer: {action.trainerId}");
                    break;
                }

                case ActionType.LearnSpell:
                {
                    Debug.Log($"[DialogueManager] Learn spell: {action.spellId}");
                    break;
                }

                case ActionType.EndDialogue:
                {
                    EndDialogue();
                    break;
                }

                default:
                    Debug.Log($"[DialogueManager] Unhandled action type: {action.type}");
                    break;
            }
        }

        #endregion

        #region Get Available Choices

        /// <summary>
        /// Gets available choices for the current node.
        /// </summary>
        public List<DialogueChoice> GetAvailableChoices()
        {
            if (_currentNode == null) return new List<DialogueChoice>();

            var available = new List<DialogueChoice>();

            foreach (var choice in _currentNode.choices)
            {
                if (CheckConditions(choice.conditions))
                {
                    available.Add(choice);
                }
            }

            return available;
        }

        /// <summary>
        /// Checks if a choice is available (for UI display).
        /// </summary>
        public bool IsChoiceAvailable(int choiceIndex)
        {
            if (_currentNode == null || choiceIndex < 0 || choiceIndex >= _currentNode.choices.Count)
            {
                return false;
            }

            return CheckConditions(_currentNode.choices[choiceIndex].conditions);
        }

        #endregion

        #region Getters

        /// <summary>
        /// Gets a dialogue by ID.
        /// </summary>
        public DialogueTree GetDialogue(string dialogueId)
        {
            return _dialogues.GetValueOrDefault(dialogueId);
        }

        /// <summary>
        /// Checks if a dialogue exists.
        /// </summary>
        public bool HasDialogue(string dialogueId)
        {
            return _dialogues.ContainsKey(dialogueId);
        }

        #endregion
    }

    #region Interface

    public interface IDialogueManager
    {
        bool IsInDialogue { get; }
        DialogueNode CurrentNode { get; }
        
        bool StartDialogue(string dialogueId, string npcId);
        void SelectChoice(int choiceIndex);
        void EndDialogue();
        void GoBack();
        List<DialogueChoice> GetAvailableChoices();
        bool IsChoiceAvailable(int choiceIndex);
    }

    public class DialogueManagerInterface : IDialogueManager
    {
        private readonly DialogueManager _manager;

        public DialogueManagerInterface(DialogueManager manager)
        {
            _manager = manager;
        }

        public bool IsInDialogue => _manager.IsInDialogue;
        public DialogueNode CurrentNode => _manager.CurrentNode;

        public bool StartDialogue(string dialogueId, string npcId) 
            => _manager.StartDialogue(dialogueId, npcId);
        public void SelectChoice(int choiceIndex) => _manager.SelectChoice(choiceIndex);
        public void EndDialogue() => _manager.EndDialogue();
        public void GoBack() => _manager.GoBack();
        public List<DialogueChoice> GetAvailableChoices() => _manager.GetAvailableChoices();
        public bool IsChoiceAvailable(int choiceIndex) => _manager.IsChoiceAvailable(choiceIndex);
    }

    #endregion
}
