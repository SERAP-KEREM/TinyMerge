using _Game.Scripts.Data;
using _Game.Scripts._helpers;
using UnityEngine;
using SerapKeremGameTools.Game._Interfaces;
using Zenject;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages item selection and interaction based on player input.
    /// Utilizes raycasting to select and collect items.
    /// </summary>
    public class Selector : MonoBehaviour, ISelector, ICollector
    {
        [Header("Camera Settings")]
        [SerializeField, Tooltip("Camera used for item selection.")]
        private Camera _selectionCamera;

        [Header("Raycast Settings")]
        [SerializeField, Tooltip("Layer mask for raycasting to filter raycast targets.")]
        private LayerMask _raycastLayerMask = ~0; // Default to all layers

        [SerializeField, Tooltip("Maximum distance for the raycast.")]
        private float _raycastLength = 100f;

        [Header("Raycast Gizmo Colors")]
        [SerializeField, Tooltip("Color of the raycast when the mouse button is pressed down.")]
        private Color _mouseDownGizmoColor = Color.green;

        [SerializeField, Tooltip("Color of the raycast when the mouse button is held.")]
        private Color _mouseHoldGizmoColor = Color.yellow;

        [SerializeField, Tooltip("Color of the raycast when the mouse button is released.")]
        private Color _mouseUpGizmoColor = Color.red;

        [Header("Dependencies")]
        [SerializeField, Tooltip("Scriptable object storing player input data.")]
       

        private ISelectable _currentSelectable;

        private PlayerInput _playerInput;
        private ItemManager _itemManager;
   
        [Inject]
        public void Construct(ItemManager itemManager, PlayerInput playerInput)
        {
            _itemManager = itemManager;
            _playerInput = playerInput;
        }

        private void OnEnable()
        {
            SubscribeToInputEvents();
        }

        private void OnDisable()
        {
            UnsubscribeFromInputEvents();
        }

        /// <summary>
        /// Subscribes to input events for item selection and interaction.
        /// </summary>
        private void SubscribeToInputEvents()
        {
            _playerInput.OnMouseDown += HandleMouseButtonDown;
            _playerInput.OnMouseHeld += HandleMouseHeld;
            _playerInput.OnMouseUp += HandleMouseButtonUp;
        }

        /// <summary>
        /// Unsubscribes from input events to prevent memory leaks.
        /// </summary>
        private void UnsubscribeFromInputEvents()
        {
            _playerInput.OnMouseDown -= HandleMouseButtonDown;
            _playerInput.OnMouseHeld -= HandleMouseHeld;
            _playerInput.OnMouseUp -= HandleMouseButtonUp;
        }

        /// <summary>
        /// Handles the logic when the mouse button is pressed down.
        /// Selects the item under the cursor if it is selectable.
        /// </summary>
        private void HandleMouseButtonDown()
        {
            PerformRaycastAction(hit =>
            {
                var selectable = hit.collider.GetComponent<ISelectable>();
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
                var selectable = hit.collider.GetComponent<ISelectable>();
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
                var collectable = hit.collider.GetComponent<ICollectable>();
                if (collectable != null)
                {
                    Collect(collectable);
                }
            });
        }

        /// <summary>
        /// Performs a raycast from the camera and executes the given action if an object is hit.
        /// </summary>
        /// <param name="onHit">Action to execute if the raycast hits an object.</param>
        private void PerformRaycastAction(System.Action<RaycastHit> onHit)
        {
            if (_selectionCamera == null) return;

            Ray ray = _selectionCamera.ScreenPointToRay(_playerInput.MousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLength, _raycastLayerMask))
            {
                onHit?.Invoke(hit);
            }
        }

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
            _itemManager.Collect(collectable);
        }

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
    }
}