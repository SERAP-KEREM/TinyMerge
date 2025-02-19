using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace _Game.Scripts.Data
{
    /// <summary>
    /// ScriptableObject that stores and handles player input data.
    /// </summary>
    [CreateAssetMenu(fileName = "PlayerInput", menuName = "Data/PlayerInput")]
    public class PlayerInput : ScriptableObjectInstaller<PlayerInput>
    {
        /// <summary>
        /// Event triggered when the mouse button is pressed down.
        /// </summary>
        public UnityAction OnMouseDown;

        /// <summary>
        /// Event triggered while the mouse button is held down.
        /// </summary>
        public UnityAction OnMouseHeld;

        /// <summary>
        /// Event triggered when the mouse button is released.
        /// </summary>
        public UnityAction OnMouseUp;

        /// <summary>
        /// Current mouse position in the screen coordinates.
        /// </summary>
        public Vector3 MousePosition { get; private set; }

        /// <summary>
        /// Indicates whether the mouse button is currently being held down.
        /// </summary>
        public bool IsMouseHeld { get; private set; }

        /// <summary>
        /// Indicates whether the mouse button was released.
        /// </summary>
        public bool IsMouseUp { get; private set; }

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
       
    }
}
