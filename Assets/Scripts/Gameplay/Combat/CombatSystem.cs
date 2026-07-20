using ZelaznaDroga.Core.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ZelaznaDroga.Data.Schema;
using ZelaznaDroga.Gameplay.Inventory;
using ZelaznaDroga.Gameplay.Progression;

namespace ZelaznaDroga.Gameplay.Combat
{
    /// <summary>
    /// Manages combat actions including attacks, blocking, and weapon switching.
    /// </summary>
    public class CombatSystem : BaseMonoBehaviour
    {
        #region Dependencies
        [Header("Dependencies")]
        [SerializeField] private PlayerStats _playerStats;
        [SerializeField] private InventorySystem _inventory;
        [SerializeField] private Animator _animator;
        #endregion
        #region Combat Settings
        [Header("Combat Settings")]
        [SerializeField] private float _lightAttackDuration = 0.4f;
        [SerializeField] private float _heavyAttackDuration = 0.7f;
        [SerializeField] private float _blockDuration = 0.3f;
        [SerializeField] private float _attackCooldown = 0.2f;
        [Header("Hit Detection")]
        [SerializeField] private float _meleeRange = 2f;
        [SerializeField] private float _hitRadius = 1.5f;
        [SerializeField] private LayerMask _enemyLayers;
        [Header("VFX")]
        [SerializeField] private GameObject _hitVfxPrefab;
        [SerializeField] private Transform _vfxSpawnPoint;
        #region State
        private bool _isAttacking;
        private bool _isBlocking;
        private bool _canAttack = true;
        private float _attackTimer;
        private float _cooldownTimer;
        private int _comboCount;
        private float _blockTimer;
        private WeaponType _currentWeaponType = WeaponType.None;
        private float _currentAttackSpeed = 1f;
        #region Properties
        public bool IsAttacking => _isAttacking;
        public bool IsBlocking => _isBlocking;
        public WeaponType CurrentWeaponType => _currentWeaponType;
        #region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();
            
            if (_vfxSpawnPoint == null)
                _vfxSpawnPoint = transform;
        }
        private void Start()
            ComponentLocator.Register<ICombatSystem>(new CombatSystemInterface(this));
        private void OnDestroy()
            ComponentLocator.Unregister<ICombatSystem>(new CombatSystemInterface(this));
        private void Update()
            UpdateTimers();
            UpdateCombo();
        #region Input Handling
        public void OnLightAttack(InputAction.CallbackContext context)
            if (context.started && _canAttack && !_isBlocking)
            {
                PerformLightAttack();
            }
        public void OnHeavyAttack(InputAction.CallbackContext context)
                PerformHeavyAttack();
        public void OnBlock(InputAction.CallbackContext context)
            if (context.started)
                StartBlock();
            else if (context.canceled)
                StopBlock();
        public void OnCastSpell(InputAction.CallbackContext context)
                TryCastActiveSpell();
        #region Attack Actions
        /// <summary>
        /// Performs a light attack.
        /// </summary>
        public void PerformLightAttack()
            if (_isAttacking || !_canAttack) return;
            _isAttacking = true;
            _canAttack = false;
            _attackTimer = _lightAttackDuration / _currentAttackSpeed;
            _cooldownTimer = _attackCooldown;
            _animator?.SetTrigger(GameConstants.ANIM_PARAM_ATTACK_LIGHT);
            _animator?.SetInteger("ComboCount", _comboCount);
            Debug.Log($"[Combat] Light attack #{_comboCount + 1}");
        /// Performs a heavy attack.
        public void PerformHeavyAttack()
            _attackTimer = _heavyAttackDuration / _currentAttackSpeed;
            _cooldownTimer = _attackCooldown * 1.5f;
            _animator?.SetTrigger(GameConstants.ANIM_PARAM_ATTACK_HEAVY);
            Debug.Log($"[Combat] Heavy attack");
        /// Called by animation events when attack connects.
        public void OnAttackHit()
            PerformHitDetection();
        /// Performs hit detection for the current attack.
        private void PerformHitDetection()
            Vector3 origin = _vfxSpawnPoint != null ? _vfxSpawnPoint.position : transform.position;
            Vector3 direction = transform.forward;
            // Determine damage based on weapon and stats
            int damage = CalculateDamage(_currentWeaponType);
            bool isHeavy = _attackTimer > _lightAttackDuration / _currentAttackSpeed;
            if (isHeavy)
                damage = Mathf.FloorToInt(damage * GameConstants.HEAVY_ATTACK_DAMAGE_MULT);
            // Perform sphere cast
            Collider[] hits = Physics.OverlapSphere(origin, _hitRadius, _enemyLayers);
            foreach (Collider hit in hits)
                // Check if it's a valid target (not behind obstacles)
                if (Physics.Raycast(origin, (hit.transform.position - origin).normalized, out RaycastHit rayHit, _meleeRange))
                {
                    if (rayHit.transform == hit.transform || rayHit.transform.IsChildOf(hit.transform))
                    {
                        ApplyDamageToTarget(hit.gameObject, damage);
                        SpawnHitVfx(rayHit.point);
                    }
                }
        /// Calculates damage based on weapon and stats.
        private int CalculateDamage(WeaponType weaponType)
            switch (weaponType)
                case WeaponType.Melee:
                    var (min, max) = _inventory.GetEquippedWeaponDamage();
                    return _playerStats.CalculateMeleeDamage(min, max);
                case WeaponType.Ranged:
                    // Ranged handled separately
                    return 0;
                case WeaponType.Magic:
                    // Magic handled separately
                default:
                    // Fists
                    return _playerStats.CalculateMeleeDamage(1, 3);
        /// Applies damage to a target.
        private void ApplyDamageToTarget(GameObject target, int damage)
            // Try to get health component
            var health = target.GetComponent<IDamageable>();
            if (health != null)
                health.TakeDamage(damage, transform.gameObject);
            Debug.Log($"[Combat] Hit {target.name} for {damage} damage");
        /// Spawns hit VFX at the impact point.
        private void SpawnHitVfx(Vector3 position)
            if (_hitVfxPrefab != null)
                Instantiate(_hitVfxPrefab, position, Quaternion.identity);
        #region Blocking
        /// Starts blocking.
        public void StartBlock()
            if (_isAttacking) return;
            _isBlocking = true;
            _blockTimer = _blockDuration;
            _animator?.SetBool(GameConstants.ANIM_PARAM_BLOCK, true);
            Debug.Log("[Combat] Blocking started");
        /// Stops blocking.
        public void StopBlock()
            if (!_isBlocking) return;
            _isBlocking = false;
            _animator?.SetBool(GameConstants.ANIM_PARAM_BLOCK, false);
            Debug.Log("[Combat] Blocking stopped");
        /// Checks if an incoming attack is blocked.
        public bool CheckBlock(Vector3 attackDirection)
            if (!_isBlocking) return false;
            // Check if attack is in front of player
            float angle = Vector3.Angle(transform.forward, attackDirection);
            return angle < 90f; // Within 90 degree cone
        /// Handles blocked damage.
        public int ReduceBlockedDamage(int damage)
            return Mathf.FloorToInt(damage * GameConstants.BLOCK_DAMAGE_REDUCTION);
        #region Spells
        /// Tries to cast the active spell.
        public void TryCastActiveSpell()
            // This would be expanded based on active spell selection
            Debug.Log("[Combat] Attempting to cast spell...");
        /// Casts a spell by ID.
        public bool CastSpell(string spellId, GameObject target = null)
            // Get spell data from spell system
            var spellSystem = ComponentLocator.Get<ISpellSystem>();
            var spell = spellSystem?.GetSpell(spellId);
            if (spell == null)
                Debug.LogWarning($"[Combat] Spell {spellId} not found");
                return false;
            // Check mana
            if (_playerStats.CurrentMana < spell.manaCost)
                Debug.Log("[Combat] Not enough mana");
            // Consume mana
            _playerStats.ModifyMana(-spell.manaCost);
            // Perform spell effect
            // This would spawn VFX, apply damage/healing, etc.
            Debug.Log($"[Combat] Cast {spell.nameKey}");
            return true;
        #region Weapon Management
        /// Switches to a specific weapon slot.
        public void SwitchToWeapon(int slot)
            // This would swap equipped weapons
            Debug.Log($"[Combat] Switching to weapon slot {slot}");
        /// Updates current weapon type based on equipped items.
        public void UpdateWeaponType()
            var weapon = _inventory.EquippedWeapon1?.Data as WeaponData ?? 
                         _inventory.EquippedWeapon2?.Data as WeaponData;
            if (weapon != null)
                switch (weapon.subType)
                    case "Sword":
                    case "Axe":
                    case "Mace":
                    case "Dagger":
                        _currentWeaponType = WeaponType.Melee;
                        break;
                    case "Bow":
                        _currentWeaponType = WeaponType.Ranged;
                    case "Staff":
                        _currentWeaponType = WeaponType.Magic;
                _currentAttackSpeed = weapon.attackSpeed;
            else
                _currentWeaponType = WeaponType.None;
                _currentAttackSpeed = 1f;
            Debug.Log($"[Combat] Weapon type: {_currentWeaponType}");
        #region Timers
        private void UpdateTimers()
            // Attack timer
            if (_isAttacking)
                _attackTimer -= Time.deltaTime;
                if (_attackTimer <= 0)
                    _isAttacking = false;
                    IncrementCombo();
            // Cooldown timer
            if (!_canAttack && !_isAttacking)
                _cooldownTimer -= Time.deltaTime;
                if (_cooldownTimer <= 0)
                    _canAttack = true;
            // Block timer
            if (_isBlocking)
                _blockTimer -= Time.deltaTime;
                if (_blockTimer <= 0)
                    StopBlock();
        private void IncrementCombo()
            _comboCount = (_comboCount + 1) % 3; // 3-hit combo
        private void UpdateCombo()
            if (!_isAttacking && !_canAttack)
                _comboCount = 0;
                _animator?.SetInteger("ComboCount", 0);
        #region Take Damage
        /// Called when player takes damage.
        public void TakeDamage(int damage, GameObject attacker)
            if (_isBlocking && CheckBlock(attacker.transform.position - transform.position))
                damage = ReduceBlockedDamage(damage);
                Debug.Log($"[Combat] Blocked! Damage reduced to {damage}");
            float reduction = _playerStats.CalculateDamageReduction(damage, 0);
            damage = Mathf.FloorToInt(damage * (1 - reduction));
            _playerStats.ModifyHealth(-damage);
            _animator?.SetTrigger(GameConstants.ANIM_PARAM_HIT);
            // Screen flash / feedback
            if (Camera.main)
                Camera.main.backgroundColor = Color.Lerp(Color.red * 0.3f, Color.black, 0.7f);
                Invoke(nameof(ResetCamColor), 0.12f);
            Debug.Log($"[Combat] Took {damage} damage");
        private void ResetCamColor()
            if (Camera.main) Camera.main.backgroundColor = Color.black;
    }
    #region Types
    public enum WeaponType
        None,
        Melee,
        Ranged,
        Magic
    public interface IDamageable
        void TakeDamage(int damage, GameObject attacker);
        void Die(GameObject killer);
    public interface ISpellSystem
        SpellData GetSpell(string spellId);
    #endregion
    #region Interface
    public interface ICombatSystem
        bool IsAttacking { get; }
        bool IsBlocking { get; }
        WeaponType CurrentWeaponType { get; }
        void PerformLightAttack();
        void PerformHeavyAttack();
        void StartBlock();
        void StopBlock();
        bool CastSpell(string spellId, GameObject target = null);
    public class CombatSystemInterface : ICombatSystem
        private readonly CombatSystem _combat;
        public CombatSystemInterface(CombatSystem combat)
            _combat = combat;
        public bool IsAttacking => _combat.IsAttacking;
        public bool IsBlocking => _combat.IsBlocking;
        public WeaponType CurrentWeaponType => _combat.CurrentWeaponType;
        public void PerformLightAttack() => _combat.PerformLightAttack();
        public void PerformHeavyAttack() => _combat.PerformHeavyAttack();
        public void StartBlock() => _combat.StartBlock();
        public void StopBlock() => _combat.StopBlock();
        public bool CastSpell(string spellId, GameObject target = null) => _combat.CastSpell(spellId, target);
        public void TakeDamage(int damage, GameObject attacker) => _combat.TakeDamage(damage, attacker);
}
