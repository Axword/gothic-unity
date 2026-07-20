using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Progression;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Place two of these in the world (Old Order and New Order choice).
    /// </summary>
    public class FactionChoiceTrigger : MonoBehaviour, IInteractable
    {
        public string faction = "OldOrder"; // or "NewOrder"

        public string GetInteractionLabel()
        {
            var fm = ComponentLocator.Get<IFactionManager>();
            if (fm != null && fm.IsFactionChosen())
                return "Już wybrałeś stronę";
            return faction == "OldOrder" ? "Dołącz do Gildii Żelaznej" : "Dołącz do Wolnych";
        }

        public void Interact(GameObject interactor)
        {
            var fm = ComponentLocator.Get<IFactionManager>();
            if (fm == null || fm.IsFactionChosen()) return;

            bool success = fm.ChooseFaction(faction);
            if (success)
            {
                Debug.Log($"[FactionChoice] Player joined {faction}");
                // Simple end message
                var txt = GameObject.Find("Instructions")?.GetComponent<UnityEngine.UI.Text>();
                if (txt) txt.text = $"Wybrałeś {faction}!\nGra zakończona (epilog).";
            }
        }
    }
}
