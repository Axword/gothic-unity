using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Inventory;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Simple merchant shop interaction.
    /// Press E to open basic buy/sell interface (console + keyboard for demo).
    /// </summary>
    public class MerchantShop : MonoBehaviour, IInteractable
    {
        public string[] sellableItems = { "item_potion_health_small", "item_lockpick", "item_iron_sword_01" };
        public int[] prices = { 25, 8, 45 };

        private TradingSystem _trading;
        private bool _shopOpen = false;

        private void Start()
        {
            _trading = GetComponent<TradingSystem>();
            if (_trading == null)
                _trading = gameObject.AddComponent<TradingSystem>();
        }

        public string GetInteractionLabel() => "Porozmawiaj z kupcem (sklep)";

        public void Interact(GameObject interactor)
        {
            _shopOpen = !_shopOpen;
            if (_shopOpen)
            {
                OpenShop();
            }
            else
            {
                Debug.Log("[Shop] Zamknięto sklep.");
            }
        }

        private void OpenShop()
        {
            Debug.Log("=== SKLEP ===");
            Debug.Log("1 = Kup miksturę (25g)");
            Debug.Log("2 = Kup wytrych (8g)");
            Debug.Log("3 = Kup miecz (45g)");
            Debug.Log("4 = Sprzedaj coś (demo)");
            Debug.Log("5 = Zamknij");
        }

        private void Update()
        {
            if (!_shopOpen) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _trading?.BuyItem("item_potion_health_small");
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _trading?.BuyItem("item_lockpick");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _trading?.BuyItem("item_iron_sword_01");
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                // Sell first item in player inventory (demo)
                var inv = ComponentLocator.Get<IInventorySystem>();
                if (inv != null && inv.Items.Count > 0)
                {
                    string id = inv.Items[0].ItemId;
                    _trading?.SellItem(id);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.E))
            {
                _shopOpen = false;
                Debug.Log("[Shop] Do zobaczenia.");
            }
        }
    }
}