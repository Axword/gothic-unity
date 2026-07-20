using UnityEngine;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Simple skinning system. Use on dead enemies.
    /// Requires "skórowanie" skill (stubbed via learning points).
    /// </summary>
    public class SkinningSystem : BaseMonoBehaviour
    {
        public string trophyItem = "item_wolf_pelt";
        public int minCount = 1;
        public int maxCount = 2;
        public bool requiresSkill = true;
        private bool _skinned = false;
        public void TrySkin(GameObject interactor)
        {
            if (_skinned) return;
            var inv = ComponentLocator.Get<IInventorySystem>();
            if (inv == null) return;
            // Simple skill check - if player has spent any LP, allow better loot
            var stats = ComponentLocator.Get<IPlayerStats>();
            bool skilled = stats != null && stats.LearningPoints > 0; // crude check
            int count = Random.Range(minCount, maxCount + (skilled ? 1 : 0));
            inv.AddItem(trophyItem, count);
            _skinned = true;
            GetComponent<Renderer>().material.color = Color.black;
            Debug.Log($"[Skinning] Got {count}x {trophyItem}");
            Destroy(gameObject, 4f);
        }
    }
}
