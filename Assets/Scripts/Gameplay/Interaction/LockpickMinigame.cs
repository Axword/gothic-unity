using System;
using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Attributes;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Improved keyboard left/right lockpick minigame.
    /// Shows current sequence and progress on screen.
    /// </summary>
    public class LockpickMinigame : BaseMonoBehaviour
    {
        public int difficulty = 3;
        public Action<bool> OnComplete;

        [Header("UI (optional)")]
        public Text instructionText;
        public Text progressText;

        private int _currentStep;
        private string _sequence = "LRRL";
        private bool _active;
        private Canvas _uiCanvas;

        public void StartMinigame(int diff, Action<bool> callback)
        {
            difficulty = Mathf.Clamp(diff, 2, 5);
            _sequence = GenerateSequence(difficulty);
            _currentStep = 0;
            _active = true;
            OnComplete = callback;

            CreateOrShowUI();

            Debug.Log($"[Lockpick] Minigra rozpoczęta. Sekwencja: {_sequence}");
            UpdateUI();
        }

        private string GenerateSequence(int len)
        {
            string s = "";
            for (int i = 0; i < len; i++)
                s += UnityEngine.Random.value > 0.5f ? "L" : "R";
            return s;
        }

        private void Update()
        {
            if (!_active) return;

            bool left = Input.GetKeyDown(KeyCode.LeftArrow);
            bool right = Input.GetKeyDown(KeyCode.RightArrow);

            if (!left && !right) return;

            char expected = _sequence[_currentStep];
            bool correct = (expected == 'L' && left) || (expected == 'R' && right);

            if (correct)
            {
                _currentStep++;
                Debug.Log($"[Lockpick] Dobrze! {_currentStep}/{_sequence.Length}");
                UpdateUI();

                if (_currentStep >= _sequence.Length)
                {
                    Finish(true);
                }
            }
            else
            {
                Debug.Log("[Lockpick] Błąd! Wytrych pęka...");
                Finish(false);
            }
        }

        private void UpdateUI()
        {
            if (progressText != null)
            {
                progressText.text = $"Postęp: {_currentStep} / {_sequence.Length}\nSekwencja: {_sequence.Substring(0, _currentStep)}<color=yellow>{_sequence[_currentStep]}</color>{_sequence.Substring(_currentStep + 1)}";
            }
            if (instructionText != null)
            {
                instructionText.text = "← Lewo    → Prawo\nNaciśnij strzałki w poprawnej kolejności";
            }
        }

        private void CreateOrShowUI()
        {
            if (_uiCanvas == null)
            {
                GameObject canvasGO = new GameObject("LockpickUI");
                _uiCanvas = canvasGO.AddComponent<Canvas>();
                _uiCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
                canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

                // Instruction
                GameObject instGO = new GameObject("Instruction");
                instGO.transform.SetParent(canvasGO.transform);
                instructionText = instGO.AddComponent<Text>();
                instructionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                instructionText.fontSize = 22;
                instructionText.color = Color.white;
                instructionText.alignment = TextAnchor.MiddleCenter;
                RectTransform rt = instGO.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 0.6f);
                rt.anchorMax = new Vector2(0.5f, 0.6f);
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(500, 80);

                // Progress
                GameObject progGO = new GameObject("Progress");
                progGO.transform.SetParent(canvasGO.transform);
                progressText = progGO.AddComponent<Text>();
                progressText.font = instructionText.font;
                progressText.fontSize = 28;
                progressText.color = Color.yellow;
                progressText.alignment = TextAnchor.MiddleCenter;
                RectTransform prt = progGO.GetComponent<RectTransform>();
                prt.anchorMin = new Vector2(0.5f, 0.45f);
                prt.anchorMax = new Vector2(0.5f, 0.45f);
                prt.anchoredPosition = Vector2.zero;
                prt.sizeDelta = new Vector2(600, 60);
            }

            _uiCanvas.gameObject.SetActive(true);
            UpdateUI();
        }

        private void Finish(bool success)
        {
            _active = false;

            if (_uiCanvas != null)
            {
                if (progressText) progressText.text = success ? "<color=green>SUKCES!</color>" : "<color=red>PORAŻKA!</color>";
                Destroy(_uiCanvas.gameObject, 1.2f);
                _uiCanvas = null;
            }

            OnComplete?.Invoke(success);
            OnComplete = null;
            gameObject.SetActive(false);
            Debug.Log($"[Lockpick] Wynik: {(success ? "SUKCES" : "PORAŻKA")}");
        }

        public void Cancel()
        {
            if (_uiCanvas) Destroy(_uiCanvas.gameObject);
            _active = false;
            OnComplete?.Invoke(false);
        }
    }
}