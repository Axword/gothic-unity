using UnityEngine;
using ZelaznaDroga.Gameplay.Player;
using ZelaznaDroga.Gameplay.Progression;
using ZelaznaDroga.Gameplay.Inventory;
using ZelaznaDroga.Gameplay.Combat;
using ZelaznaDroga.Gameplay.Interaction;
using ZelaznaDroga.Gameplay.NPCs;
using ZelaznaDroga.Gameplay.Quests;
using ZelaznaDroga.Gameplay.Dialogues;
using ZelaznaDroga.Gameplay.World;
using ZelaznaDroga.Gameplay.Save;
using ZelaznaDroga.UI.HUD;

namespace ZelaznaDroga.Gameplay
{
    public class RuntimeSceneSetup : MonoBehaviour
    {
        private static bool _hasSetup = false;

        void Awake()
        {
            if (_hasSetup) return;
            _hasSetup = true;

            Debug.Log("[RuntimeSceneSetup] Setting up rich vertical slice...");

            CreatePlayer();
            CreateCoreManagers();
            CreateWorldContent();
            CreateUI();
            Invoke(nameof(StartMainQuest), 0.6f);

            Debug.Log("[RuntimeSceneSetup] READY. Play the game!");
        }

        void CreatePlayer()
        {
            if (FindObjectOfType<PlayerController>()) return;

            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            p.name = "Player";
            p.transform.position = new Vector3(0, 1.1f, 0);
            p.tag = "Player";

            var cc = p.GetComponent<CharacterController>();
            cc.height = 1.8f; cc.radius = 0.35f;

            p.AddComponent<PlayerController>();
            p.AddComponent<PlayerStats>();
            p.AddComponent<InventorySystem>();
            p.AddComponent<CombatSystem>();
            p.AddComponent<PlayerInputHandler>();
            p.AddComponent<SpellCaster>();
            p.AddComponent<BowController>();
            p.AddComponent<ZelaznaDroga.Gameplay.World.SleepSystem>();

            var inv = p.GetComponent<InventorySystem>();
            inv.AddItem("item_old_sword_01", 1);
            inv.AddItem("item_lockpick", 3);
            inv.AddGold(30);

            var camGO = new GameObject("PlayerCamera");
            var cam = camGO.AddComponent<Camera>();
            cam.tag = "MainCamera";
            camGO.transform.SetParent(p.transform);
            camGO.transform.localPosition = new Vector3(0, 1.7f, -3.5f);

            p.GetComponent<PlayerController>().SetCamera(cam);
        }

        void CreateCoreManagers()
        {
            CreateManager<QuestManager>("QuestManager");
            CreateManager<DialogueManager>("DialogueManager");
            CreateManager<TimeManager>("TimeManager");
            CreateManager<InteractionSystem>("InteractionSystem");
            CreateManager<SaveSystem>("SaveSystem");
            CreateManager<FactionManager>("FactionManager");
            CreateManager<WeatherAndTimeEffects>("WeatherManager");
            CreateManager<CrimeSystem>("CrimeSystem");
            CreateManager<DynamicEventSystem>("DynamicEvents");
        }

        void CreateManager<T>(string name) where T : Component
        {
            if (FindObjectOfType<T>() == null) new GameObject(name).AddComponent<T>();
        }

        void CreateWorldContent()
        {
            // NPCs
            CreateNPC("NPC_Wanderer", new Vector3(4, 0.6f, 2), Color.green, "npc_wanderer");
            CreateNPC("NPC_Aldona", new Vector3(-8, 0.6f, 3), Color.red, "npc_aldona");
            CreateNPC("NPC_Boruk", new Vector3(-6, 0.6f, 8), Color.gray, "npc_boruk");
            CreateNPC("NPC_Drwal", new Vector3(12, 0.6f, -7), new Color(0.4f,0.3f,0.2f), "npc_drwal");
            CreateNPC("NPC_Plomienny", new Vector3(18, 0.6f, 10), new Color(1f,0.4f,0.1f), "npc_plomienny");
            CreateNPC("NPC_Kosa", new Vector3(10, 0.6f, -2), Color.black, "npc_kosa");

            // More NPCs scattered
            CreateNPC("NPC_Mlynarczyk", new Vector3(-2, 0.6f, 1), new Color(0.8f,0.7f,0.5f), "npc_mlynarczyk");
            CreateNPC("NPC_Zosia", new Vector3(1, 0.6f, 4), Color.white, "npc_zosia");
            CreateNPC("NPC_Guard2", new Vector3(-10, 0.6f, 5), Color.gray, "npc_guard2");
            CreateNPC("NPC_Grom", new Vector3(13, 0.6f, -4), new Color(0.5f,0.3f,0.1f), "npc_grom");

            // Enemies
            CreateEnemy("Wolf1", new Vector3(-5, 0.6f, 6), 35, 8);
            CreateEnemy("Wolf2", new Vector3(-8, 0.6f, 9), 35, 8);
            CreateEnemy("Bandit", new Vector3(8, 0.6f, -12), 55, 11);
            CreateEnemy("Bear", new Vector3(-15, 0.6f, 14), 120, 22);

            // Attach better patrol AI to some
            GameObject.Find("Wolf1")?.AddComponent<ZelaznaDroga.AI.SimplePatrolAI>();
            GameObject.Find("Bandit")?.AddComponent<ZelaznaDroga.AI.SimplePatrolAI>();

            // Add merchant to Mlynarczyk
            var merchant = GameObject.Find("NPC_Mlynarczyk");
            if (merchant)
            {
                merchant.AddComponent<MerchantShop>();
                merchant.AddComponent<TradingSystem>();
            }

            // Add schedule executors to key NPCs
            var aldona = GameObject.Find("NPC_Aldona");
            if (aldona) aldona.AddComponent<NPCScheduleExecutor>().scheduleId = "npc_aldona_schedule";

            var boruk = GameObject.Find("NPC_Boruk");
            if (boruk) boruk.AddComponent<NPCScheduleExecutor>().scheduleId = "npc_boruk_schedule";

            // Chests
            CreateChest(new Vector3(2, 0.4f, -3), false, "Chest_Regular");
            CreateChest(new Vector3(-3, 0.4f, 9), true, "Chest_Locked");

            // Plants
            for (int i = 0; i < 12; i++)
            {
                var plant = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                plant.name = $"Plant_{i}";
                plant.transform.position = new Vector3(Random.Range(-16f, 18f), 0.3f, Random.Range(-13f, 13f));
                plant.GetComponent<Renderer>().material.color = Color.green;
                var p = plant.AddComponent<Plant>();
                p.plantId = (i % 3 == 0) ? "plant_mana_flower" : "plant_healing_herb";
            }

            // Faction choice points
            var oldC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            oldC.name = "Choice_OldOrder";
            oldC.transform.position = new Vector3(-12, 0.5f, 0);
            oldC.GetComponent<Renderer>().material.color = Color.red;
            oldC.AddComponent<FactionChoiceTrigger>().faction = "OldOrder";

            var newC = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            newC.name = "Choice_NewOrder";
            newC.transform.position = new Vector3(14, 0.5f, -5);
            newC.GetComponent<Renderer>().material.color = new Color(0.3f, 0.7f, 0.3f);
            newC.AddComponent<FactionChoiceTrigger>().faction = "NewOrder";

            // Trainer marker
            var trainer = GameObject.CreatePrimitive(PrimitiveType.Cube);
            trainer.name = "Trainer_Boruk";
            trainer.transform.position = new Vector3(-5.5f, 0.6f, 7.5f);
            trainer.GetComponent<Renderer>().material.color = Color.gray;
            trainer.AddComponent<TrainerInteraction>().skill = "strength";
        }

        void CreateNPC(string name, Vector3 pos, Color col, string npcId)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.position = pos;
            go.GetComponent<Renderer>().material.color = col;
            var ctrl = go.AddComponent<NPCController>();
            ctrl.npcId = npcId;
        }

        void CreateEnemy(string name, Vector3 pos, int hp, int dmg)
        {
            var e = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            e.name = name;
            e.transform.position = pos;
            e.GetComponent<Renderer>().material.color = Color.gray;
            var ec = e.AddComponent<EnemyController>();
            ec.maxHealth = hp; ec.currentHealth = hp; ec.damage = dmg;
        }

        void CreateChest(Vector3 pos, bool locked, string name)
        {
            var c = GameObject.CreatePrimitive(PrimitiveType.Cube);
            c.name = name;
            c.transform.position = pos;
            c.GetComponent<Renderer>().material.color = locked ? new Color(0.2f,0.15f,0.1f) : new Color(0.4f,0.3f,0.2f);
            var chest = c.AddComponent<Chest>();
            chest.isLocked = locked;
            chest.chestId = name;
            if (!locked)
            {
                chest.lootItems = new[] { "item_potion_health_small", "gold" };
                chest.lootCounts = new[] { 1, 25 };
            }
        }

        void CreateUI()
        {
            var canvasGO = new GameObject("MainUI");
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            canvasGO.AddComponent<ZelaznaDroga.UI.Menus.InventoryUI>();
            canvasGO.AddComponent<ZelaznaDroga.UI.Menus.QuestJournalUI>();
            canvasGO.AddComponent<ZelaznaDroga.UI.Menus.CharacterUI>();
            canvasGO.AddComponent<ZelaznaDroga.UI.Dialogues.DialogueUI>();
            canvasGO.AddComponent<ZelaznaDroga.UI.Menus.PauseMenu>();
            canvasGO.AddComponent<ZelaznaDroga.UI.HUD.WantedHUD>();

            // Instructions
            var txtGO = new GameObject("Instructions");
            txtGO.transform.SetParent(canvasGO.transform);
            var txt = txtGO.AddComponent<UnityEngine.UI.Text>();
            txt.text = "Żelazna Droga — Vertical Slice\nWASD + Mysz | E=interakcja | LPM=atak | F=czar | PPM=łuk\nI=ekwipunek J=dziennik C=postać";
            txt.fontSize = 16;
            txt.color = Color.white;
            var rt = txtGO.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 1); rt.anchorMax = new Vector2(0.5f, 1);
            rt.anchoredPosition = new Vector2(0, -35);
            rt.sizeDelta = new Vector2(620, 70);
        }

        void StartMainQuest()
        {
            var qm = FindObjectOfType<QuestManager>();
            qm?.StartQuest("Q_M_001");
            Debug.Log("[RuntimeSceneSetup] Main quest Q_M_001 started.");
        }
    }
}
