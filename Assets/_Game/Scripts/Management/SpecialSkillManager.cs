using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using System.Collections.Generic;
using _Main._Items;

namespace _Main._Management
{
    /// <summary>
    /// Manages special skills in the game, such as destroying items, shaking items, recycling items, and freezing time.
    /// </summary>
    public class SpecialSkillManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Skill Buttons")]
        [Tooltip("Button to destroy three required items.")]
        [SerializeField]
        private Button _destroyTripleItemButton;

        [Tooltip("Button to shake items.")]
        [SerializeField]
        private Button _itemShakerButton;

        [Tooltip("Button to recycle the last collected item.")]
        [SerializeField]
        private Button _recycleItemButton;

        [Tooltip("Button to freeze time.")]
        [SerializeField]
        private Button _freezeTimeButton;

        [Header("Item Shaker Settings")]
        [Tooltip("Minimum upward force applied to items during the shake.")]
        [SerializeField]
        private float _minUpwardForce = 5f;

        [Tooltip("Maximum upward force applied to items during the shake.")]
        [SerializeField]
        private float _maxUpwardForce = 10f;

        [Tooltip("Minimum horizontal force applied to items during the shake.")]
        [SerializeField]
        private float _minHorizontalForce = 2f;

        [Tooltip("Maximum horizontal force applied to items during the shake.")]
        [SerializeField]
        private float _maxHorizontalForce = 5f;

        [Tooltip("Minimum vertical force applied to items during the shake.")]
        [SerializeField]
        private float _minVerticalForce = 2f;

        [Tooltip("Maximum vertical force applied to items during the shake.")]
        [SerializeField]
        private float _maxVerticalForce = 5f;

        [Header("Freeze Time Settings")]
        [Tooltip("Duration for which time is frozen.")]
        [SerializeField]
        private float _timeFreezeDuration = 10f;

        [Header("Effects")]
        [Tooltip("Particle effect key for the freeze effect.")]
        [SerializeField]
        private string _freezeEffectParticleKey = "Freeze";

        [Tooltip("Particle effect key for the item shaker.")]
        [SerializeField]
        private string _itemShakerParticleKey = "ItemShaker";

        [Tooltip("Audio clip key for the freeze effect.")]
        [SerializeField]
        private string _freezeEffectClipKey = "Freeze";

        [Tooltip("Audio clip key for the item shaker.")]
        [SerializeField]
        private string _itemShakerClipKey = "ItemShaker";

        #endregion

        #region Private Fields

        private ItemManager _itemManager;
        private ITimeManager _timeManager;
        private UIManager _uiManager;
        private ParticleManager _particleManager;
        private AudioManager _audioManager;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the SpecialSkillManager.
        /// </summary>
        [Inject]
        private void Construct(
            ItemManager itemManager,
            ITimeManager timeManager,
            UIManager uiManager,
            ParticleManager particleManager,
            AudioManager audioManager)
        {
            _itemManager = itemManager;
            _timeManager = timeManager;
            _uiManager = uiManager;
            _particleManager = particleManager;
            _audioManager = audioManager;
        }

        #endregion

        #region Lifecycle Methods

        private void Start()
        {
            InitializeButtons();
        }

        private void OnDestroy()
        {
            if (_destroyTripleItemButton != null)
                _destroyTripleItemButton.onClick.RemoveListener(OnDestroyTripleItem);

            if (_itemShakerButton != null)
                _itemShakerButton.onClick.RemoveListener(OnItemShaker);

            if (_recycleItemButton != null)
                _recycleItemButton.onClick.RemoveListener(OnRecycleItem);

            if (_freezeTimeButton != null)
                _freezeTimeButton.onClick.RemoveListener(OnFreezeTime);
        }

        #endregion

        #region Button Initialization

        /// <summary>
        /// Initializes button listeners for all special skill buttons.
        /// </summary>
        private void InitializeButtons()
        {
            if (_destroyTripleItemButton != null)
                _destroyTripleItemButton.onClick.AddListener(OnDestroyTripleItem);

            if (_itemShakerButton != null)
                _itemShakerButton.onClick.AddListener(OnItemShaker);

            if (_recycleItemButton != null)
                _recycleItemButton.onClick.AddListener(OnRecycleItem);

            if (_freezeTimeButton != null)
                _freezeTimeButton.onClick.AddListener(OnFreezeTime);
        }

        #endregion

        #region Skill Actions

        /// <summary>
        /// Destroys up to three required items.
        /// </summary>
        private void OnDestroyTripleItem()
        {
            _itemManager.DeactivateRandomRequiredItems();
        }

        /// <summary>
        /// Shakes active items by applying random forces to their rigidbodies.
        /// </summary>
        private void OnItemShaker()
        {
            List<Item> items = _itemManager.ActiveItems;

            foreach (Item item in items)
            {
                if (item.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 force = new Vector3(
                        Random.Range(_minHorizontalForce, _maxHorizontalForce),
                        Random.Range(_minUpwardForce, _maxUpwardForce),
                        Random.Range(_minVerticalForce, _maxVerticalForce)
                    );

                    rb.AddForce(force, ForceMode.Impulse);
                }
            }

            _particleManager.PlayParticleAtPoint(_itemShakerParticleKey, Vector3.zero);
            _audioManager.PlaySound(_itemShakerClipKey);
        }

        /// <summary>
        /// Recycles the last collected item by moving it back to an available tile.
        /// </summary>
        private void OnRecycleItem()
        {
            var recycledItem = _itemManager.GetLastCollectedItem();

            if (recycledItem != null)
            {
                // Disable button during animation
                if (_recycleItemButton != null)
                    _recycleItemButton.interactable = false;

                if (recycledItem.TryGetComponent<Rigidbody>(out var rb))
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }

                Vector3 targetPosition = _itemManager.GetAvailableTilePosition();

                recycledItem.transform.DOMove(targetPosition, 0.5f)
                    .SetEase(Ease.OutBack)
                    .OnComplete(() =>
                    {
                        if (rb != null)
                        {
                            rb.isKinematic = false;
                            rb.useGravity = true;
                            rb.velocity = Vector3.zero;
                            rb.angularVelocity = Vector3.zero;

                            // Freeze position except Y axis
                            rb.constraints = RigidbodyConstraints.FreezeRotation |
                                             RigidbodyConstraints.FreezePositionX |
                                             RigidbodyConstraints.FreezePositionZ;
                        }

                        _itemManager.RecycleLastCollectedItem();

                        // Re-enable button after animation
                        if (_recycleItemButton != null)
                            _recycleItemButton.interactable = true;
                    });
            }
        }

        /// <summary>
        /// Freezes time and plays related effects.
        /// </summary>
        private void OnFreezeTime()
        {
            _timeManager.FreezeTimer(_timeFreezeDuration);
            _uiManager.ActivateFreezeScreen(_timeFreezeDuration, 1f, 1f);
            _particleManager.PlayParticleAtPoint(_freezeEffectParticleKey, Vector3.up * 2);
            _audioManager.PlaySound(_freezeEffectClipKey);
        }

        #endregion
    }
}