using UnityEngine;
using Zenject;
using TriInspector;
using _Main._Management;
using _Main._Interfaces;

namespace _Main._InputSystem
{
    /// <summary>
    /// Manages item selection and interaction based on player input.
    /// Utilizes raycasting to select and collect items.
    /// </summary>
    public class Selector : MonoBehaviour, ISelector, ICollector
    {
        #region Serialized Fields

        [Header("Camera Settings")]
        [Required, PropertyTooltip("Camera used for item selection.")]
        [SerializeField]
        private Camera _selectionCamera;

        [Header("Raycast Settings")]
        [PropertyTooltip("Layer mask for raycasting to filter raycast targets.")]
        [SerializeField]
        private LayerMask _raycastLayerMask = ~0; // Default to all layers

        [PropertyTooltip("Maximum distance for the raycast.")]
        [SerializeField]
        private float _raycastLength = 100f;

        [Header("Raycast Gizmo Colors")]
        [PropertyTooltip("Color of the raycast when the mouse button is pressed down.")]
        [SerializeField]
        private Color _mouseDownGizmoColor = Color.green;

        [PropertyTooltip("Color of the raycast when the mouse button is held.")]
        [SerializeField]
        private Color _mouseHoldGizmoColor = Color.yellow;

        [PropertyTooltip("Color of the raycast when the mouse button is released.")]
        [SerializeField]
        private Color _mouseUpGizmoColor = Color.red;

        #endregion

        #region Dependencies

        [Header("Dependencies")]
        [Inject, Required, PropertyTooltip("Scriptable object storing player input data.")]
        private PlayerInput _playerInput;

        [Inject, Required, PropertyTooltip("Manager responsible for handling items.")]
        private ItemManager _itemManager;

        #endregion

        #region Private Fields

        private ISelectable _currentSelectable;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Subscribes to input events when the component is enabled.
        /// </summary>
        private void OnEnable()
        {
            SubscribeToInputEvents();
        }

        /// <summary>
        /// Unsubscribes from input events when the component is disabled.
        /// </summary>
        private void OnDisable()
        {
            UnsubscribeFromInputEvents();
        }

        #endregion

        #region Event Subscription

        /// <summary>
        /// Subscribes to input events for item selection and interaction.
        /// </summary>
        private void SubscribeToInputEvents()
        {
            if (_playerInput == null)
            {
                Debug.LogError("PlayerInput is not assigned. Cannot subscribe to events.", this);
                return;
            }

            _playerInput.OnMouseDown += HandleMouseButtonDown;
            _playerInput.OnMouseHeld += HandleMouseHeld;
            _playerInput.OnMouseUp += HandleMouseButtonUp;
        }

        /// <summary>
        /// Unsubscribes from input events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromInputEvents()
        {
            if (_playerInput == null) return;

            _playerInput.OnMouseDown -= HandleMouseButtonDown;
            _playerInput.OnMouseHeld -= HandleMouseHeld;
            _playerInput.OnMouseUp -= HandleMouseButtonUp;
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Handles the logic when the mouse button is pressed down.
        /// Selects the item under the cursor if it is selectable.
        /// </summary>
        private void HandleMouseButtonDown()
        {
            PerformRaycastAction(hit =>
            {
                var selectable = hit.collider?.GetComponent<ISelectable>();
                if (selectable != null)
                {
                    _currentSelectable = selectable;
                    Select(_currentSelectable);
                }
            });
        }

        /// <summary>
        /// Handles the logic when the mouse button is held down.
        /// Continuously selects the item under the cursor and deselects the previous one.
        /// </summary>
        private void HandleMouseHeld()
        {
            PerformRaycastAction(hit =>
            {
                var selectable = hit.collider?.GetComponent<ISelectable>();
                if (selectable != null && _currentSelectable != selectable)
                {
                    DeSelect(_currentSelectable);
                    _currentSelectable = selectable;
                    Select(_currentSelectable);
                }
            });
        }

        /// <summary>
        /// Handles the logic when the mouse button is released.
        /// Deselects the current item and attempts to collect the item under the cursor if it is collectable.
        /// </summary>
        private void HandleMouseButtonUp()
        {
            if (_currentSelectable != null)
            {
                DeSelect(_currentSelectable);
                _currentSelectable = null;
            }

            PerformRaycastAction(hit =>
            {
                var collectable = hit.collider?.GetComponent<ICollectable>();
                if (collectable != null)
                {
                    Collect(collectable);
                }
            });
        }

        #endregion

        #region Raycasting

        /// <summary>
        /// Performs a raycast from the camera and executes the given action if an object is hit.
        /// </summary>
        /// <param name="onHit">Action to execute if the raycast hits an object.</param>
        private void PerformRaycastAction(System.Action<RaycastHit> onHit)
        {
            if (_selectionCamera == null)
            {
                Debug.LogWarning("Selection camera is not assigned.", this);
                return;
            }

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                onHit?.Invoke(hit);
            }
        }

        #endregion

        #region Selection and Collection

        /// <summary>
        /// Selects the specified selectable object.
        /// </summary>
        /// <param name="selectable">The object to select.</param>
        public void Select(ISelectable selectable)
        {
            selectable?.Select();
        }

        /// <summary>
        /// Deselects the specified selectable object.
        /// </summary>
        /// <param name="selectable">The object to deselect.</param>
        public void DeSelect(ISelectable selectable)
        {
            selectable?.DeSelect();
        }

        /// <summary>
        /// Collects the specified collectable object.
        /// </summary>
        /// <param name="collectable">The object to collect.</param>
        public void Collect(ICollectable collectable)
        {
            _itemManager?.Collect(collectable);
        }

        #endregion

        #region Gizmos

        /// <summary>
        /// Draws raycast gizmos in the editor for visualization.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (_selectionCamera == null || _playerInput == null) return;

            Vector3 rayOrigin = _selectionCamera.transform.position;
            Vector3 rayDirection = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition).direction;

            Gizmos.color = _playerInput.IsMouseHeld ? _mouseHoldGizmoColor :
                           _playerInput.IsMouseUp ? _mouseUpGizmoColor : _mouseDownGizmoColor;

            Gizmos.DrawRay(rayOrigin, rayDirection * _raycastLength);
        }

        #endregion
    }
}