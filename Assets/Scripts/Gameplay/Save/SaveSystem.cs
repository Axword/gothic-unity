using System;
using System.IO;
using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Data.Json;
using ZelaznaDroga.Data.Schema;

namespace ZelaznaDroga.Gameplay.Save
{
    /// <summary>
    /// Handles saving and loading game state to JSON.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        private SaveDataManager _saveManager;
        private const int CURRENT_SCHEMA_VERSION = 1;

        public event Action OnGameSaved;
        public event Action OnGameLoaded;
        public event Action<string> OnSaveError;

        private void Awake()
        {
            _saveManager = new SaveDataManager();
            ComponentLocator.Register<ISaveSystem>(new SaveSystemInterface(this));
        }

        public bool SaveGame(int slot = 0)
        {
            try
            {
                var saveData = new SaveGame
                {
                    schemaVersion = "1.0.0",
                    saveVersion = "0.5.0",
                    timestamp = DateTime.UtcNow.ToString("o"),
                    playtimeSeconds = 0 // TODO track time
                };

                // Player
                var player = ComponentLocator.Get<IPlayerController>();
                var stats = ComponentLocator.Get<IPlayerStats>();
                var inventory = ComponentLocator.Get<IInventorySystem>();
                var quests = ComponentLocator.Get<IQuestManager>();

                if (stats != null)
                {
                    saveData.player = stats.GetSaveData() ?? new PlayerSaveData();
                }
                if (player != null)
                {
                    saveData.player.position = new Vector3Wrapper(player.Position);
                    saveData.player.currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                }

                if (inventory != null)
                {
                    saveData.equipment = inventory.GetEquipmentSaveData();
                    saveData.inventory = inventory.GetInventorySaveData();
                }

                if (quests != null)
                {
                    // Note: QuestManager needs to expose more save data
                    saveData.completedQuests = new System.Collections.Generic.List<string>(quests.CompletedQuests);
                }

                bool success = _saveManager.Save(slot, saveData);
                
                if (success)
                {
                    OnGameSaved?.Invoke();
                    Debug.Log($"[SaveSystem] Game saved to slot {slot}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Save failed: {ex}");
                OnSaveError?.Invoke(ex.Message);
            }
            return false;
        }

        public bool LoadGame(int slot = 0)
        {
            try
            {
                var saveData = _saveManager.Load<SaveGame>(slot);
                if (saveData == null)
                {
                    Debug.LogWarning($"[SaveSystem] No save in slot {slot}");
                    return false;
                }

                // Apply to player
                var stats = ComponentLocator.Get<IPlayerStats>();
                var inventory = ComponentLocator.Get<IInventorySystem>();
                var player = ComponentLocator.Get<IPlayerController>();
                var quests = ComponentLocator.Get<IQuestManager>();

                if (stats != null)
                {
                    stats.LoadSaveData(saveData.player);
                }

                if (inventory != null)
                {
                    inventory.LoadInventorySaveData(saveData.inventory);
                    // equipment load would require items loaded
                }

                if (player != null && saveData.player.position != null)
                {
                    player.Teleport(saveData.player.position.ToVector3());
                }

                if (quests != null)
                {
                    quests.LoadSaveData(saveData.activeQuests, saveData.completedQuests, saveData.failedQuests);
                }

                OnGameLoaded?.Invoke();
                Debug.Log($"[SaveSystem] Game loaded from slot {slot}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveSystem] Load failed: {ex}");
                OnSaveError?.Invoke(ex.Message);
                return false;
            }
        }

        public void DeleteSave(int slot)
        {
            _saveManager.Delete(slot);
        }

        public System.Collections.Generic.List<SaveSlotInfo> GetSaveSlots()
        {
            return _saveManager.GetSaveSlots();
        }
    }

    public interface ISaveSystem
    {
        bool SaveGame(int slot = 0);
        bool LoadGame(int slot = 0);
    }

    public class SaveSystemInterface : ISaveSystem
    {
        private readonly SaveSystem _system;
        public SaveSystemInterface(SaveSystem sys) { _system = sys; }
        public bool SaveGame(int slot = 0) => _system.SaveGame(slot);
        public bool LoadGame(int slot = 0) => _system.LoadGame(slot);
    }
}
