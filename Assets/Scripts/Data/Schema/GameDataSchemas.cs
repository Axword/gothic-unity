using System;
using System.Collections.Generic;
using UnityEngine;

namespace ZelaznaDroga.Data.Schema
{
    #region Base Types

    /// <summary>
    /// Base interface for all game data.
    /// </summary>
    public interface IGameData
    {
        string Id { get; set; }
    }

    /// <summary>
    /// Base class for all runtime game data.
    /// </summary>
    [Serializable]
    public abstract class BaseData : IGameData
    {
        public string Id { get; set; }
    }

    #endregion

    #region Item Types

    public enum ItemType
    {
        Weapon,
        Armor,
        Consumable,
        Material,
        QuestItem,
        Key,
        Ammo,
        Treasure
    }

    public enum WeaponSubType
    {
        Sword,
        Bow,
        Staff,
        Dagger,
        Axe,
        Mace
    }

    public enum ArmorSlot
    {
        Head,
        Body,
        Hands,
        Legs,
        Feet,
        Shield
    }

    public enum DamageType
    {
        Slashing,
        Piercing,
        Blunt,
        Magic,
        Fire,
        Ice,
        Lightning
    }

    [Serializable]
    public class ItemRequirement
    {
        public int level = 0;
        public int strength = 0;
        public int dexterity = 0;
        public int mana = 0;
    }

    [Serializable]
    public class DamageRange
    {
        public int min = 0;
        public int max = 0;
        public DamageType type = DamageType.Slashing;
    }

    [Serializable]
    public class ArmorValue
    {
        public int physical = 0;
        public int magical = 0;
        public int fire = 0;
        public int ice = 0;
        public int lightning = 0;
    }

    [Serializable]
    public class ItemEffect
    {
        public string type; // Heal, RestoreMana, Buff, Debuff
        public int value = 0;
        public float duration = 0f;
        public float cooldown = 0f;
    }

    [Serializable]
    public class Durability
    {
        public int max = 100;
        public int current = 100;
    }

    [Serializable]
    public class ItemData : BaseData
    {
        public string nameKey;
        public string descriptionKey;
        public ItemType itemType;
        public string subType;
        public int baseValue;
        public float weight;
        public string iconPath;
        public string modelPath;
        public bool stackable;
        public int maxStack = 1;
        public bool isQuestItem;
        public bool discardable = true;
        public bool sellable = true;
        public float priceModifier = 1f;
    }

    [Serializable]
    public class WeaponData : ItemData
    {
        public DamageRange damage;
        public ItemRequirement requirements;
        public float attackSpeed = 1f;
        public float reach = 1.5f;
        public int[] comboLevels = { 1 };
        public Durability durability = new Durability();
        public float drawTime = 0f; // For bows
        public float range = 30f; // For ranged
        public string ammoType; // For ranged
    }

    [Serializable]
    public class ArmorData : ItemData
    {
        public ArmorSlot slot;
        public ArmorValue armor;
        public ItemRequirement requirements;
        public Durability durability = new Durability();
    }

    [Serializable]
    public class ConsumableData : ItemData
    {
        public ItemEffect effect;
        public bool harvestable;
        public string biome;
    }

    #endregion

    #region NPC Types

    public enum NpcRole
    {
        Leader,
        Guard,
        Merchant,
        Trainer,
        Commoner,
        Traveler,
        Bandit,
        Enemy
    }

    public enum NpcReaction
    {
        Ignore,
        Warning,
        AlertAndFight,
        FleeAndAlert,
        FleeAndHide
    }

    [Serializable]
    public class NpcStats
    {
        public int health = 100;
        public int mana = 50;
        public int strength = 10;
        public int dexterity = 10;
        public int armor = 5;
        public DamageRange damage = new DamageRange();
        public float attackSpeed = 1f;
        public float moveSpeed = 3f;
    }

    [Serializable]
    public class NpcEquipment
    {
        public string slot;
        public string itemId;
    }

    [Serializable]
    public class NpcInventoryItem
    {
        public string itemId;
        public int count = 1;
    }

    [Serializable]
    public class NpcReactions
    {
        public NpcReaction theft = NpcReaction.AlertAndFight;
        public NpcReaction trespass = NpcReaction.Warning;
        public NpcReaction attack = NpcReaction.AlertAndFight;
    }

    [Serializable]
    public class NpcVisual
    {
        public string modelPath;
        public string skinTone = "#D4A574";
        public string hairColor = "#4A3728";
        public string clothesColor = "#3A3A4A";
    }

    [Serializable]
    public class NpcVoice
    {
        public float pitch = 1f;
        public float volume = 1f;
    }

    [Serializable]
    public class NpcPosition
    {
        public string scene;
        public Vector3Wrapper position = new Vector3Wrapper();
        public Vector3Wrapper rotation = new Vector3Wrapper();
    }

    [Serializable]
    public class NpcData : BaseData
    {
        public string nameKey;
        public string titleKey;
        public NpcRole role;
        public string faction;
        public int level = 1;
        public bool isEssential;
        public string defaultGreeting;
        public NpcStats stats = new NpcStats();
        public List<NpcEquipment> equipment = new List<NpcEquipment>();
        public List<NpcInventoryItem> inventory = new List<NpcInventoryItem>();
        public int initialRelation;
        public string scheduleId;
        public List<string> dialogues = new List<string>();
        public List<string> quests = new List<string>();
        public string trainer;
        public NpcReactions reactions = new NpcReactions();
        public NpcVisual visual = new NpcVisual();
        public NpcVoice voice = new NpcVoice();
        public NpcPosition location = new NpcPosition();
    }

    [Serializable]
    public class NpcSchedulePhase
    {
        public TimeRange timeRange = new TimeRange();
        public string locationId;
        public string activity; // Sleep, Work, Patrol, Eat, Social, Idle
        public string animation;
        public List<string> conditions = new List<string>();
        public FallbackPosition fallback = new FallbackPosition();
    }

    [Serializable]
    public class TimeRange
    {
        public int start; // Hour (0-23)
        public int end; // Hour (0-23)
    }

    [Serializable]
    public class FallbackPosition
    {
        public Vector3Wrapper position = new Vector3Wrapper();
    }

    [Serializable]
    public class ScheduleOverride
    {
        public ScheduleCondition condition;
        public List<NpcSchedulePhase> phases = new List<NpcSchedulePhase>();
    }

    [Serializable]
    public class ScheduleCondition
    {
        public string type; // QuestActive, FlagSet, TimeOfDay, etc.
        public string questId;
        public string flag;
    }

    [Serializable]
    public class NpcSchedule
    {
        public string npcId;
        public List<NpcSchedulePhase> phases = new List<NpcSchedulePhase>();
        public List<ScheduleOverride> overrides = new List<ScheduleOverride>();
    }

    #endregion

    #region Monster Types

    public enum MonsterType
    {
        Animal,
        Monster,
        Undead,
        Demon,
        Human
    }

    public enum MonsterBehavior
    {
        Passive,
        Defensive,
        Territorial,
        Pack,
        Aggressive
    }

    [Serializable]
    public class MonsterLootTable
    {
        public string itemId;
        public int minCount = 1;
        public int maxCount = 1;
        public float probability = 1f;
    }

    [Serializable]
    public class SkinReward
    {
        public string itemId;
        public int minCount = 1;
        public int maxCount = 1;
        public string requiresSkill;
        public int skillLevel = 1;
    }

    [Serializable]
    public class DetectionValues
    {
        public float sight = 15f;
        public float hearing = 20f;
        public float alertRange = 5f;
        public float chaseRange = 30f;
    }

    [Serializable]
    public class MonsterRewards
    {
        public int xp = 50;
        public int gold = 5;
    }

    [Serializable]
    public class MonsterVisual
    {
        public string modelPath;
        public float scale = 1f;
        public string iconPath;
    }

    [Serializable]
    public class MonsterData : BaseData
    {
        public string nameKey;
        public string descriptionKey;
        public MonsterType type;
        public List<string> biome = new List<string>();
        public MonsterBehavior behavior;
        public NpcStats stats = new NpcStats();
        public List<MonsterLootTable> lootTable = new List<MonsterLootTable>();
        public string trophyItem;
        public int trophyCount = 1;
        public SkinReward skinReward;
        public DetectionValues detection = new DetectionValues();
        public MonsterRewards rewards = new MonsterRewards();
        public bool isHostile = true;
        public bool isNocturnal;
        public bool isBoss;
        public float respawnTime = 3600f;
        public MonsterVisual visual = new MonsterVisual();
    }

    #endregion

    #region Quest Types

    public enum QuestType
    {
        Main,
        FactionOld,
        FactionNew,
        Side,
        Hidden
    }

    public enum ObjectiveType
    {
        Talk,
        Kill,
        Collect,
        Deliver,
        Explore,
        Escort,
        Persuade,
        Steal,
        Interact,
        Craft
    }

    [Serializable]
    public class QuestRequirement
    {
        public int level;
        public List<string> quests = new List<string>();
        public List<string> flags = new List<string>();
        public List<string> items = new List<string>();
        public Dictionary<string, int> skills = new Dictionary<string, int>();
    }

    [Serializable]
    public class QuestObjective
    {
        public ObjectiveType type;
        public string target;
        public int count = 1;
        public string itemId;
        public string description;
        public bool optional;
        public bool failOnDeath = true;
    }

    [Serializable]
    public class StageActions
    {
        public List<string> addFlags = new List<string>();
        public List<string> removeFlags = new List<string>();
        public List<string> giveItems = new List<string>();
        public List<string> removeItems = new List<string>();
        public string startQuest;
        public bool failQuest;
        public Dictionary<string, int> changeReputation = new Dictionary<string, int>();
        public Dictionary<string, int> changeRelation = new Dictionary<string, int>();
        public List<string> unlockDialogue = new List<string>();
        public string unlockTrainer;
        public string playCutscene;
        public TeleportInfo teleport;
    }

    [Serializable]
    public class TeleportInfo
    {
        public string scene;
        public Vector3Wrapper position = new Vector3Wrapper();
    }

    [Serializable]
    public class QuestStage
    {
        public int id;
        public string descriptionKey;
        public List<QuestObjective> objectives = new List<QuestObjective>();
        public StageActions onComplete = new StageActions();
        public StageActions onFail = new StageActions();
    }

    [Serializable]
    public class QuestRewards
    {
        public int xp;
        public int gold;
        public List<string> items = new List<string>();
        public List<string> flags = new List<string>();
        public int skillPoints;
        public Dictionary<string, int> reputation = new Dictionary<string, int>();
    }

    [Serializable]
    public class FailCondition
    {
        public string type;
        public string target;
    }

    [Serializable]
    public class QuestData : BaseData
    {
        public string titleKey;
        public string descriptionKey;
        public QuestType type;
        public string faction;
        public QuestRequirement requirements = new QuestRequirement();
        public List<QuestStage> stages = new List<QuestStage>();
        public QuestRewards rewards = new QuestRewards();
        public List<FailCondition> failConditions = new List<FailCondition>();
        public string parentQuest;
        public string nextQuest;
        public bool cancelOnFail = true;
    }

    #endregion

    #region Dialogue Types

    public enum ConditionType
    {
        HasItem,
        HasFlag,
        QuestActive,
        QuestComplete,
        QuestFailed,
        LevelGte,
        ReputationGte,
        SkillGte,
        StatGte,
        TimeOfDay,
        Random,
        FactionMatches,
        IsMale,
        IsFemale
    }

    public enum ActionType
    {
        StartQuest,
        CompleteQuestStage,
        FailQuest,
        SetFlag,
        ClearFlag,
        GiveItem,
        RemoveItem,
        ChangeRelation,
        ChangeReputation,
        Teleport,
        PlayAnimation,
        AddShop,
        UnlockTrainer,
        LearnSpell,
        OpenShop,
        EndDialogue,
        PlaySound,
        ChangeWorldState
    }

    [Serializable]
    public class DialogueCondition
    {
        public ConditionType type;
        public string itemId;
        public string flag;
        public string questId;
        public int value;
        public string stat;
        public string faction;
        public float probability = 100f;
    }

    [Serializable]
    public class DialogueAction
    {
        public ActionType type;
        public string questId;
        public int stageId;
        public string flag;
        public string itemId;
        public int count;
        public int value;
        public string scene;
        public Vector3Wrapper position = new Vector3Wrapper();
        public string animation;
        public string sound;
        public string trainerId;
        public string spellId;
    }

    [Serializable]
    public class DialogueChoice
    {
        public string text;
        public List<DialogueCondition> conditions = new List<DialogueCondition>();
        public List<DialogueAction> actions = new List<DialogueAction>();
        public string nextNode;
        public string failNode;
        public string requiredSkill;
        public int skillCheckDC;
        public string requiredStat;
        public int statCheckDC;
    }

    [Serializable]
    public class DialogueNode
    {
        public string nodeId;
        public string speaker;
        public string text;
        public string portrait;
        public string animation;
        public List<DialogueChoice> choices = new List<DialogueChoice>();
        public List<DialogueAction> onEnter = new List<DialogueAction>();
        public List<DialogueAction> onExit = new List<DialogueAction>();
    }

    [Serializable]
    public class DialogueTree : BaseData
    {
        public string rootNodeId;
        public string speaker;
        public Dictionary<string, DialogueNode> nodes = new Dictionary<string, DialogueNode>();
    }

    #endregion

    #region Spell Types

    public enum SpellType
    {
        Damage,
        Heal,
        Buff,
        Debuff,
        Utility
    }

    [Serializable]
    public class SpellData : BaseData
    {
        public string nameKey;
        public string descriptionKey;
        public SpellType spellType;
        public int manaCost;
        public float castTime;
        public float cooldown;
        public float range;
        public DamageRange damage;
        public int healAmount;
        public string vfxPath;
        public string sfxPath;
        public string learnQuestId;
        public string learnTrainerId;
        public List<string> learnFlags = new List<string>();
    }

    #endregion

    #region Trainer Types

    [Serializable]
    public class TrainerSkill
    {
        public string skillId;
        public int maxLevel;
        public int costPerLevel;
        public List<string> requiredFlags = new List<string>();
        public List<string> requiredQuests = new List<string>();
        public int requiredLevel;
    }

    [Serializable]
    public class TrainerData : BaseData
    {
        public string nameKey;
        public string npcId;
        public string faction;
        public List<TrainerSkill> skills = new List<TrainerSkill>();
        public List<string> spells = new List<string>();
        public Dictionary<string, int> priceModifier = new Dictionary<string, int>();
    }

    #endregion

    #region World Types

    [Serializable]
    public class LocationData : BaseData
    {
        public string nameKey;
        public string descriptionKey;
        public string scene;
        public Vector3Wrapper position = new Vector3Wrapper();
        public Vector3Wrapper bounds = new Vector3Wrapper();
        public List<string> connectedLocations = new List<string>();
        public string biome;
        public string ambientMusic;
        public bool isDiscovered;
    }

    [Serializable]
    public class LootEntry
    {
        public string itemId;
        public int minCount = 1;
        public int maxCount = 1;
        public float probability = 1f;
    }

    [Serializable]
    public class LootTable
    {
        public string id;
        public List<LootEntry> entries = new List<LootEntry>();
        public bool isOneTime; // True for quest-related chests
    }

    #endregion

    #region Utility Types

    [Serializable]
    public class Vector3Wrapper
    {
        public float x;
        public float y;
        public float z;

        public Vector3Wrapper() { }

        public Vector3Wrapper(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Wrapper(Vector3 v)
        {
            this.x = v.x;
            this.y = v.y;
            this.z = v.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public static implicit operator Vector3(Vector3Wrapper v) => v.ToVector3();
        public static implicit operator Vector3Wrapper(Vector3 v) => new Vector3Wrapper(v);
    }

    #endregion

    #region Save Game Types

    [Serializable]
    public class SaveGame
    {
        public string schemaVersion = "1.0.0";
        public string saveVersion = "1.0.0";
        public string timestamp;
        public int playtimeSeconds;

        public PlayerSaveData player = new PlayerSaveData();
        public EquipmentSaveData equipment = new EquipmentSaveData();
        public List<InventoryItemSave> inventory = new List<InventoryItemSave>();
        public Dictionary<string, int> skills = new Dictionary<string, int>();

        public ActiveQuestSaveData activeQuests = new ActiveQuestSaveData();
        public List<string> completedQuests = new List<string>();
        public List<string> failedQuests = new List<string>();

        public Dictionary<string, bool> dialogueFlags = new Dictionary<string, bool>();
        public Dictionary<string, int> reputation = new Dictionary<string, int>();

        public List<NpcStateSave> npcStates = new List<NpcStateSave>();
        public WorldStateSave worldState = new WorldStateSave();

        public FactionSaveData factions = new FactionSaveData();
        public StatisticsSave statistics = new StatisticsSave();
    }

    [Serializable]
    public class PlayerSaveData
    {
        public string id = "player";
        public string name = "Nieznajomy";
        public int level = 1;
        public int xp = 0;
        public int learningPoints = 0;

        public int strength = 10;
        public int dexterity = 10;
        public int mana = 10;
        public int maxHealth = 100;
        public int currentHealth = 100;
        public int maxMana = 50;
        public int currentMana = 50;
        public int armor = 0;

        public Vector3Wrapper position = new Vector3Wrapper();
        public Vector3Wrapper rotation = new Vector3Wrapper();
        public string currentScene;
    }

    [Serializable]
    public class EquipmentSaveData
    {
        public string weapon1;
        public string weapon2;
        public string armor;
        public string helmet;
        public string boots;
        public string gloves;
        public string amulet;
    }

    [Serializable]
    public class InventoryItemSave
    {
        public string id;
        public int count = 1;
    }

    [Serializable]
    public class ActiveQuestSaveData
    {
        public List<ActiveQuestInfo> quests = new List<ActiveQuestInfo>();
    }

    [Serializable]
    public class ActiveQuestInfo
    {
        public string id;
        public int stageIndex;
        public Dictionary<string, int> stageObjectives = new Dictionary<string, int>();
        public string startTime;
    }

    [Serializable]
    public class NpcStateSave
    {
        public string id;
        public string currentState;
        public int relation;
        public bool isAlive;
        public string killedBy;
        public List<InventoryItemSave> inventory = new List<InventoryItemSave>();
    }

    [Serializable]
    public class WorldStateSave
    {
        public List<string> chestsOpened = new List<string>();
        public List<string> itemsTaken = new List<string>();
        public List<string> doorsOpened = new List<string>();
        public List<string> objectsDestroyed = new List<string>();
        public string weatherOverride;
    }

    [Serializable]
    public class FactionSaveData
    {
        public string chosenFaction;
        public int oldOrderQuests;
        public int newOrderQuests;
        public bool betrayedOldOrder;
        public bool betrayedNewOrder;
    }

    [Serializable]
    public class StatisticsSave
    {
        public int enemiesKilled;
        public int questsCompleted;
        public int itemsCrafted;
        public int chestsOpened;
        public int locksPicked;
        public int itemsStolen;
        public int deaths;
        public float distanceTraveled;
    }

    #endregion
}
