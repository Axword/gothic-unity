using ZelaznaDroga.Core.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.Gameplay.Inventory
{
    /// <summary>
    /// Manages player inventory, equipment, and item interactions.
    /// </summary>
    public class InventorySystem : BaseMonoBehaviour
    {
        #region Constants
        private const int DEFAULT_CAPACITY = 20;
        private const int GOLD_STACK_LIMIT = 99999;
        #endregion
        #region Events
        public event Action<ItemInstance> OnItemAdded;
        public event Action<ItemInstance> OnItemRemoved;
        public event Action<ItemInstance> OnItemEquipped;
        public event Action<ItemInstance> OnItemUnequipped;
        public event Action<int> OnGoldChanged;
        public event Action OnInventoryFull;
        public event Action OnInventoryChanged;
        #region Inventory
        [Header("Inventory Settings")]
        [SerializeField] private int _capacity = DEFAULT_CAPACITY;
        [SerializeField] private int _gold = 0;
        private List<ItemInstance> _items = new List<ItemInstance>();
        private Dictionary<string, ItemInstance> _itemsById = new Dictionary<string, ItemInstance>();
        #region Equipment
        private ItemInstance _equippedWeapon1;
        private ItemInstance _equippedWeapon2;
        private ItemInstance _equippedArmor;
        private ItemInstance _equippedHelmet;
        private ItemInstance _equippedBoots;
        private ItemInstance _equippedGloves;
        private ItemInstance _equippedAmulet;
        #region Properties
        public int Gold => _gold;
        public int Capacity => _capacity;
        public int UsedSlots => _items.Count;
        public int FreeSlots => _capacity - _items.Count;
        public bool IsFull => _items.Count >= _capacity;
        public IReadOnlyList<ItemInstance> Items => _items;
        public ItemInstance EquippedWeapon1 => _equippedWeapon1;
        public ItemInstance EquippedWeapon2 => _equippedWeapon2;
        public ItemInstance EquippedArmor => _equippedArmor;
        public ItemInstance EquippedHelmet => _equippedHelmet;
        public ItemInstance EquippedBoots => _equippedBoots;
        public ItemInstance EquippedGloves => _equippedGloves;
        public ItemInstance EquippedAmulet => _equippedAmulet;
        #region Unity Lifecycle
        private void Start()
        {
            ComponentLocator.Register<IInventorySystem>(new InventorySystemInterface(this));
        }
        private void OnDestroy()
            ComponentLocator.Unregister<IInventorySystem>(new InventorySystemInterface(this));
        #region Item Management
        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        public bool AddItem(string itemId, int count = 1)
            return AddItem(new ItemInstance { ItemId = itemId, Count = count });
        /// Adds an item instance to the inventory.
        public bool AddItem(ItemInstance item)
            if (item == null || string.IsNullOrEmpty(item.ItemId)) return false;
            // Special handling for gold
            if (item.ItemId == GameConstants.ITEM_GOLD)
            {
                _gold = Mathf.Min(_gold + item.Count, GOLD_STACK_LIMIT);
                OnGoldChanged?.Invoke(_gold);
                OnInventoryChanged?.Invoke();
                return true;
            }
            // Check for existing stackable item
            ItemInstance existing = FindStackableItem(item.ItemId);
            if (existing != null && existing.Data != null && existing.Data.stackable)
                existing.Count = Mathf.Min(existing.Count + item.Count, existing.Data.maxStack);
                OnItemAdded?.Invoke(existing);
            // Check capacity
            if (IsFull)
                OnInventoryFull?.Invoke();
                Debug.Log($"[Inventory] Cannot add item {item.ItemId}: Inventory full");
                return false;
            // Add new item
            ItemInstance newItem = item.Clone();
            _items.Add(newItem);
            _itemsById[newItem.InstanceId] = newItem;
            
            OnItemAdded?.Invoke(newItem);
            EventBus.Publish(new ItemPickupEvent(item.ItemId, item.Count));
            OnInventoryChanged?.Invoke();
            return true;
        /// Removes an item from the inventory.
        public bool RemoveItem(string itemId, int count = 1)
            return RemoveItem(FindItem(itemId), count);
        /// Removes an item instance from the inventory.
        public bool RemoveItem(ItemInstance item, int count = 1)
            if (item == null) return false;
                _gold = Mathf.Max(0, _gold - count);
                item.Count -= count;
            if (item.Count <= count)
                _items.Remove(item);
                _itemsById.Remove(item.InstanceId);
                item.Count = 0;
                OnItemRemoved?.Invoke(item);
            else
        /// Checks if player has an item.
        public bool HasItem(string itemId, int count = 1)
            if (itemId == GameConstants.ITEM_GOLD)
                return _gold >= count;
            int total = 0;
            foreach (var item in _items)
                if (item.ItemId == itemId)
                {
                    total += item.Count;
                }
            return total >= count;
        /// Gets total count of an item.
        public int GetItemCount(string itemId)
                return _gold;
            return total;
        /// Finds first item with given ID.
        public ItemInstance FindItem(string itemId)
                    return item;
            return null;
        /// Finds stackable item.
        private ItemInstance FindStackableItem(string itemId)
                if (item.ItemId == itemId && item.Data != null && item.Data.stackable && item.Count < item.Data.maxStack)
        /// Clears all items from inventory.
        public void Clear()
            _items.Clear();
            _itemsById.Clear();
            _gold = 0;
        /// Equips an item.
        public bool EquipItem(ItemInstance item)
            if (item == null || item.Data == null) return false;
            bool success = false;
            ItemInstance previousItem = null;
            switch (item.Data.itemType)
                case ItemType.Weapon:
                    if (_equippedWeapon1 == null || _equippedWeapon1.Data == null)
                    {
                        previousItem = _equippedWeapon1;
                        _equippedWeapon1 = item;
                        success = true;
                    }
                    else
                        previousItem = _equippedWeapon2;
                        _equippedWeapon2 = item;
                    break;
                case ItemType.Armor:
                    var armorData = item.Data as ArmorData;
                    if (armorData != null)
                        switch (armorData.slot)
                        {
                            case ArmorSlot.Body:
                                previousItem = _equippedArmor;
                                _equippedArmor = item;
                                break;
                            case ArmorSlot.Head:
                                previousItem = _equippedHelmet;
                                _equippedHelmet = item;
                            case ArmorSlot.Feet:
                                previousItem = _equippedBoots;
                                _equippedBoots = item;
                            case ArmorSlot.Hands:
                                previousItem = _equippedGloves;
                                _equippedGloves = item;
                        }
            if (success)
                RemoveItem(item);
                OnItemEquipped?.Invoke(item);
                if (previousItem != null)
                    AddItem(previousItem);
            return success;
        /// Unequips an item to inventory.
        public bool UnequipItem(ItemType type, ArmorSlot? armorSlot = null)
            ItemInstance item = null;
            if (type == ItemType.Weapon)
                if (_equippedWeapon1 != null)
                    item = _equippedWeapon1;
                    _equippedWeapon1 = _equippedWeapon2;
                    _equippedWeapon2 = null;
                else if (_equippedWeapon2 != null)
                    item = _equippedWeapon2;
            else if (type == ItemType.Armor && armorSlot.HasValue)
                switch (armorSlot.Value)
                    case ArmorSlot.Body:
                        item = _equippedArmor;
                        _equippedArmor = null;
                        break;
                    case ArmorSlot.Head:
                        item = _equippedHelmet;
                        _equippedHelmet = null;
                    case ArmorSlot.Feet:
                        item = _equippedBoots;
                        _equippedBoots = null;
                    case ArmorSlot.Hands:
                        item = _equippedGloves;
                        _equippedGloves = null;
            if (item != null)
                AddItem(item);
                OnItemUnequipped?.Invoke(item);
            return false;
        /// Gets equipped weapon damage range.
        public (int min, int max) GetEquippedWeaponDamage()
            var weapon = _equippedWeapon1?.Data as WeaponData ?? _equippedWeapon2?.Data as WeaponData;
            if (weapon != null)
                return (weapon.damage.min, weapon.damage.max);
            return (1, 2); // Fists
        /// Gets equipped armor value.
        public int GetTotalArmor()
            if (_equippedArmor?.Data is ArmorData armor)
                total += armor.armor.physical;
            if (_equippedHelmet?.Data is ArmorData helmet)
                total += helmet.armor.physical;
            if (_equippedBoots?.Data is ArmorData boots)
                total += boots.armor.physical;
            if (_equippedGloves?.Data is ArmorData gloves)
                total += gloves.armor.physical;
        #region Trading
        /// Adds gold.
        public void AddGold(int amount)
            _gold = Mathf.Min(_gold + amount, GOLD_STACK_LIMIT);
            OnGoldChanged?.Invoke(_gold);
        /// Spends gold if player has enough.
        public bool SpendGold(int amount)
            if (_gold < amount) return false;
            _gold -= amount;
        #region Save/Load
        public EquipmentSaveData GetEquipmentSaveData()
            return new EquipmentSaveData
                weapon1 = _equippedWeapon1?.ItemId,
                weapon2 = _equippedWeapon2?.ItemId,
                armor = _equippedArmor?.ItemId,
                helmet = _equippedHelmet?.ItemId,
                boots = _equippedBoots?.ItemId,
                gloves = _equippedGloves?.ItemId,
                amulet = _equippedAmulet?.ItemId
            };
        public List<InventoryItemSave> GetInventorySaveData()
            var list = new List<InventoryItemSave>();
            if (_gold > 0)
                list.Add(new InventoryItemSave { id = GameConstants.ITEM_GOLD, count = _gold });
                list.Add(new InventoryItemSave { id = item.ItemId, count = item.Count });
            return list;
        public void LoadInventorySaveData(List<InventoryItemSave> data)
            Clear();
            foreach (var item in data)
                if (item.id == GameConstants.ITEM_GOLD)
                    _gold = item.count;
                else
                    AddItem(item.id, item.count);
        public void LoadEquipmentSaveData(EquipmentSaveData data)
            // Equipment is loaded by equipping items from inventory
            if (!string.IsNullOrEmpty(data.weapon1))
                var item = FindItem(data.weapon1);
                if (item != null) EquipItem(item);
            // ... etc
    }
    #region Item Instance
    /// Runtime representation of an item in inventory.
    [Serializable]
    public class ItemInstance
        public string InstanceId { get; set; }
        public string ItemId { get; set; }
        public int Count { get; set; } = 1;
        public bool IsEquipped { get; set; }
        // Runtime data (set when loaded from database)
        [NonSerialized] public ItemData Data;
        [NonSerialized] public int CurrentDurability;
        public ItemInstance()
            InstanceId = System.Guid.NewGuid().ToString();
        public ItemInstance(string itemId, int count = 1)
            ItemId = itemId;
            Count = count;
        public ItemInstance Clone()
            return new ItemInstance
                InstanceId = System.Guid.NewGuid().ToString(),
                ItemId = ItemId,
                Count = Count,
                Data = Data,
                CurrentDurability = Data?.durability?.current ?? 0
    #endregion
    #region Interface
    public interface IInventorySystem
        int Gold { get; }
        bool IsFull { get; }
        int FreeSlots { get; }
        bool AddItem(string itemId, int count = 1);
        bool RemoveItem(string itemId, int count = 1);
        bool HasItem(string itemId, int count = 1);
        int GetItemCount(string itemId);
        ItemInstance FindItem(string itemId);
        bool EquipItem(ItemInstance item);
        bool UnequipItem(ItemType type, ArmorSlot? slot = null);
        bool SpendGold(int amount);
    public class InventorySystemInterface : IInventorySystem
        private readonly InventorySystem _inventory;
        public InventorySystemInterface(InventorySystem inventory)
            _inventory = inventory;
        public int Gold => _inventory.Gold;
        public bool IsFull => _inventory.IsFull;
        public int FreeSlots => _inventory.FreeSlots;
        public bool AddItem(string itemId, int count = 1) => _inventory.AddItem(itemId, count);
        public bool RemoveItem(string itemId, int count = 1) => _inventory.RemoveItem(itemId, count);
        public bool HasItem(string itemId, int count = 1) => _inventory.HasItem(itemId, count);
        public int GetItemCount(string itemId) => _inventory.GetItemCount(itemId);
        public ItemInstance FindItem(string itemId) => _inventory.FindItem(itemId);
        public bool EquipItem(ItemInstance item) => _inventory.EquipItem(item);
        public bool UnequipItem(ItemType type, ArmorSlot? slot = null) => _inventory.UnequipItem(type, slot);
        public bool SpendGold(int amount) => _inventory.SpendGold(amount);
}
