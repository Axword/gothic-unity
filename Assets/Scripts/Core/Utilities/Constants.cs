using UnityEngine;

namespace ZelaznaDroga.Core.Utilities
{
    /// <summary>
    /// Game-wide constants.
    /// </summary>
    public static class GameConstants
    {
        // Game settings
        public const string GAME_NAME = "Żelazna Droga";
        public const string GAME_VERSION = "1.0.0";
        public const string SAVE_FOLDER = "Saves";
        public const int MAX_SAVE_SLOTS = 5;

        // Player settings
        public const float BASE_MOVE_SPEED = 5f;
        public const float SPRINT_MULTIPLIER = 1.5f;
        public const float JUMP_FORCE = 8f;
        public const float GRAVITY = 20f;
        public const float MOUSE_SENSITIVITY = 2f;
        public const float CAMERA_DISTANCE = 5f;
        public const float CAMERA_HEIGHT = 2f;

        // Combat settings
        public const float LIGHT_ATTACK_DAMAGE_MULT = 1f;
        public const float HEAVY_ATTACK_DAMAGE_MULT = 1.5f;
        public const float BLOCK_DAMAGE_REDUCTION = 0.5f;
        public const float CRITICAL_HIT_CHANCE = 0.05f;
        public const float CRITICAL_HIT_MULTIPLIER = 2f;

        // Leveling
        public const int BASE_XP_PER_LEVEL = 100;
        public const float XP_SCALING_PER_LEVEL = 1.5f;
        public const int LEARNING_POINTS_PER_LEVEL = 1;
        public const int BASE_HP_PER_LEVEL = 10;

        // Time
        public const int HOURS_PER_DAY = 24;
        public const int DAY_MINUTE_DURATION = 20; // seconds per game hour
        public const float DEFAULT_TIME_SCALE = 1f;
        public const float FAST_TIME_SCALE = 60f; // when sleeping

        // Factions
        public const string FACTION_OLD_ORDER = "OldOrder";
        public const string FACTION_NEW_ORDER = "NewOrder";
        public const string FACTION_BANDITS = "Bandits";
        public const string FACTION_NEUTRAL = "Neutral";

        // Layers
        public const string LAYER_PLAYER = "Player";
        public const string LAYER_NPC = "NPC";
        public const string LAYER_ENEMY = "Enemy";
        public const string LAYER_INTERACTABLE = "Interactable";
        public const string LAYER_GROUND = "Ground";
        public const string LAYER_DEFAULT = "Default";

        // Tags
        public const string TAG_PLAYER = "Player";
        public const string TAG_NPC = "NPC";
        public const string TAG_ENEMY = "Enemy";
        public const string TAG_CHEST = "Chest";
        public const string TAG_DOOR = "Door";
        public const string TAG_ITEM = "Item";
        public const string TAG_DIALOGUE_TRIGGER = "DialogueTrigger";

        // Animation parameter names
        public const string ANIM_PARAM_SPEED = "Speed";
        public const string ANIM_PARAM_IS_GROUNDED = "IsGrounded";
        public const string ANIM_PARAM_IS_SPRINTING = "IsSprinting";
        public const string ANIM_PARAM_ATTACK_LIGHT = "AttackLight";
        public const string ANIM_PARAM_ATTACK_HEAVY = "AttackHeavy";
        public const string ANIM_PARAM_BLOCK = "Block";
        public const string ANIM_PARAM_DODGE = "Dodge";
        public const string ANIM_PARAM_HIT = "Hit";
        public const string ANIM_PARAM_DEATH = "Death";
        public const string ANIM_PARAM_INTERACT = "Interact";

        // Paths
        public const string DATA_PATH = "StreamingAssets/Data/Json";
        public const string SCHEMA_PATH = "StreamingAssets/Data/Schemas";

        // Item IDs
        public const string ITEM_GOLD = "gold";
        public const string ITEM_LOCKPICK = "item_lockpick";

        // Input
        public const string INPUT_HORIZONTAL = "Horizontal";
        public const string INPUT_VERTICAL = "Vertical";
        public const string INPUT_MOUSE_X = "Mouse X";
        public const string INPUT_MOUSE_Y = "Mouse Y";
        public const string INPUT_JUMP = "Jump";
        public const string INPUT_SPRINT = "Sprint";
        public const string INPUT_INTERACT = "Interact";
        public const string INPUT_ATTACK_LIGHT = "Fire1";
        public const string INPUT_ATTACK_HEAVY = "Fire2";
        public const string INPUT_BLOCK = "Block";
        public const string INPUT_DODGE = "Dodge";
        public const string INPUT_CAST_SPELL = "CastSpell";
        public const string INPUT_ABILITY_1 = "Ability1";
        public const string INPUT_ABILITY_2 = "Ability2";
        public const string INPUT_WEAPON_1 = "Weapon1";
        public const string INPUT_WEAPON_2 = "Weapon2";
        public const string INPUT_WEAPON_3 = "Weapon3";
        public const string INPUT_INVENTORY = "Inventory";
        public const string INPUT_JOURNAL = "Journal";
        public const string INPUT_CHARACTER = "Character";
        public const string INPUT_PAUSE = "Pause";
    }
}
