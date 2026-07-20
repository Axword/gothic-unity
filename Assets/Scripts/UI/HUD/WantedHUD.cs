using UnityEngine;
using UnityEngine.UI;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.World;

namespace ZelaznaDroga.UI.HUD
{
    /// <summary>
    /// Shows current wanted level and time of day.
    /// </summary>
    public class WantedHUD : BaseMonoBehaviour
    {
        public Text wantedText;
        public Text timeText;

        private CrimeSystem _crime;
        private TimeManager _time;

        private void Start()
        {
            _crime = ComponentLocator.Get<CrimeSystem>();
            _time = FindObjectOfType<TimeManager>();

            if (wantedText == null)
            {
                // Create simple wanted text if not assigned
                GameObject go = new GameObject("WantedText");
                go.transform.SetParent(transform);
                wantedText = go.AddComponent<Text>();
                wantedText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                wantedText.fontSize = 18;
                wantedText.color = Color.red;
                RectTransform rt = go.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(1, 1);
                rt.anchorMax = new Vector2(1, 1);
                rt.anchoredPosition = new Vector2(-120, -30);
            }
        }

        private void Update()
        {
            if (wantedText)
            {
                if (_crime != null && _crime.WantedLevel > 0)
                {
                    wantedText.text = $"POSZUKIWANY: {_crime.WantedLevel} ★";
                    wantedText.color = Color.Lerp(Color.yellow, Color.red, _crime.WantedLevel / 5f);
                }
                else
                {
                    wantedText.text = "";
                }
            }

            if (timeText && _time)
            {
                int h = _time.CurrentHour;
                timeText.text = $"{h:00}:00";
            }
        }
    }
}
