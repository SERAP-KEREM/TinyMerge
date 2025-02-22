using UnityEngine;
using UnityEngine.UI;
using Zenject;
using DG.Tweening;
using System.Collections.Generic;
using TriInspector;
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
        [PropertyTooltip("Button to destroy three required items.")]
        [SerializeField, Required]
        private Button _destroyTripleItemButton;

        [PropertyTooltip("Button to shake items.")]
        [SerializeField, Required]
        private Button _itemShakerButton;

        [PropertyTooltip("Button to recycle the last collected item.")]
        [SerializeField, Required]
        private Button _recycleItemButton;

        [PropertyTooltip("Button to freeze time.")]
        [SerializeField, Required]
        private Button _freezeTimeButton;

        [Header("Item Shaker Settings")]
        [PropertyTooltip("Minimum upward force applied to items during the shake.")]
        [SerializeField]
        private float _minUpwardForce = 5f;

        [PropertyTooltip("Maximum upward force applied to items during the shake.")]
        [SerializeField]
        private float _maxUpwardForce = 10f;

        [PropertyTooltip("Minimum horizontal force applied to items during the shake.")]
        [SerializeField]
        private float _minHorizontalForce = 2f;

        [PropertyTooltip("Maximum horizontal force applied to items during the shake.")]
        [SerializeField]
        private float _maxHorizontalForce = 5f;

        [PropertyTooltip("Minimum vertical force applied to items during the shake.")]
        [SerializeField]
        private float _minVerticalForce = 2f;

        [PropertyTooltip("Maximum vertical force applied to items during the shake.")]
        [SerializeField]
        private float _maxVerticalForce = 5f;

        [Header("Freeze Time Settings")]
        [PropertyTooltip("Duration for which time is frozen.")]
        [SerializeField]
        private float _timeFreezeDuration = 10f;

        [Header("Effects")]
        [PropertyTooltip("Particle effect key for the freeze effect.")]
        [SerializeField]
        private string _freezeEffectParticleKey = "Freeze";

        [PropertyTooltip("Particle effect key for the item shaker.")]
        [SerializeField]
        private string _itemShakerParticleKey = "ItemShaker";

        [PropertyTooltip("Audio clip key for the freeze effect.")]
        [SerializeField]
        private string _freezeEffectClipKey = "Freeze";

        [PropertyTooltip("Audio clip key for the item shaker.")]
        [SerializeField]
        private string _itemShakerClipKey = "ItemShaker";

        #endregion

        #region Private Fields

        [Inject, Required]
        private ItemManager _itemManager;

        [Inject, Required]
        private ITimeManager _timeManager;

        [Inject, Required]
        private UIManager _uiManager;

        [Inject, Required]
        private ParticleManager _particleManager;

        [Inject, Required]
        private AudioManager _audioManager;

        #endregion

        #region Lifecycle Methods

        private void Start()
        {
            InitializeButtons();
        }

        private void OnDestroy()
        {
            RemoveButtonListeners();
        }

        #endregion

        #region Button Initialization

        /// <summary>
        /// Initializes button listeners for all special skill buttons.
        /// </summary>
        private void InitializeButtons()
        {
            AddButtonListener(_destroyTripleItemButton, OnDestroyTripleItem);
            AddButtonListener(_itemShakerButton, OnItemShaker);
            AddButtonListener(_recycleItemButton, OnRecycleItem);
            AddButtonListener(_freezeTimeButton, OnFreezeTime);
        }

        /// <summary>
        /// Removes all button listeners to prevent memory leaks.
        /// </summary>
        private void RemoveButtonListeners()
        {
            RemoveButtonListener(_destroyTripleItemButton, OnDestroyTripleItem);
            RemoveButtonListener(_itemShakerButton, OnItemShaker);
            RemoveButtonListener(_recycleItemButton, OnRecycleItem);
            RemoveButtonListener(_freezeTimeButton, OnFreezeTime);
        }

        /// <summary>
        /// Adds a listener to a button if it's not null.
        /// </summary>
        private void AddButtonListener(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null)
                button.onClick.AddListener(action);
        }

        /// <summary>
        /// Removes a listener from a button if it's not null.
        /// </summary>
        private void RemoveButtonListener(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null)
                button.onClick.RemoveListener(action);
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
            foreach (var item in _itemManager.ActiveItems)
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

            PlayEffect(_itemShakerParticleKey, Vector3.zero, _itemShakerClipKey);
        }

        /// <summary>
        /// Recycles the last collected item by moving it back to an available tile.
        /// </summary>
        private void OnRecycleItem()
        {
            var recycledItem = _itemManager.GetLastCollectedItem();
            if (recycledItem == null) return;

            // Disable button during animation
            if (_recycleItemButton != null)
                _recycleItemButton.interactable = false;

            // Random position within game bounds
            Vector3 randomPosition = new Vector3(
                Random.Range(-5f, 5f), // X-axis bounds
                10f,                  // Height
                Random.Range(-5f, 5f) // Z-axis bounds
            );

            // Rigidbody setup
            if (recycledItem.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            // Animation sequence
            Sequence sequence = DOTween.Sequence();
            sequence.Append(recycledItem.transform.DOMove(randomPosition, 0.5f).SetEase(Ease.OutQuad));
            sequence.OnComplete(() =>
            {
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = true;
                    rb.constraints = RigidbodyConstraints.FreezeRotation;

                    // Apply a small random force
                    Vector3 randomForce = new Vector3(
                        Random.Range(-2f, 2f),
                        0f,
                        Random.Range(-2f, 2f)
                    );
                    rb.AddForce(randomForce, ForceMode.Impulse);
                }

                _itemManager.RecycleLastCollectedItem();

                DOVirtual.DelayedCall(1f, () =>
                {
                    if (rb != null)
                    {
                        rb.velocity = Vector3.zero;
                        rb.angularVelocity = Vector3.zero;
                        rb.constraints = RigidbodyConstraints.FreezeRotation |
                                         RigidbodyConstraints.FreezePositionX |
                                         RigidbodyConstraints.FreezePositionZ;
                    }

                    // Re-enable button
                    if (_recycleItemButton != null)
                        _recycleItemButton.interactable = true;
                });
            });
        }

        /// <summary>
        /// Freezes time and plays related effects.
        /// </summary>
        private void OnFreezeTime()
        {
            _timeManager.FreezeTimer(_timeFreezeDuration);
            _uiManager.ActivateFreezeScreen(_timeFreezeDuration, 1f, 1f);
            PlayEffect(_freezeEffectParticleKey, Vector3.up * 2, _freezeEffectClipKey);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Plays a particle effect and audio clip at the specified position.
        /// </summary>
        private void PlayEffect(string particleKey, Vector3 position, string audioKey)
        {
            if (_particleManager != null)
                _particleManager.PlayParticleAtPoint(particleKey, position);

            if (_audioManager != null)
                _audioManager.PlaySound(audioKey);
        }

        #endregion
    }
}