using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Quests;

namespace ZelaznaDroga.UI.Menus
{
    /// <summary>
    /// Quest journal with Active / Completed tabs.
    /// </summary>
    public class QuestJournalUI : BaseMonoBehaviour
    {
        [Header("UI")]
        public GameObject journalPanel;
        public Transform questListContent;
        public GameObject questButtonPrefab;
        public Text questTitle;
        public Text questDesc;
        public Text questObjectives;

        public Button activeTab;
        public Button completedTab;

        private IQuestManager _quests;
        private string _selectedQuestId;
        private bool _showActive = true;

        private void Start()
        {
            _quests = ComponentLocator.Get<IQuestManager>();
            if (activeTab) activeTab.onClick.AddListener(() => { _showActive = true; Refresh(); });
            if (completedTab) completedTab.onClick.AddListener(() => { _showActive = false; Refresh(); });

            if (journalPanel) journalPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (journalPanel) journalPanel.SetActive(!journalPanel.activeSelf);
            if (journalPanel.activeSelf) Refresh();
        }

        public void Refresh()
        {
            if (_quests == null || questListContent == null) return;

            foreach (Transform t in questListContent) Destroy(t.gameObject);

            var active = _quests.ActiveQuests;
            var completed = _quests.CompletedQuests;

            if (_showActive)
            {
                foreach (var kvp in active)
                {
                    CreateQuestButton(kvp.Key, kvp.Value.Data?.titleKey ?? kvp.Key, true);
                }
            }
            else
            {
                foreach (string qid in completed)
                {
                    CreateQuestButton(qid, qid, false);
                }
            }
        }

        private void CreateQuestButton(string questId, string title, bool isActive)
        {
            GameObject btn = Instantiate(questButtonPrefab, questListContent);
            btn.GetComponentInChildren<Text>().text = title;
            btn.GetComponent<Button>().onClick.AddListener(() => ShowQuestDetails(questId));
        }

        private void ShowQuestDetails(string questId)
        {
            _selectedQuestId = questId;
            var qm = _quests as QuestManager; // cast for full data
            if (qm == null) return;

            var data = qm.GetQuest(questId);
            if (data == null) return;

            if (questTitle) questTitle.text = data.titleKey;
            if (questDesc) questDesc.text = data.descriptionKey;

            string objText = "";
            var objectives = qm.GetCurrentStageObjectives(questId);
            foreach (var (obj, prog) in objectives)
            {
                objText += $"- {obj.description} ({prog}/{obj.count})\n";
            }
            if (questObjectives) questObjectives.text = objText;
        }
    }
}
