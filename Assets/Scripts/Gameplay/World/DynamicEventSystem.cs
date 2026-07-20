using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.World
{
    /// <summary>
    /// Very simple dynamic world events (patrols, random spawns, etc).
    /// </summary>
    public class DynamicEventSystem : BaseMonoBehaviour
    {
        private float _nextEventTime;

        private void Start()
        {
            _nextEventTime = Time.time + 45f;
        }

        private void Update()
        {
            if (Time.time > _nextEventTime)
            {
                TrySpawnRandomEvent();
                _nextEventTime = Time.time + Random.Range(60f, 120f);
            }
        }

        private void TrySpawnRandomEvent()
        {
            var player = ComponentLocator.Get<IPlayerController>();
            if (player == null) return;

            // 30% chance to spawn a small patrol or chest near player
            if (Random.value < 0.3f)
            {
                Vector3 spawnPos = player.Position + Random.insideUnitSphere * 12f;
                spawnPos.y = 0.6f;

                // Spawn a bandit patrol
                var bandit = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                bandit.name = "DynamicBandit";
                bandit.transform.position = spawnPos;
                bandit.GetComponent<Renderer>().material.color = new Color(0.25f, 0.15f, 0.1f);
                bandit.AddComponent<Combat.EnemyController>();
                bandit.AddComponent<AI.SimplePatrolAI>();

                Debug.Log("[DynamicEvent] Patrol spawned near player!");
            }
            else if (Random.value < 0.25f)
            {
                // Spawn a loot chest
                var chest = GameObject.CreatePrimitive(PrimitiveType.Cube);
                chest.name = "DynamicChest";
                chest.transform.position = player.Position + Random.insideUnitSphere * 8f + Vector3.up * 0.3f;
                chest.GetComponent<Renderer>().material.color = new Color(0.35f, 0.28f, 0.18f);
                chest.AddComponent<Interaction.Chest>();
                Debug.Log("[DynamicEvent] Random chest appeared!");
            }
        }
    }
}
