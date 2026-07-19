using System;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Progression
{
    /// <summary>
    /// Manages player statistics, leveling, and base stats.
    /// </summary>
    public class PlayerStats : BaseMonoBehaviour
    {
        #region Events

        public event Action<int, int> OnHealthChanged;
        public event Action<int, int> OnManaChanged;
        public event Action<int> OnLevelUp;
        public event Action<int> OnLearningPointsChanged;
        public event Action OnDeath;
        public event Action OnRespawn;

        #endregion

        #region Base Stats

        [Header("Base Stats")]
        [SerializeField] private int _strength = 10;
        [SerializeField] private int _dexterity = 10;
        [SerializeField] private int _mana = 10;

        #endregion

        #region Derived Stats

        [Header("Health & Mana")]
        [SerializeField] private int _currentHealth = 100;
        [SerializeField] private int _maxHealth = 100;
        [SerializeField] private int _currentMana = 50;
        [SerializeField] private int _maxMana = 50;

        [Header("Armor")]
        [SerializeField] private int _armor = 0;

        #endregion

        #region Leveling

        [Header("Leveling")]
        [SerializeField] private int _level = 1;
        [SerializeField] private int _currentXp = 0;
        [SerializeField] private int _learningPoints = 0;

        #endregion

        #region Properties

        // Base stats
        public int Strength => _strength;
        public int Dexterity => _dexterity;
        public int ManaBase => _mana;

        // Derived stats
        public int MaxHealth => _maxHealth;
        public int CurrentHealth => _currentHealth;
        public int MaxMana => _maxMana;
        public int CurrentMana => _currentMana;
        public int Armor => _armor;

        // Leveling
        public int Level => _level;
        public int CurrentXp => _currentXp;
        public int LearningPoints => _learningPoints;

        public float HealthPercent => (float)_currentHealth / _maxHealth;
        public float ManaPercent => (float)_currentMana / _maxMana;

        public int XpToNextLevel => CalculateXpForLevel(_level + 1);
        public float XpProgress => (float)_currentXp / XpToNextLevel;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();
            RecalculateStats();
        }

        private void Start()
        {
            ComponentLocator.Register<IPlayerStats>(new PlayerStatsInterface(this));
        }

        private void OnDestroy()
        {
            ComponentLocator.Unregister<IPlayerStats>(new PlayerStatsInterface(this));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Modifies health by delta (positive for heal, negative for damage).
        /// </summary>
        public void ModifyHealth(int delta)
        {
            int oldHealth = _currentHealth;
            _currentHealth = Mathf.Clamp(_currentHealth + delta, 0, _maxHealth);

            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            EventBus.Publish(new PlayerHealthChangedEvent(_currentHealth, _maxHealth, delta));

            if (_currentHealth <= 0 && oldHealth > 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Modifies mana by delta.
        /// </summary>
        public void ModifyMana(int delta)
        {
            int oldMana = _currentMana;
            _currentMana = Mathf.Clamp(_currentMana + delta, 0, _maxMana);

            OnManaChanged?.Invoke(_currentMana, _maxMana);
            EventBus.Publish(new PlayerManaChangedEvent(_currentMana, _maxMana, delta));
        }

        /// <summary>
        /// Adds experience points.
        /// </summary>
        public void AddXp(int amount)
        {
            if (amount <= 0) return;

            _currentXp += amount;

            // Check for level up
            while (_currentXp >= XpToNextLevel)
            {
                LevelUp();
            }
        }

        /// <summary>
        /// Adds learning points.
        /// </summary>
        public void AddLearningPoints(int amount)
        {
            if (amount <= 0) return;

            _learningPoints += amount;
            OnLearningPointsChanged?.Invoke(_learningPoints);
        }

        /// <summary>
        /// Uses learning points to increase a base stat.
        /// </summary>
        public bool SpendLearningPoints(string stat, int amount = 1)
        {
            if (_learningPoints < amount) return false;

            switch (stat.ToLower())
            {
                case "strength":
                case "str":
                    _strength += amount;
                    break;
                case "dexterity":
                case "dex":
                    _dexterity += amount;
                    break;
                case "mana":
                    _mana += amount;
                    _maxMana = CalculateMaxMana();
                    break;
                default:
                    return false;
            }

            _learningPoints -= amount;
            RecalculateStats();
            OnLearningPointsChanged?.Invoke(_learningPoints);
            return true;
        }

        /// <summary>
        /// Heals to full health.
        /// </summary>
        public void FullHeal()
        {
            ModifyHealth(_maxHealth - _currentHealth);
        }

        /// <summary>
        /// Restores mana to full.
        /// </summary>
        public void FullMana()
        {
            ModifyMana(_maxMana - _currentMana);
        }

        /// <summary>
        /// Respawns the player after death.
        /// </summary>
        public void Respawn()
        {
            _currentHealth = _maxHealth;
            _currentMana = _maxMana;
            OnRespawn?.Invoke();
            EventBus.Publish(new PlayerHealthChangedEvent(_currentHealth, _maxHealth, _maxHealth));
            EventBus.Publish(new PlayerManaChangedEvent(_currentMana, _maxMana, _maxMana));
        }

        /// <summary>
        /// Recalculates all derived stats based on base stats and level.
        /// </summary>
        public void RecalculateStats()
        {
            _maxHealth = CalculateMaxHealth();
            _maxMana = CalculateMaxMana();
        }

        /// <summary>
        /// Adds armor value.
        /// </summary>
        public void AddArmor(int amount)
        {
            _armor += amount;
        }

        /// <summary>
        /// Removes armor value.
        /// </summary>
        public void RemoveArmor(int amount)
        {
            _armor = Mathf.Max(0, _armor - amount);
        }

        #endregion

        #region Private Methods

        private void LevelUp()
        {
            _currentXp -= XpToNextLevel;
            _level++;

            // Calculate bonus HP
            int bonusHp = GameConstants.BASE_HP_PER_LEVEL + (_strength / 2);
            _maxHealth += bonusHp;
            _currentHealth = _maxHealth; // Full heal on level up

            // Add learning points
            int pointsGained = GameConstants.LEARNING_POINTS_PER_LEVEL;
            _learningPoints += pointsGained;

            Debug.Log($"[PlayerStats] Level up! Now level {_level}. +{bonusHp} HP, +{pointsGained} LP");

            OnLevelUp?.Invoke(_level);
            OnLearningPointsChanged?.Invoke(_learningPoints);
            EventBus.Publish(new PlayerLevelUpEvent(_level, pointsGained));
        }

        private int CalculateMaxHealth()
        {
            return 100 + (_level - 1) * GameConstants.BASE_HP_PER_LEVEL + (_strength - 10) * 2;
        }

        private int CalculateMaxMana()
        {
            return 50 + (_level - 1) * 5 + (_mana - 10) * 3;
        }

        private void Die()
        {
            Debug.Log("[PlayerStats] Player died!");
            OnDeath?.Invoke();
            EventBus.Publish(new PlayerDiedEvent());
        }

        /// <summary>
        /// Calculates XP required for a specific level.
        /// </summary>
        public static int CalculateXpForLevel(int level)
        {
            return Mathf.FloorToInt(GameConstants.BASE_XP_PER_LEVEL * Mathf.Pow(GameConstants.XP_SCALING_PER_LEVEL, level - 1));
        }

        /// <summary>
        /// Calculates damage dealt based on weapon and stats.
        /// </summary>
        public int CalculateMeleeDamage(int weaponMinDamage, int weaponMaxDamage)
        {
            int baseDamage = UnityEngine.Random.Range(weaponMinDamage, weaponMaxDamage + 1);
            int strengthBonus = (_strength - 10) / 2;
            
            // Critical hit chance based on dexterity
            bool isCritical = UnityEngine.Random.value < (GameConstants.CRITICAL_HIT_CHANCE + _dexterity * 0.001f);
            float critMultiplier = isCritical ? GameConstants.CRITICAL_HIT_MULTIPLIER : 1f;

            int totalDamage = Mathf.FloorToInt((baseDamage + strengthBonus) * critMultiplier);
            return Mathf.Max(1, totalDamage);
        }

        /// <summary>
        /// Calculates ranged damage based on weapon and stats.
        /// </summary>
        public int CalculateRangedDamage(int weaponMinDamage, int weaponMaxDamage)
        {
            int baseDamage = UnityEngine.Random.Range(weaponMinDamage, weaponMaxDamage + 1);
            int dexterityBonus = (_dexterity - 10) / 2;

            bool isCritical = UnityEngine.Random.value < (GameConstants.CRITICAL_HIT_CHANCE + _dexterity * 0.002f);
            float critMultiplier = isCritical ? GameConstants.CRITICAL_HIT_MULTIPLIER : 1f;

            int totalDamage = Mathf.FloorToInt((baseDamage + dexterityBonus) * critMultiplier);
            return Mathf.Max(1, totalDamage);
        }

        /// <summary>
        /// Calculates magic damage based on spell and stats.
        /// </summary>
        public int CalculateMagicDamage(int baseSpellDamage)
        {
            int manaBonus = (_mana - 10) / 3;
            return Mathf.Max(1, baseSpellDamage + manaBonus);
        }

        /// <summary>
        /// Calculates damage reduction from armor.
        /// </summary>
        public float CalculateDamageReduction(int incomingDamage, int targetArmor)
        {
            int totalArmor = targetArmor + _armor;
            float reduction = totalArmor / (50f + totalArmor); // Diminishing returns formula
            return reduction;
        }

        #endregion

        #region Save/Load

        /// <summary>
        /// Gets current stats for saving.
        /// </summary>
        public PlayerSaveData GetSaveData()
        {
            return new PlayerSaveData
            {
                id = "player",
                level = _level,
                xp = _currentXp,
                learningPoints = _learningPoints,
                strength = _strength,
                dexterity = _dexterity,
                mana = _mana,
                maxHealth = _maxHealth,
                currentHealth = _currentHealth,
                maxMana = _maxMana,
                currentMana = _currentMana,
                armor = _armor
            };
        }

        /// <summary>
        /// Loads stats from save data.
        /// </summary>
        public void LoadSaveData(PlayerSaveData data)
        {
            _level = data.level;
            _currentXp = data.xp;
            _learningPoints = data.learningPoints;
            _strength = data.strength;
            _dexterity = data.dexterity;
            _mana = data.mana;
            _maxHealth = data.maxHealth;
            _currentHealth = data.currentHealth;
            _maxMana = data.maxMana;
            _currentMana = data.currentMana;
            _armor = data.armor;
        }

        #endregion
    }

    #region Interface

    public interface IPlayerStats
    {
        int MaxHealth { get; }
        int CurrentHealth { get; }
        int MaxMana { get; }
        int CurrentMana { get; }
        int Level { get; }
        int LearningPoints { get; }
        float HealthPercent { get; }
        float ManaPercent { get; }

        void ModifyHealth(int delta);
        void ModifyMana(int delta);
        void AddXp(int amount);
    }

    public class PlayerStatsInterface : IPlayerStats
    {
        private readonly PlayerStats _stats;

        public PlayerStatsInterface(PlayerStats stats)
        {
            _stats = stats;
        }

        public int MaxHealth => _stats.MaxHealth;
        public int CurrentHealth => _stats.CurrentHealth;
        public int MaxMana => _stats.MaxMana;
        public int CurrentMana => _stats.CurrentMana;
        public int Level => _stats.Level;
        public int LearningPoints => _stats.LearningPoints;
        public float HealthPercent => _stats.HealthPercent;
        public float ManaPercent => _stats.ManaPercent;

        public void ModifyHealth(int delta) => _stats.ModifyHealth(delta);
        public void ModifyMana(int delta) => _stats.ModifyMana(delta);
        public void AddXp(int amount) => _stats.AddXp(amount);
    }

    #endregion
}
