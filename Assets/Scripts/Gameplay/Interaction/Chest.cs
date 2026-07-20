using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Inventory;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Simple chest that gives loot. Supports basic lockpick stub.
    /// </summary>
    public class Chest : MonoBehaviour, IInteractable
    {
        public string chestId = "chest_01";
        public string[] lootItems = { "item_potion_health_small", "gold" };
        public int[] lootCounts = { 1, 15 };
        public bool isLocked = false;
        public int lockDifficulty = 1;
        public bool isOpen = false;

        private bool _looted = false;

        public string GetInteractionLabel()
        {
            if (isOpen || _looted) return "Otwarta skrzynia (pusta)";
            return isLocked ? "Otwórz zamek (minigra)" : "Otwórz skrzynię";
        }

        public void Interact(GameObject interactor)
        {
            if (_looted || isOpen) return;

            if (isLocked)
            {
                // Stub minigame - always succeed for vertical slice
                Debug.Log("[Chest] Minigra zamka - sukces (prosta implementacja)");
                // In full: would start LockpickMinigame UI
            }

            OpenChest();
        }

        private void OpenChest()
        {
            isOpen = true;
            _looted = true;

            var inv = ComponentLocator.Get<IInventorySystem>();
            if (inv != null)
            {
                for (int i = 0; i < lootItems.Length; i++)
                {
                    inv.AddItem(lootItems[i], lootCounts.Length > i ? lootCounts[i] : 1);
                }
            }

            Debug.Log($"[Chest] Opened {chestId} - loot given");

            // Report crime if it was someone else's chest
            var crime = ComponentLocator.Get<World.CrimeSystem>();
            if (crime != null && name.Contains("Locked"))
            {
                var player = ComponentLocator.Get<IPlayerController>();
                if (player != null)
                {
                    crime.ReportCrime(((MonoBehaviour)player).gameObject, this.gameObject, "theft");
                }
            }

            // Visual
            var rend = GetComponent<Renderer>();
            if (rend) rend.material.color = Color.gray;
        }
    }
}
