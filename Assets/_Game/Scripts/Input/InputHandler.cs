using UnityEngine;
using _Game.Scripts.Data;
using Zenject;

namespace _Game.Scripts
{
    /// <summary>
    /// Handles user input and forwards the input data to the PlayerInput ScriptableObject.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        [Header("Input Settings")]
        [Tooltip("Scriptable object for handling player input.")]
        private PlayerInput _playerInput;

        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput;
        }
        private void Update()
        {
            HandleMouseInput();
        }

        /// <summary>
        /// Handles mouse input and updates the PlayerInput ScriptableObject.
        /// </summary>
        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse button pressed
            {
                _playerInput.SetMouseDown(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0)) // Left mouse button held
            {
                _playerInput.SetMouseHeld(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0)) // Left mouse button released
            {
                _playerInput.SetMouseUp(Input.mousePosition);
            }
        }
    }
}
