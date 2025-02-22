using System;
using UnityEngine;
using Zenject;
using TriInspector;

namespace _Main._InputSystem
{
    /// <summary>
    /// Handles user input and forwards the input data to the PlayerInput ScriptableObject.
    /// </summary>
    public class InputHandler : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Input Settings")]
        [Required, PropertyTooltip("Scriptable object for handling player input.")]
        [SerializeField]
        private PlayerInput _playerInput;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects the PlayerInput ScriptableObject via Zenject.
        /// </summary>
        /// <param name="playerInput">The PlayerInput ScriptableObject instance.</param>
        [Inject]
        public void Construct(PlayerInput playerInput)
        {
            _playerInput = playerInput ?? throw new ArgumentNullException(nameof(playerInput), "PlayerInput cannot be null.");
        }

        #endregion

        #region Unity Lifecycle Methods

        /// <summary>
        /// Updates every frame to handle user input.
        /// </summary>
        private void Update()
        {
            HandleMouseInput();
        }

        #endregion

        #region Private Methods

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

        #endregion
    }
}