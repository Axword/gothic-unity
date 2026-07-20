using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Progression;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Attach to NPC that is a trainer. Simple spending of learning points.
    /// </summary>
    public class TrainerInteraction : MonoBehaviour, IInteractable
    {
        public string trainerId = "trainer_combat";
        public string skill = "strength";

        public string GetInteractionLabel() => $"Trenuj ({skill}) - 1 LP";

        public void Interact(GameObject interactor)
        {
            var stats = ComponentLocator.Get<IPlayerStats>() as PlayerStats;
            if (stats == null) return;

            bool success = stats.SpendLearningPoints(skill, 1);
            if (success)
            {
                Debug.Log($"[Trainer] Trained {skill} with 1 LP");
            }
            else
            {
                Debug.Log("[Trainer] Not enough learning points or invalid stat.");
            }
        }
    }
}
