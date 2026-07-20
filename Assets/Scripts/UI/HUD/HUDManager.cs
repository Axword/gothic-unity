using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Progression;
using ZelaznaDroga.Gameplay.Inventory;
using ZelaznaDroga.Gameplay.Interaction;

namespace ZelaznaDroga.UI.HUD
{
    /// <summary>
    /// Simple uGUI HUD for health, mana, interaction prompt.
    /// </summary>
    public class HUDManager : BaseMonoBehaviour
    {
        [Header("UI References")]
        public Slider healthSlider;
        public Slider manaSlider;
        public Text interactionText;
        public Text goldText;
        public Text levelText;

        private IPlayerStats _stats;
        private IInventorySystem _inventory;
        private IInteractionSystem _interaction;

        private void Start()
        {
            _stats = ComponentLocator.Get<IPlayerStats>();
            _inventory = ComponentLocator.Get<IInventorySystem>();
            _interaction = ComponentLocator.Get<IInteractionSystem>();

            if (_stats != null)
            {
                _stats.OnHealthChanged += UpdateHealth;
                _stats.OnManaChanged += UpdateMana;
            }

            if (_inventory != null)
            {
                _inventory.OnGoldChanged += UpdateGold;
            }

            UpdateAll();
        }

        private void Update()
        {
            if (_interaction != null && interactionText != null)
            {
                string label = _interaction.GetCurrentInteractionLabel();
                interactionText.text = string.IsNullOrEmpty(label) ? "" : $"[E] {label}";
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                // TODO: open inventory UI
                Debug.Log("[HUD] Inventory toggle (not implemented in slice)");
            }
        }

        private void UpdateHealth(int current, int max)
        {
            if (healthSlider != null)
            {
                healthSlider.value = (float)current / max;
            }
        }

        private void UpdateMana(int current, int max)
        {
            if (manaSlider != null)
            {
                manaSlider.value = (float)current / max;
            }
        }

        private void UpdateGold(int gold)
        {
            if (goldText != null)
            {
                goldText.text = $"Złoto: {gold}";
            }
        }

        private void UpdateAll()
        {
            if (_stats != null)
            {
                UpdateHealth(_stats.CurrentHealth, _stats.MaxHealth);
                UpdateMana(_stats.CurrentMana, _stats.MaxMana);
                if (levelText) levelText.text = $"Poziom {_stats.Level}";
            }
            if (_inventory != null && goldText)
            {
                goldText.text = $"Złoto: {_inventory.Gold}";
            }
        }

        private void OnDestroy()
        {
            if (_stats != null)
            {
                _stats.OnHealthChanged -= UpdateHealth;
                _stats.OnManaChanged -= UpdateMana;
            }
            if (_inventory != null)
            {
                _inventory.OnGoldChanged -= UpdateGold;
            }
        }
    }
}
