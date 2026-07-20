#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneSetup
{
    [MenuItem("Żelazna Droga/Setup Vertical Slice Scene")]
    public static void SetupVerticalSlice()
    {
        var scene = SceneManager.GetActiveScene();
        
        // Create Bootstrap
        GameObject bootstrapGO = new GameObject("GameBootstrap");
        var bootstrap = bootstrapGO.AddComponent<ZelaznaDroga.Gameplay.GameBootstrap>();
        
        // Create Player
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        player.name = "Player";
        player.transform.position = new Vector3(0, 1, 0);
        player.AddComponent<CharacterController>();
        var pc = player.AddComponent<ZelaznaDroga.Gameplay.Player.PlayerController>();
        player.AddComponent<ZelaznaDroga.Gameplay.Progression.PlayerStats>();
        player.AddComponent<ZelaznaDroga.Gameplay.Inventory.InventorySystem>();
        player.AddComponent<ZelaznaDroga.Gameplay.Combat.CombatSystem>();
        player.AddComponent<ZelaznaDroga.Gameplay.Player.PlayerInputHandler>();
        
        // Camera
        GameObject camGO = new GameObject("Main Camera");
        Camera cam = camGO.AddComponent<Camera>();
        cam.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 2, -4);
        camGO.transform.LookAt(player.transform);
        pc.SetCamera(cam);
        
        // Add managers
        new GameObject("QuestManager").AddComponent<ZelaznaDroga.Gameplay.Quests.QuestManager>();
        new GameObject("DialogueManager").AddComponent<ZelaznaDroga.Gameplay.Dialogues.DialogueManager>();
        new GameObject("TimeManager").AddComponent<ZelaznaDroga.Gameplay.World.TimeManager>();
        new GameObject("InteractionSystem").AddComponent<ZelaznaDroga.Gameplay.Interaction.InteractionSystem>();
        new GameObject("SaveSystem").AddComponent<ZelaznaDroga.Gameplay.Save.SaveSystem>();
        
        // Add simple NPC
        GameObject npc = GameObject.CreatePrimitive(PrimitiveType.Cube);
        npc.name = "NPC_Wanderer";
        npc.transform.position = new Vector3(5, 0.5f, 2);
        var npcCtrl = npc.AddComponent<ZelaznaDroga.Gameplay.NPCs.NPCController>();
        npcCtrl.npcId = "npc_wanderer";
        
        // Add enemy
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        enemy.name = "Wolf";
        enemy.transform.position = new Vector3(-6, 0.5f, 5);
        enemy.AddComponent<ZelaznaDroga.Gameplay.Combat.EnemyController>();
        
        // Simple HUD Canvas
        GameObject canvas = new GameObject("HUDCanvas");
        Canvas c = canvas.AddComponent<Canvas>();
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvas.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        GameObject health = new GameObject("HealthSlider");
        health.transform.SetParent(canvas.transform);
        var hs = health.AddComponent<UnityEngine.UI.Slider>();
        health.GetComponent<RectTransform>().anchoredPosition = new Vector2(-300, 200);
        
        // Add HUD Manager
        var hud = canvas.AddComponent<ZelaznaDroga.UI.HUD.HUDManager>();
        hud.healthSlider = hs;
        
        Debug.Log("Vertical Slice scene setup complete! Play the scene.");
        EditorUtility.SetDirty(scene);
    }
}
#endif
