using UnityEngine;
using UnityEngine.UI;

namespace ZelaznaDroga
{
    /// <summary>Small input-driven controller used by the greybox scene.</summary>
    public sealed class VerticalSlicePlayer : MonoBehaviour
    {
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintMultiplier = 1.7f;
        [SerializeField] private float interactionDistance = 3f;
        private Text prompt;
        private Text dialogue;
        private Transform npc;
        private VerticalSliceQuest quest;

        public void Configure(Transform npcTransform, Text promptText, Text dialogueText, VerticalSliceQuest questSystem)
        {
            npc = npcTransform;
            prompt = promptText;
            dialogue = dialogueText;
            quest = questSystem;
            SetDialogue(string.Empty);
        }

        private void Update()
        {
            Move();
            bool nearNpc = npc != null && Vector3.Distance(transform.position, npc.position) <= interactionDistance;
            if (prompt != null) prompt.text = nearNpc ? "E — Rozmowa z Aldoną" : "WASD — Ruch   Shift — Bieg";
            if (nearNpc && Input.GetKeyDown(KeyCode.E))
            {
                quest?.StartQuest();
                SetDialogue("Aldona: Witaj, przybyszu. Droga przed tobą nie będzie łatwa.\n\nAldona: Odnajdź ślad przy starym młynie.");
            }
        }

        private void Move()
        {
            var input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
            if (input.sqrMagnitude > 1f) input.Normalize();
            float speed = walkSpeed * (Input.GetKey(KeyCode.LeftShift) ? sprintMultiplier : 1f);
            transform.position += input * speed * Time.deltaTime;
        }

        private void SetDialogue(string message)
        {
            if (dialogue != null) dialogue.text = message;
        }
    }
}
