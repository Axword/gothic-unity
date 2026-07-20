using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.NPCs;

namespace ZelaznaDroga.AI
{
    /// <summary>
    /// Simple waypoint patrol + basic detection for NPCs and enemies.
    /// No NavMesh required — uses direct movement.
    /// </summary>
    public class SimplePatrolAI : BaseMonoBehaviour
    {
        [Header("Patrol")]
        public Transform[] waypoints;
        public float moveSpeed = 2.5f;
        public float waitTime = 2f;

        [Header("Detection")]
        public float detectionRange = 10f;
        public float attackRange = 2f;
        public LayerMask playerLayer;

        private int _currentWaypoint = 0;
        private float _waitTimer;
        private bool _isWaiting;
        private Transform _player;
        private NPCController _npc;
        private EnemyController _enemy;

        private void Start()
        {
            _npc = GetComponent<NPCController>();
            _enemy = GetComponent<EnemyController>();

            if (waypoints == null || waypoints.Length == 0)
            {
                // Create default patrol points around start position
                waypoints = new Transform[3];
                for (int i = 0; i < 3; i++)
                {
                    GameObject wp = new GameObject($"Waypoint_{i}");
                    wp.transform.position = transform.position + new Vector3(Mathf.Cos(i * 2f) * 4f, 0, Mathf.Sin(i * 2f) * 4f);
                    waypoints[i] = wp.transform;
                }
            }
        }

        private void Update()
        {
            if (_player == null)
            {
                var p = ComponentLocator.Get<IPlayerController>();
                if (p != null) _player = ((MonoBehaviour)p).transform;
            }

            if (_player != null && Vector3.Distance(transform.position, _player.position) < detectionRange)
            {
                // Alert / chase
                ChasePlayer();
                return;
            }

            // Normal patrol
            Patrol();
        }

        private void Patrol()
        {
            if (waypoints.Length == 0) return;

            Transform target = waypoints[_currentWaypoint];

            if (Vector3.Distance(transform.position, target.position) < 0.8f)
            {
                if (!_isWaiting)
                {
                    _isWaiting = true;
                    _waitTimer = waitTime;
                }

                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0)
                {
                    _currentWaypoint = (_currentWaypoint + 1) % waypoints.Length;
                    _isWaiting = false;
                }
                return;
            }

            Vector3 dir = (target.position - transform.position).normalized;
            transform.position += dir * moveSpeed * Time.deltaTime;
            transform.LookAt(target.position);
        }

        private void ChasePlayer()
        {
            if (_player == null) return;

            float dist = Vector3.Distance(transform.position, _player.position);

            if (dist > attackRange)
            {
                Vector3 dir = (_player.position - transform.position).normalized;
                transform.position += dir * (moveSpeed * 1.4f) * Time.deltaTime;
                transform.LookAt(_player);
            }
            else
            {
                // Attack
                if (_enemy != null)
                {
                    // Enemy will handle damage in its own Update
                }
                else if (_npc != null)
                {
                    // NPC alerts or flees
                }
            }
        }
    }
}
