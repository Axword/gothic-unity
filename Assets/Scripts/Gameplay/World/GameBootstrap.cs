using System.Collections;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Json;
using ZelaznaDroga.Gameplay.Player;
using ZelaznaDroga.Gameplay.Progression;
using ZelaznaDroga.Gameplay.Inventory;
using ZelaznaDroga.Gameplay.Quests;
using ZelaznaDroga.Gameplay.Dialogues;
using ZelaznaDroga.Gameplay.Combat;
using ZelaznaDroga.Gameplay.World;

namespace ZelaznaDroga.Gameplay
{
    /// <summary>
    /// Bootstrap script that initializes all game systems on scene start.
    /// Attach to an empty GameObject named "GameBootstrap" in main scene.
    /// </summary>
    public class GameBootstrap : MonoBehaviour
    {
        [Header("Player Setup")]
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private Transform playerSpawnPoint;

        [Header("Systems")]
        [SerializeField] private bool autoStartGame = true;

        private JsonDataLoader _dataLoader;
        private PlayerController _playerController;
        private PlayerStats _playerStats;
        private InventorySystem _inventory;
        private QuestManager _questManager;
        private DialogueManager _dialogueManager;
        private CombatSystem _combatSystem;
        private TimeManager _timeManager;

        private void Awake()
        {
            InitializeDataLoader();
            InitializeCoreSystems();
        }

        private void Start()
        {
            if (autoStartGame)
            {
                StartCoroutine(InitializeGame());
            }
        }

        private void InitializeDataLoader()
        {
            _dataLoader = new JsonDataLoader();
            ComponentLocator.Register(_dataLoader);
            Debug.Log("[Bootstrap] Data loader initialized");
        }

        private void InitializeCoreSystems()
        {
            // Create or find systems
            _timeManager = FindObjectOfType<TimeManager>();
            if (_timeManager == null)
            {
                GameObject timeGO = new GameObject("TimeManager");
                _timeManager = timeGO.AddComponent<TimeManager>();
            }

            _questManager = FindObjectOfType<QuestManager>();
            if (_questManager == null)
            {
                GameObject questGO = new GameObject("QuestManager");
                _questManager = questGO.AddComponent<QuestManager>();
                _questManager.GetComponent<QuestManager>()._dataLoader = _dataLoader; // hack for inspector
            }

            _dialogueManager = FindObjectOfType<DialogueManager>();
            if (_dialogueManager == null)
            {
                GameObject dialogueGO = new GameObject("DialogueManager");
                _dialogueManager = dialogueGO.AddComponent<DialogueManager>();
            }

            Debug.Log("[Bootstrap] Core systems initialized");
        }

        private IEnumerator InitializeGame()
        {
            yield return null; // Wait one frame

            // Spawn player if not present
            SpawnPlayer();

            // Start main quest
            if (_questManager != null)
            {
                _questManager.StartQuest("Q_M_001");
                Debug.Log("[Bootstrap] Started main quest Q_M_001");
            }

            // Give starting items
            if (_inventory != null)
            {
                _inventory.AddItem("item_old_sword_01");
                _inventory.AddGold(25);
            }

            // Setup time
            if (_timeManager != null)
            {
                // Start at morning
            }

            Debug.Log("[Bootstrap] Game initialized. Welcome to Żelazna Droga!");
            
            // Notify that game is ready
            EventBus.Publish(new GameInitializedEvent());
        }

        private void SpawnPlayer()
        {
            // Find existing player or spawn
            var existing = FindObjectOfType<PlayerController>();
            if (existing != null)
            {
                _playerController = existing;
                Debug.Log("[Bootstrap] Found existing player");
                return;
            }

            Vector3 spawnPos = playerSpawnPoint != null ? playerSpawnPoint.position : new Vector3(0, 1, 0);
            Quaternion spawnRot = playerSpawnPoint != null ? playerSpawnPoint.rotation : Quaternion.identity;

            GameObject playerGO;
            if (playerPrefab != null)
            {
                playerGO = Instantiate(playerPrefab, spawnPos, spawnRot);
            }
            else
            {
                playerGO = new GameObject("Player");
                playerGO.transform.position = spawnPos;
                playerGO.transform.rotation = spawnRot;
                
                // Add required components
                var cc = playerGO.AddComponent<CharacterController>();
                cc.height = 1.8f;
                cc.radius = 0.4f;

                _playerController = playerGO.AddComponent<PlayerController>();
                
                // Add stats
                _playerStats = playerGO.AddComponent<PlayerStats>();
                
                // Add inventory
                _inventory = playerGO.AddComponent<InventorySystem>();
                
                // Add combat
                _combatSystem = playerGO.AddComponent<CombatSystem>();
                
                // Camera setup
                GameObject camGO = new GameObject("PlayerCamera");
                Camera cam = camGO.AddComponent<Camera>();
                camGO.transform.SetParent(playerGO.transform);
                camGO.transform.localPosition = new Vector3(0, 1.6f, 0);
                _playerController.SetCamera(cam);
                
                // Add basic input handling (assume InputActions attached or use legacy for now)
                Debug.Log("[Bootstrap] Created basic player");
            }

            if (_playerController != null)
            {
                ComponentLocator.Register<IPlayerController>(new PlayerControllerInterface(_playerController));
            }

            if (_playerStats != null)
            {
                ComponentLocator.Register<IPlayerStats>(new PlayerStatsInterface(_playerStats));
            }

            if (_inventory != null)
            {
                ComponentLocator.Register<IInventorySystem>(new InventorySystemInterface(_inventory));
            }
        }

        private void OnGUI()
        {
            // Simple debug HUD for testing
            if (Event.current.type == EventType.Repaint)
            {
                GUI.Label(new Rect(10, 10, 400, 20), "Żelazna Droga - Vertical Slice");
                GUI.Label(new Rect(10, 30, 400, 20), "Press E to interact (demo)");
                if (_playerStats != null)
                {
                    GUI.Label(new Rect(10, 50, 300, 20), $"HP: {_playerStats.CurrentHealth}/{_playerStats.MaxHealth}  Level: {_playerStats.Level}");
                }
            }
        }
    }

    public class GameInitializedEvent { }
}
