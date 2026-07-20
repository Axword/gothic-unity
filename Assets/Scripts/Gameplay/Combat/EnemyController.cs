using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Schema;
using ZelaznaDroga.Gameplay.Combat;

namespace ZelaznaDroga.Gameplay.Combat
{
    /// <summary>
    /// Simple enemy that can be attacked and dies.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class EnemyController : BaseMonoBehaviour, IDamageable
    {
        public string monsterId = "wolf";
        public int maxHealth = 40;
        public int currentHealth = 40;
        public int damage = 8;
        private bool _isDead;
        private void Start()
        {
            currentHealth = maxHealth;
        }
    public void TakeDamage(int dmg, GameObject attacker)
        if (_isDead) return;
        currentHealth -= dmg;
        Debug.Log($"[Enemy] {name} took {dmg} damage, HP: {currentHealth}");
        // Hit reaction
        var reaction = GetComponent<HitReaction>();
        if (reaction == null) reaction = gameObject.AddComponent<HitReaction>();
        Vector3 dir = (transform.position - attacker.transform.position).normalized;
        reaction.OnHit(dir, dmg);
        if (currentHealth <= 0)
            Die(attacker);
    }
        public void Die(GameObject killer)
            if (_isDead) return;
            _isDead = true;
            Debug.Log($"[Enemy] {name} died");
            var stats = ComponentLocator.Get<IPlayerStats>();
            stats?.AddXp(25);
            var inv = ComponentLocator.Get<IInventorySystem>();
            if (inv != null && Random.value > 0.4f)
            {
                inv.AddItem("item_potion_health_small");
            }
            // Add skinning component instead of immediate destroy
            var skin = gameObject.AddComponent<Interaction.SkinningSystem>();
            skin.trophyItem = "item_wolf_pelt";
            skin.minCount = 1;
            skin.maxCount = 2;
            // Change to "dead" visual
            GetComponent<Renderer>().material.color = new Color(0.2f, 0.1f, 0.1f);
            // Keep the object so player can skin it
        // Simple AI chase stub
        private void Update()
            var player = ComponentLocator.Get<IPlayerController>();
            if (player == null) return;
            float dist = Vector3.Distance(transform.position, player.Position);
            if (dist < 12f && dist > 1.5f)
                Vector3 dir = (player.Position - transform.position).normalized;
                transform.position += dir * 3f * Time.deltaTime;
                transform.LookAt(player.Position);
            else if (dist <= 1.5f)
                // Attack player
                var combat = ComponentLocator.Get<ICombatSystem>();
                // Simple damage to player
                var stats = ComponentLocator.Get<IPlayerStats>();
                if (stats != null)
                {
                    stats.ModifyHealth(-damage);
                }
}
