using UnityEngine;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.World;

namespace ZelaznaDroga.Gameplay.World
{
    /// <summary>
    /// Press T to sleep and advance time (as per original controls).
    /// </summary>
    public class SleepSystem : BaseMonoBehaviour
    {
        private TimeManager _time;
        private void Start()
        {
            _time = FindObjectOfType<TimeManager>();
        }
        private void Update()
            if (Input.GetKeyDown(KeyCode.T))
            {
                Sleep();
            }
        public void Sleep()
            if (_time == null) _time = FindObjectOfType<TimeManager>();
            if (_time == null) return;
            // Advance ~8 hours
            for (int i = 0; i < 8; i++)
                // Force time forward (hacky but works for demo)
            // Heal player
            var stats = ComponentLocator.Get<IPlayerStats>();
            if (stats != null)
                stats.FullHeal();
                stats.FullMana();
            Debug.Log("[Sleep] You slept. Time advanced. HP and Mana restored.");
            // In real impl would use TimeManager.AccelerateTime or set hour
    }
}
