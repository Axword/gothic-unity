using UnityEngine;
using ZelaznaDroga.Core.Attributes;

namespace ZelaznaDroga.Gameplay.Combat
{
    /// <summary>
    /// Simple hit reaction for any damageable.
    /// Flashes red and pushes back.
    /// </summary>
    public class HitReaction : BaseMonoBehaviour
    {
        [SerializeField] private float flashDuration = 0.15f;
        [SerializeField] private float knockbackForce = 3f;

        private Renderer _renderer;
        private Color _originalColor;
        private Rigidbody _rb;
        private bool _flashing;

        private void Start()
        {
            _renderer = GetComponent<Renderer>();
            if (_renderer) _originalColor = _renderer.material.color;

            _rb = GetComponent<Rigidbody>();
        }

        public void OnHit(Vector3 hitDirection, int damage)
        {
            // Visual flash
            if (_renderer && !_flashing)
            {
                _flashing = true;
                _renderer.material.color = Color.red;
                Invoke(nameof(ResetColor), flashDuration);
            }

            // Knockback
            if (_rb)
            {
                _rb.AddForce(hitDirection.normalized * knockbackForce, ForceMode.Impulse);
            }
            else
            {
                // Fallback for CharacterController objects
                transform.position += hitDirection.normalized * (knockbackForce * 0.3f);
            }

            // Small hit effect
            if (damage > 15)
            {
                var p = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                p.transform.position = transform.position + Vector3.up;
                p.transform.localScale = Vector3.one * 0.3f;
                p.GetComponent<Renderer>().material.color = Color.yellow;
                Destroy(p, 0.2f);
            }
        }

        private void ResetColor()
        {
            if (_renderer)
                _renderer.material.color = _originalColor;
            _flashing = false;
        }
    }
}