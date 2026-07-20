using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Progression;

namespace ZelaznaDroga.Gameplay.Combat
{
    public class BowController : BaseMonoBehaviour
    {
        [SerializeField] private GameObject arrowPrefab;
        [SerializeField] private float drawTime = 0.6f;
        [SerializeField] private float arrowSpeed = 22f;

        private bool _isDrawing;
        private float _drawTimer;
        private PlayerStats _stats;

        private void Start()
        {
            _stats = GetComponent<PlayerStats>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(1)) // RMB to draw bow
            {
                _isDrawing = true;
                _drawTimer = 0;
            }

            if (_isDrawing && Input.GetMouseButton(1))
            {
                _drawTimer += Time.deltaTime;
            }

            if (_isDrawing && Input.GetMouseButtonUp(1))
            {
                ShootArrow();
                _isDrawing = false;
            }
        }

        private void ShootArrow()
        {
            if (arrowPrefab == null) return;

            GameObject arrow = Instantiate(arrowPrefab, transform.position + transform.forward * 1.2f + Vector3.up * 1.2f, Quaternion.LookRotation(transform.forward));
            var rb = arrow.GetComponent<Rigidbody>();
            if (rb == null) rb = arrow.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.velocity = transform.forward * arrowSpeed;

            // Damage on hit
            var hit = arrow.AddComponent<ArrowHit>();
            hit.damage = _stats != null ? _stats.CalculateRangedDamage(12, 20) : 15;

            Debug.Log("[Bow] Shot arrow");
        }
    }

    public class ArrowHit : MonoBehaviour
    {
        public int damage = 15;

        private void OnCollisionEnter(Collision col)
        {
            var target = col.gameObject.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage, gameObject);
            }
            Destroy(gameObject, 0.2f);
        }
    }
}
