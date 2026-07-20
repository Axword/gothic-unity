using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Progression;

namespace ZelaznaDroga.UI.Menus
{
    /// <summary>
    /// Character sheet: stats, level, learning points, spend buttons.
    /// </summary>
    public class CharacterUI : BaseMonoBehaviour
    {
        [Header("UI")]
        public GameObject characterPanel;
        public Text levelText;
        public Text xpText;
        public Text lpText;

        public Text strText;
        public Text dexText;
        public Text manaText;
        public Text hpText;

        public Button strPlus;
        public Button dexPlus;
        public Button manaPlus;

        private IPlayerStats _stats;

        private void Start()
        {
            _stats = ComponentLocator.Get<IPlayerStats>();
            if (strPlus) strPlus.onClick.AddListener(() => SpendLP("strength"));
            if (dexPlus) dexPlus.onClick.AddListener(() => SpendLP("dexterity"));
            if (manaPlus) manaPlus.onClick.AddListener(() => SpendLP("mana"));

            if (characterPanel) characterPanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                Toggle();
            }
            if (characterPanel && characterPanel.activeSelf)
            {
                UpdateStats();
            }
        }

        public void Toggle()
        {
            if (characterPanel) characterPanel.SetActive(!characterPanel.activeSelf);
        }

        private void UpdateStats()
        {
            if (_stats == null) return;

            if (levelText) levelText.text = $"Poziom: {_stats.Level}";
            if (xpText) xpText.text = $"XP: {_stats.CurrentXp} / {_stats.XpToNextLevel}";
            if (lpText) lpText.text = $"Punkty Nauki: {_stats.LearningPoints}";

            if (strText) strText.text = $"Siła: {_stats.Strength}";
            if (dexText) dexText.text = $"Zręczność: {_stats.Dexterity}";
            if (manaText) manaText.text = $"Mana: {_stats.ManaBase}";
            if (hpText) hpText.text = $"HP: {_stats.CurrentHealth} / {_stats.MaxHealth}";
        }

        private void SpendLP(string stat)
        {
            var statsComp = _stats as PlayerStats;
            if (statsComp != null)
            {
                statsComp.SpendLearningPoints(stat, 1);
                UpdateStats();
            }
        }
    }
}
