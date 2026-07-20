using System;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Inventory;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.Gameplay.Inventory
{
    /// <summary>
    /// Simple trading system. Merchants have infinite stock for demo.
    /// </summary>
    public class TradingSystem : BaseMonoBehaviour
    {
        public int buyPriceModifier = 100; // % of base value
        public int sellPriceModifier = 40;

        private IInventorySystem _playerInv;

        private void Start()
        {
            _playerInv = ComponentLocator.Get<IInventorySystem>();
        }

        public bool BuyItem(string itemId, int count = 1)
        {
            if (_playerInv == null) return false;

            // For demo we hardcode a few prices
            int price = GetItemPrice(itemId) * count;

            if (!_playerInv.SpendGold(price))
            {
                Debug.Log("[Trade] Not enough gold");
                return false;
            }

            _playerInv.AddItem(itemId, count);
            Debug.Log($"[Trade] Bought {count}x {itemId} for {price}");
            return true;
        }

        public bool SellItem(string itemId, int count = 1)
        {
            if (_playerInv == null) return false;

            if (!_playerInv.HasItem(itemId, count)) return false;

            int price = Mathf.FloorToInt(GetItemPrice(itemId) * (sellPriceModifier / 100f) * count);
            _playerInv.RemoveItem(itemId, count);
            _playerInv.AddGold(price);

            Debug.Log($"[Trade] Sold {count}x {itemId} for {price}");
            return true;
        }

        private int GetItemPrice(string itemId)
        {
            int basePrice;
            switch (itemId)
            {
                case "item_iron_sword_01": basePrice = 45; break;
                case "item_potion_health_small": basePrice = 25; break;
                case "item_lockpick": basePrice = 8; break;
                case "gold": basePrice = 1; break;
                default: basePrice = 15; break;
            }

            // Apply reputation modifier if available
            var rep = ComponentLocator.Get<Progression.IReputationSystem>();
            if (rep != null)
            {
                float mod = rep.GetPriceModifier("OldOrder"); // default to Old Order
                basePrice = Mathf.FloorToInt(basePrice * mod);
            }

            return Mathf.Max(1, basePrice);
        }
    }
}
