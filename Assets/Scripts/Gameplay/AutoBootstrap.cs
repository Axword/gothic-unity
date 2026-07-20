using UnityEngine;
using UnityEngine.SceneManagement;

namespace ZelaznaDroga.Gameplay
{
    /// <summary>
    /// If no bootstrap exists in scene, create minimal systems on play.
    /// Attach to any object or let it be auto-created.
    /// </summary>
    public class AutoBootstrap : MonoBehaviour
    {
        private static bool _initialized = false;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (_initialized) return;
            _initialized = true;

            // Only run in play mode for SampleScene if no bootstrap
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "SampleScene") return;

            if (FindObjectOfType<GameBootstrap>() != null) return;

            Debug.Log("[AutoBootstrap] No GameBootstrap found — creating minimal vertical slice setup...");

            // Create bootstrap
            var go = new GameObject("AutoGameBootstrap");
            go.AddComponent<GameBootstrap>();
        }
    }
}
