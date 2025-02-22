using UnityEngine;
using UnityEngine.Events;
using Zenject;
using TriInspector;

namespace _Main._InputSystem
{
    /// <summary>
    /// ScriptableObject that stores and handles player input data.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Data/PlayerInput")]
    public class PlayerInput : ScriptableObjectInstaller<PlayerInput>
    {
        #region Events

        /// <summary>
        /// Event triggered when the mouse button is pressed down.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Event triggered when the mouse button is pressed down.")]
        public UnityAction OnMouseDown;

        /// <summary>
        /// Event triggered while the mouse button is held down.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Event triggered while the mouse button is held down.")]
        public UnityAction OnMouseHeld;

        /// <summary>
        /// Event triggered when the mouse button is released.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Event triggered when the mouse button is released.")]
        public UnityAction OnMouseUp;

        #endregion

        #region Properties

        /// <summary>
        /// Current mouse position in screen coordinates.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Current mouse position in screen coordinates.")]
        public Vector3 MousePosition { get; private set; }

        /// <summary>
        /// Indicates whether the mouse button is currently being held down.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Indicates whether the mouse button is currently being held down.")]
        public bool IsMouseHeld { get; private set; }

        /// <summary>
        /// Indicates whether the mouse button was released.
        /// </summary>
        [ShowInInspector, ReadOnly, PropertyTooltip("Indicates whether the mouse button was released.")]
        public bool IsMouseUp { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the mouse down state and invokes the corresponding event.
        /// </summary>
        /// <param name="position">Position of the mouse when pressed down.</param>
        public void SetMouseDown(Vector3 position)
        {
            MousePosition = position;
            IsMouseHeld = true;
            IsMouseUp = false; // Reset the IsMouseUp state
            OnMouseDown?.Invoke();
        }

        /// <summary>
        /// Sets the mouse held state and invokes the corresponding event.
        /// </summary>
        /// <param name="position">Position of the mouse while held down.</param>
        public void SetMouseHeld(Vector3 position)
        {
            MousePosition = position;
            OnMouseHeld?.Invoke();
        }

        /// <summary>
        /// Sets the mouse up state and invokes the corresponding event.
        /// </summary>
        /// <param name="position">Position of the mouse when released.</param>
        public void SetMouseUp(Vector3 position)
        {
            MousePosition = position;
            IsMouseHeld = false;
            IsMouseUp = true;
            OnMouseUp?.Invoke();
        }

        #endregion

    }
}