using UnityEngine;
using UnityEngine.SceneManagement;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Save;

namespace ZelaznaDroga.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        public GameObject pausePanel;
        private bool isPaused = false;
        private SaveSystem _save;

        private void Start()
        {
            _save = FindObjectOfType<SaveSystem>();
            if (pausePanel) pausePanel.SetActive(false);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            if (pausePanel) pausePanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
        }

        public void Resume() => TogglePause();

        public void SaveGame()
        {
            if (_save != null) _save.SaveGame(0);
            else Debug.Log("SaveSystem not found");
        }

        public void LoadGame()
        {
            if (_save != null) _save.LoadGame(0);
            TogglePause();
        }

        public void ExitToMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SampleScene"); // or main menu if exists
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
