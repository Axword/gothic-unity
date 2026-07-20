#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;
using Newtonsoft.Json.Linq;

public static class DataValidator
{
    [MenuItem("Żelazna Droga/Validate All JSON Data")]
    public static void ValidateAll()
    {
        string basePath = Application.streamingAssetsPath + "/Data/Json";
        int errors = 0;

        string[] files = Directory.GetFiles(basePath, "*.json", SearchOption.AllDirectories);
        foreach (var file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                JObject.Parse(json);
                Debug.Log($"[Validator] OK: {Path.GetFileName(file)}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Validator] ERROR in {Path.GetFileName(file)}: {e.Message}");
                errors++;
            }
        }

        if (errors == 0)
            Debug.Log("=== All JSON files are valid! ===");
        else
            Debug.LogError($"=== {errors} files have errors! ===");
    }
}
#endif
