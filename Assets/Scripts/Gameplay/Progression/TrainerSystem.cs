using System;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Progression
{
    /// <summary>
    /// Trainer system stub - player can spend LP on stats via NPC.
    /// </summary>
    public class TrainerSystem : BaseMonoBehaviour
    {
        public void TrainStat(string statName, int points = 1)
        {
            var stats = ComponentLocator.Get<IPlayerStats>();
            if (stats == null) return;
            bool success = stats.SpendLearningPoints(statName, points);
            if (success)
            {
                Debug.Log($"[Trainer] Trained {statName} +{points}");
            }
            else
                Debug.Log("[Trainer] Not enough LP or invalid stat");
        }
    }
}
