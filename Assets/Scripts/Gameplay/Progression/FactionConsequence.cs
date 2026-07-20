using UnityEngine;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Progression
{
    /// <summary>
    /// Simple consequence system. Old Order NPCs become hostile if you joined New, etc.
    /// </summary>
    public class FactionConsequence : MonoBehaviour
    {
        public string myFaction = "OldOrder"; // or "NewOrder"

        private void Start()
        {
            var fm = ComponentLocator.Get<IFactionManager>();
            if (fm == null || !fm.IsFactionChosen()) return;

            bool joinedOld = fm.ChooseFaction("OldOrder"); // we just check state
            // Since we can't easily query, use a simple check via quest flags
            var qm = ComponentLocator.Get<IQuestManager>();
            bool joinedOldOrder = qm != null && qm.IsQuestCompleted("Q_M_FINAL_OLD");
            bool joinedNewOrder = qm != null && qm.IsQuestCompleted("Q_M_FINAL_NEW");

            if ((myFaction == "OldOrder" && joinedNewOrder) || (myFaction == "NewOrder" && joinedOldOrder))
            {
                // Make hostile
                var npc = GetComponent<NPCs.NPCController>();
                if (npc) 
                {
                    // Simple: change color to red and add enemy component
                    GetComponent<Renderer>().material.color = Color.red * 0.8f;
                    gameObject.AddComponent<Combat.EnemyController>();
                    Debug.Log($"[FactionConsequence] {name} became hostile!");
                }
            }
        }
    }
}
