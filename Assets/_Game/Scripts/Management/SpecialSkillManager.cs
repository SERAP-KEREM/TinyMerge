using UnityEngine;
using UnityEngine.UI;
using Zenject;
using _Game.Scripts.Management;
using _Game.Scripts.UI;
using _Game.Scripts._helpers;
using DG.Tweening;

namespace _Game.Scripts.Services
{
    public class SpecialSkillManager : MonoBehaviour, IInitializable
    {
        [Header("Skill Buttons")]
        [SerializeField] private Button _destroyTripleItemButton;
        [SerializeField] private Button _itemShakerButton;
        [SerializeField] private Button _recycleItemButton;
        [SerializeField] private Button _freezeTimeButton;

        [Header("Item Shaker Settings")]
        [SerializeField] private float _minUpwardForce = 5f;
        [SerializeField] private float _maxUpwardForce = 10f;
        [SerializeField] private float _minHorizontalForce = 2f;
        [SerializeField] private float _maxHorizontalForce = 5f;
        [SerializeField] private float _minVerticalForce = 2f;
        [SerializeField] private float _maxVerticalForce = 5f;

        [Header("Freeze Time Settings")]
        [SerializeField] private float _timeFreezeDuration = 10f;

        [Header("Button Animation")]
        [SerializeField] private float _buttonScaleDuration = 0.2f;
        [SerializeField] private float _buttonPressScale = 0.9f;

        [Header("Effects")]
        [SerializeField] private string _freezeEffectParticleKey = "Freeze";
        [SerializeField] private string _itemShakerParticleKey = "ItemShaker";

        private ItemManager _itemManager;
        private ITimeManager _timeManager;
        private UIManager _uiManager;
        private ParticleManager _particleManager;

        [Inject]
        public void Construct(
            ItemManager itemManager,
            ITimeManager timeManager,
            UIManager uiManager,
            ParticleManager particleManager)
        {
            _itemManager = itemManager;
            _timeManager = timeManager;
            _uiManager = uiManager;
            _particleManager = particleManager;
        }

        public void Initialize()
        {
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            SetupButton(_destroyTripleItemButton, DestroyTripleItem);
            SetupButton(_itemShakerButton, ShakeItems);
            SetupButton(_recycleItemButton, RecycleLastItem);
            SetupButton(_freezeTimeButton, FreezeTime);
        }

        private void SetupButton(Button button, UnityEngine.Events.UnityAction action)
        {
            if (button != null)
            {
                button.onClick.AddListener(() => {
                    AnimateButton(button.transform);
                    action.Invoke();
                });
            }
        }

        private void AnimateButton(Transform buttonTransform)
        {
            buttonTransform.DOScale(Vector3.one * _buttonPressScale, _buttonScaleDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    buttonTransform.DOScale(Vector3.one, _buttonScaleDuration)
                        .SetEase(Ease.OutQuad);
                });
        }

        private void OnDestroy()
        {
            if (_destroyTripleItemButton) _destroyTripleItemButton.onClick.RemoveAllListeners();
            if (_itemShakerButton) _itemShakerButton.onClick.RemoveAllListeners();
            if (_recycleItemButton) _recycleItemButton.onClick.RemoveAllListeners();
            if (_freezeTimeButton) _freezeTimeButton.onClick.RemoveAllListeners();
        }

        public void DestroyTripleItem()
        {
            _itemManager.DeactivateRandomRequiredItems();
            PlayEffect(_itemShakerParticleKey, "DestroyTriple");
            Debug.Log("DestroyTripleItem skill activated.");
        }

        public void ShakeItems()
        {
            foreach (var item in _itemManager.ActiveItems)
            {
                if (item.TryGetComponent<Rigidbody>(out var rb))
                {
                    ApplyRandomForce(rb);
                }
            }

            PlayEffect(_itemShakerParticleKey, "ItemShake");
            Debug.Log("ItemShaker skill activated.");
        }

        public void RecycleLastItem()
        {
            _itemManager.RecycleLastCollectedItem();
            PlayEffect(_itemShakerParticleKey, "Recycle");
            Debug.Log("RecycleItem skill activated.");
        }

        public void FreezeTime()
        {
            _timeManager.FreezeTimer(_timeFreezeDuration);
            PlayEffect(_freezeEffectParticleKey, "Freeze", Vector3.up * 2);
            Debug.Log("FreezeTime skill activated.");
        }

        private void ApplyRandomForce(Rigidbody rb)
        {
            Vector3 force = new Vector3(
                Random.Range(_minHorizontalForce, _maxHorizontalForce),
                Random.Range(_minUpwardForce, _maxUpwardForce),
                Random.Range(_minVerticalForce, _maxVerticalForce)
            );
            rb.AddForce(force, ForceMode.Impulse);
        }

        private void PlayEffect(string particleKey, string soundKey, Vector3 position = default)
        {
            _particleManager.PlayParticleAtPoint(particleKey, position);
            // GlobalBinder.singleton.AudioManager.PlaySound(soundKey);
        }
    }
}