using UnityEngine;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Interaction
{
    public class Plant : MonoBehaviour, IInteractable
    {
        public string plantId = "plant_healing_herb";
        public int count = 1;
        private bool harvested = false;

        public string GetInteractionLabel() => harvested ? "Zebrano" : $"Zbierz {plantId}";

        public void Interact(GameObject interactor)
        {
            if (harvested) return;
            harvested = true;

            var inv = ComponentLocator.Get<IInventorySystem>();
            if (inv != null)
            {
                inv.AddItem(plantId, count);
            }

            GetComponent<Renderer>().material.color = Color.gray;
            Destroy(gameObject, 8f);
            Debug.Log($"[Plant] Harvested {plantId}");
        }
    }
}
