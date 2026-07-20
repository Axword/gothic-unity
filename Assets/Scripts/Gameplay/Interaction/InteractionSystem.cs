using System;
using UnityEngine;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Interaction
{
    /// <summary>
    /// Handles world interactions: talk, pick up, open, use.
    /// </summary>
    public class InteractionSystem : BaseMonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float interactionRange = 3f;
        [SerializeField] private LayerMask interactableLayers;

        private IInteractable _currentTarget;
        private Camera _cam;

        private void Start()
        {
            _cam = Camera.main;
            ComponentLocator.Register<IInteractionSystem>(new InteractionSystemInterface(this));
        }

        private void Update()
        {
            UpdateTarget();
            
            // Demo: press E to interact
            if (Input.GetKeyDown(KeyCode.E) && _currentTarget != null)
            {
                _currentTarget.Interact(gameObject);
            }
        }

        private void UpdateTarget()
        {
            if (_cam == null) _cam = Camera.main;
            if (_cam == null) return;

            Ray ray = _cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            if (Physics.Raycast(ray, out RaycastHit hit, interactionRange, interactableLayers))
            {
                var interactable = hit.collider.GetComponent<IInteractable>();
                if (interactable != null && interactable != _currentTarget)
                {
                    _currentTarget = interactable;
                    Debug.Log($"[Interaction] Looking at: {_currentTarget.GetInteractionLabel()}");
                }
            }
            else
            {
                _currentTarget = null;
            }
        }

        public string GetCurrentInteractionLabel()
        {
            return _currentTarget?.GetInteractionLabel() ?? "";
        }

        public void TryInteract()
        {
            if (_currentTarget != null)
            {
                _currentTarget.Interact(gameObject);
            }
        }
    }

    public interface IInteractable
    {
        string GetInteractionLabel();
        void Interact(GameObject interactor);
    }

    public interface IInteractionSystem
    {
        void TryInteract();
        string GetCurrentInteractionLabel();
    }

    public class InteractionSystemInterface : IInteractionSystem
    {
        private readonly InteractionSystem _sys;
        public InteractionSystemInterface(InteractionSystem sys) { _sys = sys; }
        public void TryInteract() => _sys.TryInteract();
        public string GetCurrentInteractionLabel() => _sys.GetCurrentInteractionLabel();
    }
}
