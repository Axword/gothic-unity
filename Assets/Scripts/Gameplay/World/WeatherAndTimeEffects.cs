using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.World
{
    /// <summary>
    /// Simple day/night + basic weather lighting.
    /// Changes ambient light, fog, and sun intensity.
    /// </summary>
    public class WeatherAndTimeEffects : BaseMonoBehaviour
    {
        [Header("References")]
        public Light sunLight;
        public Material skyboxMaterial; // optional

        [Header("Day/Night")]
        [Range(0, 24)] public float currentHour = 12f;

        private TimeManager _timeManager;

        private void Start()
        {
            _timeManager = FindObjectOfType<TimeManager>();
            if (sunLight == null)
            {
                sunLight = FindObjectOfType<Light>();
            }

            if (_timeManager != null)
            {
                _timeManager.OnTimeChanged += UpdateLighting;
            }

            UpdateLighting(12f);
        }

        private void UpdateLighting(float hour)
        {
            currentHour = hour;

            // Sun angle
            float sunAngle = (hour / 24f) * 360f - 90f;
            if (sunLight)
            {
                sunLight.transform.rotation = Quaternion.Euler(sunAngle, 45f, 0);
                float intensity = Mathf.Clamp01(Mathf.Sin((hour - 6) / 12f * Mathf.PI));
                sunLight.intensity = Mathf.Lerp(0.1f, 1.1f, intensity);
            }

            // Ambient / fog
            float nightFactor = Mathf.Clamp01((hour < 6 || hour > 20) ? 1f : (hour < 8 || hour > 18 ? 0.6f : 0f));
            RenderSettings.ambientLight = Color.Lerp(new Color(0.6f, 0.65f, 0.75f), new Color(0.15f, 0.15f, 0.25f), nightFactor);
            RenderSettings.fogColor = Color.Lerp(new Color(0.7f, 0.75f, 0.85f), new Color(0.1f, 0.1f, 0.2f), nightFactor);
            RenderSettings.fogDensity = Mathf.Lerp(0.005f, 0.018f, nightFactor);

            // Optional: make some plants glow at night (demo)
            if (hour > 20 || hour < 5)
            {
                foreach (var plant in FindObjectsOfType<Interaction.Plant>())
                {
                    if (plant.GetComponent<Renderer>())
                        plant.GetComponent<Renderer>().material.color = Color.Lerp(Color.green, new Color(0.3f, 0.6f, 0.3f), 0.6f);
                }
            }
        }

        private void OnDestroy()
        {
            if (_timeManager != null)
                _timeManager.OnTimeChanged -= UpdateLighting;
        }
    }
}
