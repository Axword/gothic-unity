using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Attach to chests/doors. Starts the improved lockpick minigame.
    /// </summary>
    public class LockpickTrigger : MonoBehaviour
    {
        public int difficulty = 3;
        public bool requireLockpick = true;

        private LockpickMinigame _minigame;

        private void Start()
        {
            _minigame = GetComponent<LockpickMinigame>();
            if (_minigame == null)
                _minigame = gameObject.AddComponent<LockpickMinigame>();
        }

        public void TryPickLock(System.Action<bool> resultCallback)
        {
            var inv = ComponentLocator.Get<IInventorySystem>();
            if (requireLockpick && inv != null)
            {
                if (!inv.HasItem(GameConstants.ITEM_LOCKPICK))
                {
                    Debug.Log("[Lockpick] Brak wytrychów!");
                    resultCallback?.Invoke(false);
                    return;
                }
                inv.RemoveItem(GameConstants.ITEM_LOCKPICK, 1);
            }

            _minigame.StartMinigame(difficulty, resultCallback);
        }
    }
}