
using UnityEngine;

namespace SerapKeremGameTools._Game._InputSystem
{
    /// <summary>
    /// Handles all player input for FPS controls, including movement, sprinting, jumping, crouching, zooming, and interacting.
    /// </summary>
    public class PlayerFPSInput : MonoBehaviour
    {
        // Static properties for accessing input states globally.
        public static Vector2 MovementInput { get; private set; }
        public static bool IsSprinting { get; private set; }
        public static bool IsShouldJump { get; private set; }
        public static bool IsShouldCrouch { get; private set; }
        public static bool IsZooming { get; private set; }

        public static float MouseLookInputX { get; private set; }
        public static float MouseLookInputY { get; private set; }

        [Header("Controls")]
        [SerializeField] private KeyCode _sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode _jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode _crouchKey = KeyCode.LeftControl;
        [SerializeField] public static KeyCode ZoomKey = KeyCode.Mouse1;
        [SerializeField] public static KeyCode InteractKey = KeyCode.E;

        [Header("Movement")]
        [SerializeField] private float _mouseSensitivityX = 2.0f;
        [SerializeField] private float _mouseSensitivityY = 2.0f;

        private void Update()
        {
            HandleMovementInput();
            HandleActions();
            HandleMouseLook();

            // Interaction input handled here.
            //if (Input.GetKey(InteractKey))
            //{
            //    FPSInteractionSystem.Instance.TryInteract();
            //}
        }

        /// <summary>
        /// Captures movement input (WASD or arrow keys).
        /// </summary>
        private void HandleMovementInput()
        {
            MovementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        /// <summary>
        /// Handles action inputs such as sprinting, jumping, crouching, and zooming.
        /// </summary>
        private void HandleActions()
        {
            IsSprinting = Input.GetKey(_sprintKey);
            IsShouldJump = Input.GetKeyDown(_jumpKey);
            IsShouldCrouch = Input.GetKeyDown(_crouchKey);
            IsZooming = Input.GetKey(ZoomKey);
        }

        /// <summary>
        /// Captures mouse input for camera rotation.
        /// </summary>
        private void HandleMouseLook()
        {
            MouseLookInputX = Input.GetAxis("Mouse X") * _mouseSensitivityX;
            MouseLookInputY = Input.GetAxis("Mouse Y") * _mouseSensitivityY;
        }
    }
}
