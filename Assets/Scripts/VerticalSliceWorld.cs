using UnityEngine;
using UnityEngine.UI;

namespace ZelaznaDroga
{
    public sealed class VerticalSliceWorld : MonoBehaviour
    {
        private VerticalSliceQuest quest;
        private Transform player;
        private Text prompt;
        private Text combatText;
        private GameObject enemy;
        private float enemyHealth = 100f;
        private float nextAttack;

        public void Configure(Transform playerTransform, VerticalSliceQuest questSystem, Text promptText, Text combatOutput)
        {
            player = playerTransform;
            quest = questSystem;
            prompt = promptText;
            combatText = combatOutput;
            CreateMill();
            CreateEnemy();
        }

        private void Update()
        {
            if (player == null) return;
            HandleMillInteraction();
            HandleCombat();
        }

        private void HandleMillInteraction()
        {
            var mill = GameObject.Find("Old Mill (Objective)");
            if (mill == null) return;
            float distance = Vector3.Distance(player.position, mill.transform.position);
            if (distance <= 3f)
            {
                if (prompt != null) prompt.text = "E — Zbadaj stary młyn";
                if (Input.GetKeyDown(KeyCode.E)) quest.CompleteObjective();
            }
        }

        private void HandleCombat()
        {
            if (enemy == null || !enemy.activeSelf) return;
            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance <= 3.5f && Input.GetMouseButtonDown(0) && Time.time >= nextAttack)
            {
                nextAttack = Time.time + 0.45f;
                enemyHealth -= 25f;
                if (combatText != null) combatText.text = $"Trafienie! Wilk: {Mathf.Max(0, enemyHealth):0} HP";
                if (enemyHealth <= 0f)
                {
                    enemy.SetActive(false);
                    if (combatText != null) combatText.text = "Wilk pokonany.";
                }
            }
        }

        private void CreateMill()
        {
            var mill = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mill.name = "Old Mill (Objective)";
            mill.transform.position = new Vector3(7f, 1.5f, 5f);
            mill.transform.localScale = new Vector3(3f, 3f, 3f);
            mill.GetComponent<Renderer>().material.color = new Color(0.35f, 0.2f, 0.1f);
        }

        private void CreateEnemy()
        {
            enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            enemy.name = "Wolf (Combat Target)";
            enemy.transform.position = new Vector3(4f, 0.8f, -1f);
            enemy.transform.localScale = Vector3.one * 1.5f;
            enemy.GetComponent<Renderer>().material.color = new Color(0.3f, 0.3f, 0.32f);
            ProceduralModelFactory.BuildWolf(enemy);
            enemy.AddComponent<ProceduralModelAnimator>();
        }
    }
}
