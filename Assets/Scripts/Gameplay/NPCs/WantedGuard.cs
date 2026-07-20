using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.NPCs
{
    /// <summary>
    /// Guard that becomes aggressive when player has high wanted level.
    /// </summary>
    public class WantedGuard : BaseMonoBehaviour
    {
        public float checkInterval = 3f;
        private float _nextCheck;
        private EnemyController _enemy;

        private void Start()
        {
            _enemy = GetComponent<EnemyController>();
            if (_enemy == null) _enemy = gameObject.AddComponent<EnemyController>();
        }

        private void Update()
        {
            if (Time.time < _nextCheck) return;
            _nextCheck = Time.time + checkInterval;

            var crime = ComponentLocator.Get<World.CrimeSystem>();
            if (crime != null && crime.WantedLevel >= 2)
            {
                // Become hostile
                GetComponent<Renderer>().material.color = Color.red;
                if (_enemy) _enemy.damage = Mathf.Max(_enemy.damage, 12);
            }
        }
    }
}
