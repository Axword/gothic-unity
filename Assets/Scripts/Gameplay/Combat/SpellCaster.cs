using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Schema;
using ZelaznaDroga.Gameplay.Progression;

namespace ZelaznaDroga.Gameplay.Combat
{
    /// <summary>
    /// Simple spell casting for player.
    /// </summary>
    public class SpellCaster : BaseMonoBehaviour
    {
        [SerializeField] private GameObject fireballPrefab;
        [SerializeField] private GameObject iceboltPrefab;

        private PlayerStats _stats;
        private string _activeSpell = "spell_fireball";

        private void Start()
        {
            _stats = GetComponent<PlayerStats>();
            ComponentLocator.Register<ISpellSystem>(new SpellSystemStub());
        }

        public void CastActiveSpell()
        {
            if (_stats == null || _stats.CurrentMana < 15) 
            {
                Debug.Log("[Spell] Not enough mana");
                return;
            }

            _stats.ModifyMana(-15);

            GameObject vfx = null;
            int dmg = 20;

            if (_activeSpell == "spell_fireball" && fireballPrefab)
            {
                vfx = Instantiate(fireballPrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
                dmg = Random.Range(18, 26);
            }
            else if (_activeSpell == "spell_icebolt" && iceboltPrefab)
            {
                vfx = Instantiate(iceboltPrefab, transform.position + transform.forward * 1.5f, Quaternion.identity);
                dmg = Random.Range(12, 20);
            }
            else
            {
                // fallback projectile
                vfx = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                vfx.transform.position = transform.position + transform.forward * 1.2f;
                Destroy(vfx, 2f);
            }

            // Simple forward projectile
            if (vfx) 
            {
                var rb = vfx.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.velocity = transform.forward * 18f;

                // Damage on hit (very simple)
                var trigger = vfx.AddComponent<SpellHitTrigger>();
                trigger.damage = dmg;
            }

            Debug.Log($"[Spell] Cast {_activeSpell} for {dmg} damage");
        }

        public void SwitchSpell(string spellId)
        {
            _activeSpell = spellId;
        }
    }

    public class SpellHitTrigger : MonoBehaviour
    {
        public int damage = 20;

        private void OnCollisionEnter(Collision col)
        {
            var dmg = col.gameObject.GetComponent<IDamageable>();
            if (dmg != null)
            {
                dmg.TakeDamage(damage, gameObject);
            }
            Destroy(gameObject, 0.1f);
        }
    }

    // Minimal spell system for now
    public class SpellSystemStub : ISpellSystem
    {
        public SpellData GetSpell(string spellId)
        {
            return new SpellData { Id = spellId, manaCost = 15, nameKey = spellId };
        }
    }
}
