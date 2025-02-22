using DG.Tweening;
using UnityEngine;
using Zenject;
using TriInspector;
using _Main._Tiles;
using _Main._Management;
using System;
using _Main._Interfaces;

namespace _Main._Items
{
    /// <summary>
    /// Represents an item in the game. This class encapsulates the item's properties and behaviors.
    /// </summary>
    public class Item : MonoBehaviour, ISelectable, ICollectable
    {
        #region Serialized Fields

        [Header("Item Properties")]
        [Required, PropertyTooltip("Unique identifier for the item.")]
        [SerializeField]
        private int _itemId;

        [PropertyTooltip("Icon representing the item.")]
        [SerializeField]
        private Sprite _itemIcon;

        [Header("Tile Parameters")]
        [PropertyTooltip("Tile associated with the item.")]
        [SerializeField]
        private Tile _itemTile;

        [Header("Item Move Settings")]
        [PropertyTooltip("Duration for items to move to their new positions.")]
        [SerializeField]
        private float _itemMoveDuration = 0.5f;

        [PropertyTooltip("Position offset when the item moves.")]
        [SerializeField]
        private Vector3 _itemPositionOffset = Vector3.up;

        [Header("Scale Parameters")]
        [PropertyTooltip("Rotation of the item when it is collected.")]
        [SerializeField]
        private Vector3 _itemCollectRotation = new Vector3(0f, 90f, 90f);

        [PropertyTooltip("Scale multiplier for the item when it is in its normal state.")]
        [SerializeField]
        private float _itemNormalScaleMultiplier = 1f;

        [PropertyTooltip("Scale multiplier for the item when it is selected.")]
        [SerializeField]
        private float _itemSelectedMultiplier = 1.25f;

        [PropertyTooltip("Scale multiplier for the item when it is collected.")]
        [SerializeField]
        private float _itemCollectedScaleMultiplier = 0.5f;

        [PropertyTooltip("Duration of the scale change animation.")]
        [SerializeField]
        private float _itemScaleChangeDuration = 0.2f;

        [Header("Materials")]
        [PropertyTooltip("Material when the item is selected.")]
        [SerializeField]
        private Material _itemSelectedMaterial;

        [PropertyTooltip("Material when the item is not selected.")]
        [SerializeField]
        private Material _itemDefaultMaterial;

        [Header("Effects")]
        [PropertyTooltip("Particle effect key for when the item is collected.")]
        [SerializeField]
        private string _itemCollectParticleKey = "ItemCollect";

        [PropertyTooltip("Audio clip key for when the item is collected.")]
        [SerializeField]
        private string _itemCollectClipKey = "ItemCollect";

        [Header("References")]
        [PropertyTooltip("Renderer component")]
        [SerializeField, Required]
        private Renderer _renderer;

        #endregion

        #region Private Fields

        private Rigidbody _rigidbody;
        private bool _isCollectable = true;
        private ParticleManager _particleManager;
        private AudioManager _audioManager;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the item.
        /// </summary>
        public int ItemId
        {
            get => _itemId;
            set => _itemId = value;
        }

        /// <summary>
        /// Gets or sets the icon representing the item.
        /// </summary>
        public Sprite ItemIcon
        {
            get => _itemIcon;
            set => _itemIcon = value;
        }

        /// <summary>
        /// Gets or sets the tile associated with the item.
        /// </summary>
        public Tile ItemTile
        {
            get => _itemTile;
            set
            {
                _itemTile = value;
                UpdateItemPosition();
                PlayCollectionEffects();
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item is collectable.
        /// </summary>
        public bool IsCollectable => _isCollectable;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the item's components and default state.
        /// </summary>
        private void Awake()
        {
            _rigidbody = GetComponentInChildren<Rigidbody>();
            _renderer = GetComponentInChildren<Renderer>();

            if (_renderer == null)
            {
                Debug.LogError("Renderer component is missing on the item.", this);
            }

            _itemDefaultMaterial = _renderer?.material;
            ResetItemScale();
        }

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the item.
        /// </summary>
        /// <param name="particleManager">The particle manager instance.</param>
        /// <param name="audioManager">The audio manager instance.</param>
        [Inject]
        public void Construct(ParticleManager particleManager, AudioManager audioManager)
        {
            _particleManager = particleManager ?? throw new ArgumentNullException(nameof(particleManager));
            _audioManager = audioManager ?? throw new ArgumentNullException(nameof(audioManager));
        }

        #endregion

        #region Selection and Collection Logic

        /// <summary>
        /// Handles the selection of the item and applies a scale animation.
        /// </summary>
        public void Select()
        {
            ChangeRendererMaterial(_itemSelectedMaterial);
            ApplyScaleAnimation(_itemSelectedMultiplier);
        }

        /// <summary>
        /// Handles the deselection of the item and reverts the scale.
        /// </summary>
        public void DeSelect()
        {
            ChangeRendererMaterial(_itemDefaultMaterial);
            ApplyScaleAnimation(_itemNormalScaleMultiplier);
        }

        /// <summary>
        /// Handles the collection of the item.
        /// </summary>
        public void Collect()
        {
            _isCollectable = false;
            ApplyScaleAnimation(_itemCollectedScaleMultiplier, () =>
            {
                _rigidbody.isKinematic = true;
                ChangeRendererMaterial(_itemDefaultMaterial);
            });
        }

        /// <summary>
        /// Recycles the item, resetting its collectable state and scale.
        /// </summary>
        public void Recycle()
        {
            _isCollectable = true;
            ApplyScaleAnimation(_itemNormalScaleMultiplier, () =>
            {
                _rigidbody.isKinematic = false;
                ChangeRendererMaterial(_itemDefaultMaterial);
            });
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Changes the material of the item's renderer.
        /// </summary>
        /// <param name="material">The material to apply.</param>
        private void ChangeRendererMaterial(Material material)
        {
            if (_renderer != null)
            {
                _renderer.material = material;
            }
        }

        /// <summary>
        /// Resets the scale of the item to its normal state.
        /// </summary>
        private void ResetItemScale()
        {
            transform.localScale = Vector3.one * _itemNormalScaleMultiplier;
        }

        /// <summary>
        /// Updates the item's position and rotation based on the associated tile.
        /// </summary>
        private void UpdateItemPosition()
        {
            if (_itemTile == null) return;

            Vector3 itemPosition = _itemTile.transform.position + _itemPositionOffset;
            transform.DOMove(itemPosition, _itemMoveDuration);
            transform.DORotate(_itemCollectRotation, _itemMoveDuration);
        }

        /// <summary>
        /// Plays particle and sound effects when the item is collected.
        /// </summary>
        private void PlayCollectionEffects()
        {
            if (!IsCollectable || _particleManager == null || _audioManager == null) return;

            _particleManager.PlayParticleAtPoint(_itemCollectParticleKey, transform.position);
            _audioManager.PlaySound(_itemCollectClipKey);
        }

        /// <summary>
        /// Applies a scale animation to the item.
        /// </summary>
        /// <param name="scaleMultiplier">The target scale multiplier.</param>
        /// <param name="onComplete">Optional callback to be invoked when the animation is complete.</param>
        private void ApplyScaleAnimation(float scaleMultiplier, TweenCallback onComplete = null)
        {
            transform.DOScale(Vector3.one * scaleMultiplier, _itemScaleChangeDuration).OnComplete(onComplete);
        }

        #endregion
    }
}