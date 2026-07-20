using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Inventory;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.UI.Menus
{
    /// <summary>
    /// Full inventory UI with categories (Weapons, Armor, Consumables, Misc).
    /// </summary>
    public class InventoryUI : BaseMonoBehaviour
    {
        [Header("Panels")]
        public GameObject inventoryPanel;
        public Transform itemListContent;
        public GameObject itemButtonPrefab;
        [Header("Details")]
        public Text itemNameText;
        public Text itemDescText;
        public Text itemStatsText;
        public Button equipButton;
        public Button useButton;
        public Button dropButton;
        [Header("Categories")]
        public Button weaponsTab;
        public Button armorTab;
        public Button consumablesTab;
        public Button miscTab;
        private IInventorySystem _inventory;
        private ItemInstance _selectedItem;
        private List<GameObject> _itemButtons = new List<GameObject>();
        private string _currentCategory = "All";
        private void Start()
        {
            _inventory = ComponentLocator.Get<IInventorySystem>();
            if (_inventory != null)
            {
                _inventory.OnInventoryChanged += Refresh;
            }
            if (weaponsTab) weaponsTab.onClick.AddListener(() => SetCategory("Weapon"));
            if (armorTab) armorTab.onClick.AddListener(() => SetCategory("Armor"));
            if (consumablesTab) consumablesTab.onClick.AddListener(() => SetCategory("Consumable"));
            if (miscTab) miscTab.onClick.AddListener(() => SetCategory("Misc"));
            if (equipButton) equipButton.onClick.AddListener(EquipSelected);
            if (useButton) useButton.onClick.AddListener(UseSelected);
            if (dropButton) dropButton.onClick.AddListener(DropSelected);
            if (inventoryPanel) inventoryPanel.SetActive(false);
        }
        private void Update()
            if (Input.GetKeyDown(KeyCode.I))
                Toggle();
        public void Toggle()
            if (inventoryPanel == null) return;
            bool show = !inventoryPanel.activeSelf;
            inventoryPanel.SetActive(show);
            if (show) Refresh();
        public void SetCategory(string cat)
            _currentCategory = cat;
            Refresh();
        public void Refresh()
            if (_inventory == null || itemListContent == null) return;
            // Clear
            foreach (var b in _itemButtons) Destroy(b);
            _itemButtons.Clear();
            var items = _inventory.Items;
            foreach (var inst in items)
                if (_currentCategory != "All" && inst.Data != null)
                {
                    string typeStr = inst.Data.itemType.ToString();
                    if (!typeStr.Contains(_currentCategory)) continue;
                }
                GameObject btn = Instantiate(itemButtonPrefab, itemListContent);
                btn.GetComponentInChildren<Text>().text = $"{inst.Data?.nameKey ?? inst.ItemId} x{inst.Count}";
                var btnComp = btn.GetComponent<Button>();
                btnComp.onClick.AddListener(() => SelectItem(inst));
                _itemButtons.Add(btn);
            if (itemNameText) itemNameText.text = "";
            if (itemDescText) itemDescText.text = "";
            if (itemStatsText) itemStatsText.text = "";
        private void SelectItem(ItemInstance item)
            _selectedItem = item;
            if (item.Data == null) return;
            if (itemNameText) itemNameText.text = item.Data.nameKey;
            if (itemDescText) itemDescText.text = item.Data.descriptionKey;
            string stats = $"Wartość: {item.Data.baseValue}\nWaga: {item.Data.weight}";
            if (item.Data is WeaponData w)
                stats += $"\nObrażenia: {w.damage.min}-{w.damage.max}";
            if (itemStatsText) itemStatsText.text = stats;
            bool canEquip = item.Data.itemType == ItemType.Weapon || item.Data.itemType == ItemType.Armor;
            if (equipButton) equipButton.gameObject.SetActive(canEquip);
            if (useButton) useButton.gameObject.SetActive(item.Data.itemType == ItemType.Consumable);
        private void EquipSelected()
            if (_selectedItem != null && _inventory != null)
                _inventory.EquipItem(_selectedItem);
                Refresh();
        private void UseSelected()
            if (_selectedItem != null && _inventory != null && _selectedItem.Data?.itemType == ItemType.Consumable)
                // Simple consume: heal
                var stats = ComponentLocator.Get<IPlayerStats>();
                if (stats != null)
                    stats.ModifyHealth(25);
                    _inventory.RemoveItem(_selectedItem, 1);
        private void DropSelected()
                _inventory.RemoveItem(_selectedItem, 1);
    }
}
