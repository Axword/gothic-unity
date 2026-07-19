using UnityEngine;

namespace ZelaznaDroga
{
    /// <summary>
    /// Temporary procedural animation layer for greybox assets. It keeps the slice
    /// readable until production FBX models and Animator Controllers are imported.
    /// </summary>
    public sealed class ProceduralModelAnimator : MonoBehaviour
    {
        public enum Motion { Idle, Walk, Combat }
        [SerializeField] private Motion motion = Motion.Idle;
        private Vector3 basePosition;
        private float phase;

        public Motion CurrentMotion { get => motion; set => motion = value; }
        private void Awake() => basePosition = transform.localPosition;

        private void Update()
        {
            phase += Time.deltaTime * (motion == Motion.Walk ? 8f : 3f);
            float amplitude = motion == Motion.Combat ? 0.06f : motion == Motion.Walk ? 0.09f : 0.025f;
            transform.localPosition = basePosition + Vector3.up * (Mathf.Sin(phase) * amplitude);
            if (motion == Motion.Combat) transform.localRotation = Quaternion.Euler(0f, Mathf.Sin(phase * 2f) * 6f, 0f);
        }
    }
}
