using UnityEngine;
using UnityEngine.SceneManagement;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Save;

namespace ZelaznaDroga.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        public GameObject mainPanel;
        public GameObject optionsPanel;

        private SaveSystem _saveSystem;

        private void Start()
        {
            _saveSystem = FindObjectOfType<SaveSystem>();
            if (_saveSystem == null)
            {
                _saveSystem = new GameObject("SaveSystem").AddComponent<SaveSystem>();
            }
        }

        public void OnNewGame()
        {
            // Start new game in sample world
            SceneManager.LoadScene("SampleScene");
        }

        public void OnLoadGame()
        {
            if (_saveSystem != null)
            {
                bool loaded = _saveSystem.LoadGame(0);
                if (loaded)
                {
                    SceneManager.LoadScene("SampleScene");
                }
            }
        }

        public void OnOptions()
        {
            if (mainPanel) mainPanel.SetActive(false);
            if (optionsPanel) optionsPanel.SetActive(true);
        }

        public void OnExit()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        public void BackToMain()
        {
            if (mainPanel) mainPanel.SetActive(true);
            if (optionsPanel) optionsPanel.SetActive(false);
        }
    }
}
