using System;
using UnityEngine;
using UnityEngine.InputSystem;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;

namespace ZelaznaDroga.Gameplay.Player
{
    /// <summary>
    /// Main player controller handling movement, camera, and input.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : BaseMonoBehaviour
    {
        #region Components

        [Header("Components")]
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private Camera _playerCamera;
        [SerializeField] private Transform _cameraTarget;

        #endregion

        #region Movement Settings

        [Header("Movement")]
        [SerializeField] private float _moveSpeed = GameConstants.BASE_MOVE_SPEED;
        [SerializeField] private float _sprintMultiplier = GameConstants.SPRINT_MULTIPLIER;
        [SerializeField] private float _jumpForce = GameConstants.JUMP_FORCE;
        [SerializeField] private float _gravity = GameConstants.GRAVITY;

        [Header("Camera")]
        [SerializeField] private float _mouseSensitivity = GameConstants.MOUSE_SENSITIVITY;
        [SerializeField] private float _cameraDistance = GameConstants.CAMERA_DISTANCE;
        [SerializeField] private float _cameraHeight = GameConstants.CAMERA_HEIGHT;
        [SerializeField] private float _cameraMinPitch = -60f;
        [SerializeField] private float _cameraMaxPitch = 60f;

        #endregion

        #region State

        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private bool _jumpPressed;
        private bool _sprintPressed;
        
        private float _verticalVelocity;
        private float _cameraPitch;
        private float _cameraYaw;

        private bool _isSprinting;
        private bool _isGrounded;
        private bool _isLocked;

        private Vector3 _lastPosition;

        #endregion

        #region Properties

        public Vector3 Position => transform.position;
        public Quaternion Rotation => transform.rotation;
        public Vector3 Forward => transform.forward;
        public Vector3 Right => transform.right;
        public Vector3 Velocity => _characterController.velocity;
        public bool IsGrounded => _characterController.isGrounded;
        public bool IsSprinting => _isSprinting;
        public bool IsLocked => _isLocked;
        public float CurrentMoveSpeed => _isSprinting ? _moveSpeed * _sprintMultiplier : _moveSpeed;

        public Camera PlayerCamera => _playerCamera;

        #endregion

        #region Events

        public event Action<Vector3> OnPositionChanged;
        public event Action OnJump;
        public event Action OnLand;

        #endregion

        #region Unity Lifecycle

        protected override void Awake()
        {
            base.Awake();

            if (_characterController == null)
                _characterController = GetComponent<CharacterController>();

            _cameraPitch = 0f;
            _cameraYaw = transform.eulerAngles.y;
            _lastPosition = transform.position;
        }

        private void Start()
        {
            // Register as player controller service
            ComponentLocator.Register<IPlayerController>(new PlayerControllerInterface(this));
        }

        private void OnDestroy()
        {
            ComponentLocator.Unregister<IPlayerController>(new PlayerControllerInterface(this));
        }

        private void Update()
        {
            if (_isLocked) return;

            UpdateGroundCheck();
            UpdateMovement();
            UpdateCamera();
            UpdatePositionTracking();
        }

        #endregion

        #region Input Handling

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _jumpPressed = true;
            }
            else if (context.canceled)
            {
                _jumpPressed = false;
            }
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                _sprintPressed = true;
                _isSprinting = true;
            }
            else if (context.canceled)
            {
                _sprintPressed = false;
                _isSprinting = false;
            }
        }

        #endregion

        #region Movement

        private void UpdateGroundCheck()
        {
            bool wasGrounded = _isGrounded;
            _isGrounded = _characterController.isGrounded;

            if (_isGrounded && !wasGrounded)
            {
                OnLand?.Invoke();
            }
        }

        private void UpdateMovement()
        {
            // Calculate movement direction
            Vector3 moveDirection = Vector3.zero;

            if (_moveInput.magnitude > 0.1f)
            {
                // Get camera-relative directions
                Vector3 forward = _playerCamera != null ? _playerCamera.transform.forward : transform.forward;
                Vector3 right = _playerCamera != null ? _playerCamera.transform.right : transform.right;

                // Flatten to horizontal plane
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();

                // Calculate movement
                moveDirection = (forward * _moveInput.y + right * _moveInput.x) * CurrentMoveSpeed;

                // Face movement direction
                if (moveDirection.magnitude > 0.1f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                    transform.rotation = Quaternion.SmoothDamp(transform.rotation, targetRotation, ref _tempRotation, 0.1f);
                }
            }

            // Apply gravity
            if (_isGrounded && _verticalVelocity < 0)
            {
                _verticalVelocity = -2f; // Small negative to keep grounded
            }
            else
            {
                _verticalVelocity -= _gravity * Time.deltaTime;
            }

            // Jump
            if (_jumpPressed && _isGrounded)
            {
                _verticalVelocity = _jumpForce;
                _jumpPressed = false;
                OnJump?.Invoke();
            }

            // Apply vertical velocity
            moveDirection.y = _verticalVelocity;

            // Move character
            _characterController.Move(moveDirection * Time.deltaTime);
        }

        private Quaternion _tempRotation;

        #endregion

        #region Camera

        private void UpdateCamera()
        {
            if (_playerCamera == null || _cameraTarget == null) return;

            // Update camera rotation
            _cameraYaw += _lookInput.x * _mouseSensitivity;
            _cameraPitch -= _lookInput.y * _mouseSensitivity;
            _cameraPitch = Mathf.Clamp(_cameraPitch, _cameraMinPitch, _cameraMaxPitch);

            // Apply rotation to camera target
            _cameraTarget.localRotation = Quaternion.Euler(_cameraPitch, _cameraYaw, 0f);

            // Position camera behind player
            Vector3 cameraOffset = -_cameraTarget.forward * _cameraDistance;
            cameraOffset.y = _cameraHeight;

            _playerCamera.transform.position = _cameraTarget.position + cameraOffset;
            _playerCamera.transform.LookAt(_cameraTarget.position + Vector3.up * 1.5f);
        }

        #endregion

        #region Position Tracking

        private void UpdatePositionTracking()
        {
            float distance = Vector3.Distance(_lastPosition, transform.position);
            if (distance > 0.1f)
            {
                OnPositionChanged?.Invoke(transform.position);
                _lastPosition = transform.position;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Locks player movement and camera.
        /// </summary>
        public void Lock()
        {
            _isLocked = true;
            _moveInput = Vector2.zero;
            _lookInput = Vector2.zero;
            _characterController.Move(Vector3.zero);
        }

        /// <summary>
        /// Unlocks player movement and camera.
        /// </summary>
        public void Unlock()
        {
            _isLocked = false;
        }

        /// <summary>
        /// Teleports the player to a position.
        /// </summary>
        public void Teleport(Vector3 position)
        {
            _characterController.enabled = false;
            transform.position = position;
            _characterController.enabled = true;
            _lastPosition = position;
        }

        /// <summary>
        /// Sets the player camera.
        /// </summary>
        public void SetCamera(Camera camera)
        {
            _playerCamera = camera;
        }

        /// <summary>
        /// Updates mouse sensitivity.
        /// </summary>
        public void SetSensitivity(float sensitivity)
        {
            _mouseSensitivity = sensitivity;
        }

        #endregion
    }

    #region Interface

    /// <summary>
    /// Interface for accessing player controller from other systems.
    /// </summary>
    public interface IPlayerController
    {
        Vector3 Position { get; }
        Quaternion Rotation { get; }
        Vector3 Forward { get; }
        Vector3 Velocity { get; }
        bool IsGrounded { get; }
        bool IsSprinting { get; }
        bool IsLocked { get; }
        Camera PlayerCamera { get; }

        void Lock();
        void Unlock();
        void Teleport(Vector3 position);
    }

    /// <summary>
    /// Implementation of IPlayerController.
    /// </summary>
    public class PlayerControllerInterface : IPlayerController
    {
        private readonly PlayerController _controller;

        public PlayerControllerInterface(PlayerController controller)
        {
            _controller = controller;
        }

        public Vector3 Position => _controller.Position;
        public Quaternion Rotation => _controller.Rotation;
        public Vector3 Forward => _controller.Forward;
        public Vector3 Velocity => _controller.Velocity;
        public bool IsGrounded => _controller.IsGrounded;
        public bool IsSprinting => _controller.IsSprinting;
        public bool IsLocked => _controller.IsLocked;
        public Camera PlayerCamera => _controller.PlayerCamera;

        public void Lock() => _controller.Lock();
        public void Unlock() => _controller.Unlock();
        public void Teleport(Vector3 position) => _controller.Teleport(position);
    }

    #endregion
}
