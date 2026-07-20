using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZelaznaDroga.Data.Json
{
    /// <summary>
    /// Loads and validates JSON data files from StreamingAssets.
    /// </summary>
    public class JsonDataLoader
    {
        private readonly string _basePath;
        private readonly bool _validate;
        private readonly List<string> _loadedFiles = new List<string>();
        private readonly Dictionary<string, JObject> _cache = new Dictionary<string, JObject>();

        public JsonDataLoader(string basePath = null, bool validate = true)
        {
            _basePath = basePath ?? Application.streamingAssetsPath + "/Data/Json";
            _validate = validate;
        }

        /// <summary>
        /// Loads a JSON file and returns it as a JObject.
        /// </summary>
        public JObject Load(string filename)
        {
            string fullPath = Path.Combine(_basePath, filename);
            
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[JsonDataLoader] File not found: {fullPath}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                JObject obj = JObject.Parse(json);
                
                _loadedFiles.Add(filename);
                _cache[filename] = obj;

                if (_validate)
                {
                    ValidateFile(filename, obj);
                }

                return obj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonDataLoader] Error loading {filename}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads a JSON file and deserializes it to a strongly-typed object.
        /// </summary>
        public T Load<T>(string filename) where T : class
        {
            string fullPath = Path.Combine(_basePath, filename);
            
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"[JsonDataLoader] File not found: {fullPath}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(fullPath);
                T obj = JsonConvert.DeserializeObject<T>(json);
                
                _loadedFiles.Add(filename);
                return obj;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonDataLoader] Error deserializing {filename}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Loads all JSON files from a specific category folder.
        /// </summary>
        public List<T> LoadAll<T>(string folder) where T : class
        {
            string fullFolder = Path.Combine(_basePath, folder);
            List<T> results = new List<T>();

            if (!Directory.Exists(fullFolder))
            {
                Debug.LogWarning($"[JsonDataLoader] Folder not found: {fullFolder}");
                return results;
            }

            string[] files = Directory.GetFiles(fullFolder, "*.json");
            foreach (string file in files)
            {
                string filename = Path.GetFileName(file);
                T obj = Load<T>(Path.Combine(folder, filename));
                if (obj != null)
                {
                    results.Add(obj);
                }
            }

            return results;
        }

        /// <summary>
        /// Saves an object to a JSON file.
        /// </summary>
        public bool Save(string filename, object obj, bool prettyPrint = true)
        {
            string fullPath = Path.Combine(_basePath, filename);

            try
            {
                string json;
                if (prettyPrint)
                {
                    json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                }
                else
                {
                    json = JsonConvert.SerializeObject(obj);
                }

                File.WriteAllText(fullPath, json);
                Debug.Log($"[JsonDataLoader] Saved {filename}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[JsonDataLoader] Error saving {filename}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets a cached JObject if available.
        /// </summary>
        public JObject GetCached(string filename)
        {
            if (_cache.TryGetValue(filename, out JObject obj))
            {
                return obj;
            }
            return Load(filename);
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Gets list of all loaded files.
        /// </summary>
        public IReadOnlyList<string> GetLoadedFiles()
        {
            return _loadedFiles;
        }

        /// <summary>
        /// Validates a loaded file against its schema.
        /// </summary>
        private void ValidateFile(string filename, JObject obj)
        {
            // Basic validation - in production, this would use JSON Schema
            if (obj == null)
            {
                Debug.LogError($"[JsonDataLoader] Validation failed: {filename} is null");
                return;
            }

            // Check for required root properties based on file type
            if (filename.Contains("item"))
            {
                ValidateItems(obj, filename);
            }
            else if (filename.Contains("npc"))
            {
                ValidateNPCs(obj, filename);
            }
            else if (filename.Contains("quest"))
            {
                ValidateQuests(obj, filename);
            }
            else if (filename.Contains("dialogue"))
            {
                ValidateDialogues(obj, filename);
            }
        }

        private void ValidateItems(JObject obj, string filename)
        {
            // Check if has items array or object with items
            if (obj["items"] == null && obj["items"] == null)
            {
                Debug.LogWarning($"[JsonDataLoader] {filename} may not have 'items' root");
            }
        }

        private void ValidateNPCs(JObject obj, string filename)
        {
            if (obj["npcs"] == null)
            {
                Debug.LogWarning($"[JsonDataLoader] {filename} may not have 'npcs' root");
            }
        }

        private void ValidateQuests(JObject obj, string filename)
        {
            if (obj["quests"] == null)
            {
                Debug.LogWarning($"[JsonDataLoader] {filename} may not have 'quests' root");
            }
        }

        private void ValidateDialogues(JObject obj, string filename)
        {
            // Dialogue files can have various structures
        }
    }

    /// <summary>
    /// Handles loading and saving of persistent game data.
    /// </summary>
    public class SaveDataManager
    {
        private readonly string _saveFolder;

        public SaveDataManager(string saveFolder = null)
        {
            _saveFolder = saveFolder ?? Application.persistentDataPath + "/" + GameConstants.SAVE_FOLDER;
            
            if (!Directory.Exists(_saveFolder))
            {
                Directory.CreateDirectory(_saveFolder);
            }
        }

        /// <summary>
        /// Gets all save slot info without loading full save data.
        /// </summary>
        public List<SaveSlotInfo> GetSaveSlots()
        {
            List<SaveSlotInfo> slots = new List<SaveSlotInfo>();
            
            for (int i = 0; i < GameConstants.MAX_SAVE_SLOTS; i++)
            {
                string path = GetSlotPath(i);
                SaveSlotInfo info = new SaveSlotInfo
                {
                    SlotIndex = i,
                    Exists = File.Exists(path),
                    FilePath = path
                };

                if (info.Exists)
                {
                    try
                    {
                        string json = File.ReadAllText(path);
                        JObject data = JObject.Parse(json);
                        info.SaveVersion = data["saveVersion"]?.ToString();
                        info.Timestamp = data["timestamp"]?.ToString();
                        info.PlaytimeSeconds = data["playtimeSeconds"]?.Value<int>() ?? 0;
                        info.PlayerLevel = data["player"]?["level"]?.Value<int>() ?? 1;
                        info.SceneName = data["player"]?["currentScene"]?.ToString();
                    }
                    catch
                    {
                        info.Exists = false; // Mark as corrupted
                    }
                }

                slots.Add(info);
            }

            return slots;
        }

        /// <summary>
        /// Saves game data to a slot.
        /// </summary>
        public bool Save(int slotIndex, object saveData)
        {
            if (slotIndex < 0 || slotIndex >= GameConstants.MAX_SAVE_SLOTS)
            {
                Debug.LogError($"[SaveDataManager] Invalid slot index: {slotIndex}");
                return false;
            }

            string path = GetSlotPath(slotIndex);
            string backupPath = path + ".backup";

            try
            {
                // Create backup of existing save
                if (File.Exists(path))
                {
                    File.Copy(path, backupPath, true);
                }

                string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
                File.WriteAllText(path, json);

                // Remove backup on success
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }

                Debug.Log($"[SaveDataManager] Saved to slot {slotIndex}");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveDataManager] Error saving to slot {slotIndex}: {ex.Message}");
                
                // Try to restore backup
                if (File.Exists(backupPath))
                {
                    File.Copy(backupPath, path, true);
                    Debug.Log($"[SaveDataManager] Restored backup for slot {slotIndex}");
                }
                
                return false;
            }
        }

        /// <summary>
        /// Loads game data from a slot.
        /// </summary>
        public T Load<T>(int slotIndex) where T : class
        {
            if (slotIndex < 0 || slotIndex >= GameConstants.MAX_SAVE_SLOTS)
            {
                Debug.LogError($"[SaveDataManager] Invalid slot index: {slotIndex}");
                return null;
            }

            string path = GetSlotPath(slotIndex);

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[SaveDataManager] No save found in slot {slotIndex}");
                return null;
            }

            try
            {
                string json = File.ReadAllText(path);
                T data = JsonConvert.DeserializeObject<T>(json);
                Debug.Log($"[SaveDataManager] Loaded from slot {slotIndex}");
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[SaveDataManager] Error loading from slot {slotIndex}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Deletes a save slot.
        /// </summary>
        public bool Delete(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= GameConstants.MAX_SAVE_SLOTS)
            {
                return false;
            }

            string path = GetSlotPath(slotIndex);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        private string GetSlotPath(int slotIndex)
        {
            return Path.Combine(_saveFolder, $"save_{slotIndex}.json");
        }
    }

    /// <summary>
    /// Information about a save slot.
    /// </summary>
    public class SaveSlotInfo
    {
        public int SlotIndex;
        public bool Exists;
        public string FilePath;
        public string SaveVersion;
        public string Timestamp;
        public int PlaytimeSeconds;
        public int PlayerLevel;
        public string SceneName;

        public string DisplayName => Exists 
            ? $"Slot {SlotIndex + 1} - Level {PlayerLevel}" 
            : $"Slot {SlotIndex + 1} (Empty)";

        public string FormattedPlaytime
        {
            get
            {
                TimeSpan time = TimeSpan.FromSeconds(PlaytimeSeconds);
                return $"{time.Hours:D2}:{time.Minutes:D2}:{time.Seconds:D2}";
            }
        }

        public string FormattedTimestamp => Exists && !string.IsNullOrEmpty(Timestamp)
            ? DateTime.Parse(Timestamp).ToString("yyyy-MM-dd HH:mm")
            : "N/A";
    }
}
