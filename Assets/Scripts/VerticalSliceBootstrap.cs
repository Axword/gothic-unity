using UnityEngine;

namespace ZelaznaDroga
{
    /// <summary>
    /// Creates a dependency-free greybox scene so the vertical slice can be
    /// opened and played before art assets and prefabs are available.
    /// </summary>
    public sealed class VerticalSliceBootstrap : MonoBehaviour
    {
        [SerializeField] private bool createGreybox = true;

        private void Start()
        {
            if (!createGreybox) return;

            CreateEnvironment();
            var player = CreatePlayer();
            var npc = CreateNpc();
            CreateHud(player, npc);
        }

        private static void CreateEnvironment()
        {
            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Greybox Ground";
            ground.transform.localScale = new Vector3(5f, 1f, 5f);
            ground.GetComponent<Renderer>().material.color = new Color(0.16f, 0.18f, 0.16f);

            var lightObject = new GameObject("Sun");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.1f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static GameObject CreatePlayer()
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player (Greybox)";
            player.transform.position = new Vector3(0f, 1f, -4f);
            player.GetComponent<Renderer>().material.color = new Color(0.25f, 0.45f, 0.7f);
            ProceduralModelFactory.BuildHumanoid(player, new Color(0.25f, 0.45f, 0.7f));
            player.AddComponent<ProceduralModelAnimator>();

            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            var camera = cameraObject.AddComponent<Camera>();
            cameraObject.transform.position = new Vector3(0f, 4f, -9f);
            cameraObject.transform.LookAt(player.transform.position + Vector3.up);
            camera.fieldOfView = 60f;
            player.AddComponent<VerticalSlicePlayer>();
            return player;
        }

        private static GameObject CreateNpc()
        {
            var npc = GameObject.CreatePrimitive(PrimitiveType.Cube);
            npc.name = "Aldona (Dialogue NPC)";
            npc.transform.position = new Vector3(0f, 1f, 3f);
            npc.transform.localScale = new Vector3(1.2f, 2f, 1.2f);
            npc.GetComponent<Renderer>().material.color = new Color(0.65f, 0.25f, 0.18f);
            ProceduralModelFactory.BuildHumanoid(npc, new Color(0.65f, 0.25f, 0.18f));
            npc.AddComponent<ProceduralModelAnimator>();
            return npc;
        }

        private static void CreateHud(GameObject player, GameObject npc)
        {
            var canvasObject = new GameObject("Vertical Slice HUD");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var textObject = new GameObject("Instructions");
            textObject.transform.SetParent(canvasObject.transform, false);
            var text = textObject.AddComponent<UnityEngine.UI.Text>();
            text.text = "ŻELAZNA DROGA — VERTICAL SLICE\n\nNiebieski: gracz    Czerwony: Aldona\n\nScena testowa gotowa. Następny krok: podłączenie PlayerController i dialogu.";
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            var rect = text.rectTransform;
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(24f, -24f);
            rect.sizeDelta = new Vector2(700f, 150f);

            var promptObject = new GameObject("Interaction Prompt");
            promptObject.transform.SetParent(canvasObject.transform, false);
            var prompt = promptObject.AddComponent<UnityEngine.UI.Text>();
            prompt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            prompt.fontSize = 22;
            prompt.color = Color.yellow;
            prompt.alignment = TextAnchor.LowerCenter;
            var promptRect = prompt.rectTransform;
            promptRect.anchorMin = new Vector2(0.5f, 0f);
            promptRect.anchorMax = new Vector2(0.5f, 0f);
            promptRect.pivot = new Vector2(0.5f, 0f);
            promptRect.anchoredPosition = new Vector2(0f, 30f);
            promptRect.sizeDelta = new Vector2(700f, 50f);

            var dialogueObject = new GameObject("Dialogue Text");
            dialogueObject.transform.SetParent(canvasObject.transform, false);
            var dialogue = dialogueObject.AddComponent<UnityEngine.UI.Text>();
            dialogue.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            dialogue.fontSize = 20;
            dialogue.color = Color.white;
            dialogue.alignment = TextAnchor.MiddleCenter;
            var dialogueRect = dialogue.rectTransform;
            dialogueRect.anchorMin = new Vector2(0.5f, 0f);
            dialogueRect.anchorMax = new Vector2(0.5f, 0f);
            dialogueRect.pivot = new Vector2(0.5f, 0f);
            dialogueRect.anchoredPosition = new Vector2(0f, 90f);
            dialogueRect.sizeDelta = new Vector2(900f, 120f);

            var questObject = new GameObject("Quest System");
            var quest = questObject.AddComponent<VerticalSliceQuest>();
            var questTextObject = new GameObject("Quest Journal");
            questTextObject.transform.SetParent(canvasObject.transform, false);
            var questText = questTextObject.AddComponent<UnityEngine.UI.Text>();
            questText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            questText.fontSize = 16;
            questText.color = Color.cyan;
            questText.alignment = TextAnchor.UpperRight;
            var questRect = questText.rectTransform;
            questRect.anchorMin = new Vector2(1f, 1f);
            questRect.anchorMax = new Vector2(1f, 1f);
            questRect.pivot = new Vector2(1f, 1f);
            questRect.anchoredPosition = new Vector2(-24f, -24f);
            questRect.sizeDelta = new Vector2(360f, 90f);
            quest.Configure(questText);

            var combatObject = new GameObject("Combat Feedback");
            combatObject.transform.SetParent(canvasObject.transform, false);
            var combatText = combatObject.AddComponent<UnityEngine.UI.Text>();
            combatText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            combatText.fontSize = 18;
            combatText.color = Color.red;
            combatText.alignment = TextAnchor.MiddleCenter;
            var combatRect = combatText.rectTransform;
            combatRect.anchorMin = new Vector2(0.5f, 0.5f);
            combatRect.anchorMax = new Vector2(0.5f, 0.5f);
            combatRect.sizeDelta = new Vector2(500f, 50f);

            player.GetComponent<VerticalSlicePlayer>().Configure(npc.transform, prompt, dialogue, quest);
            var world = new GameObject("Vertical Slice World Systems");
            world.AddComponent<VerticalSliceWorld>().Configure(player.transform, quest, prompt, combatText);
        }
    }
}
