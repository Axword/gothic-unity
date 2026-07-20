using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Dialogues;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.UI.Dialogues
{
    /// <summary>
    /// Very basic dialogue UI using uGUI for vertical slice.
    /// </summary>
    public class DialogueUI : BaseMonoBehaviour
    {
        [Header("UI")]
        public GameObject dialoguePanel;
        public Text speakerText;
        public Text dialogueText;
        public Transform choicesContainer;
        public GameObject choiceButtonPrefab;
        private DialogueManager _dialogueManager;
        private List<GameObject> _choiceButtons = new List<GameObject>();
        private void Start()
        {
            _dialogueManager = ComponentLocator.Get<DialogueManager>();
            if (_dialogueManager != null)
            {
                _dialogueManager.OnDialogueStarted += ShowDialogue;
                _dialogueManager.OnNodeChanged += UpdateNode;
                _dialogueManager.OnDialogueEnded += HideDialogue;
            }
            if (dialoguePanel) dialoguePanel.SetActive(false);
        }
        private void ShowDialogue(DialogueTree tree, string npcId)
            if (dialoguePanel) dialoguePanel.SetActive(true);
            UpdateNode(_dialogueManager.CurrentNode);
        private void UpdateNode(DialogueNode node)
            if (node == null) return;
            if (speakerText) speakerText.text = node.speaker ?? "NPC";
            if (dialogueText) dialogueText.text = node.text ?? "(brak tekstu)";
            // Clear old choices
            foreach (var btn in _choiceButtons) Destroy(btn);
            _choiceButtons.Clear();
            var choices = _dialogueManager.GetAvailableChoices();
            for (int i = 0; i < choices.Count; i++)
                int idx = i;
                GameObject btnGO = Instantiate(choiceButtonPrefab, choicesContainer);
                btnGO.GetComponentInChildren<Text>().text = choices[i].text;
                btnGO.GetComponent<Button>().onClick.AddListener(() => 
                {
                    _dialogueManager.SelectChoice(idx);
                });
                _choiceButtons.Add(btnGO);
            // If no choices, add "Dalej" / "Zakończ"
            if (choices.Count == 0)
                btnGO.GetComponentInChildren<Text>().text = "Dalej / Zakończ";
                btnGO.GetComponent<Button>().onClick.AddListener(() => _dialogueManager.EndDialogue());
        private void HideDialogue()
        private void OnDestroy()
                _dialogueManager.OnDialogueStarted -= ShowDialogue;
                _dialogueManager.OnNodeChanged -= UpdateNode;
                _dialogueManager.OnDialogueEnded -= HideDialogue;
    }
}
