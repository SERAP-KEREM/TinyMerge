using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;
using Zenject;
using _Game.Scripts.Services;

namespace _Game.Scripts.UI.Components
{
    [RequireComponent(typeof(EventTrigger))]
    public class LeafButton : MonoBehaviour
    {
        [Header("Effect Settings")]
        [SerializeField] private ButtonEffectSettings _effectSettings;

        [Header("Colors")]
        [SerializeField] private ButtonColorSettings _colorSettings;

        [Header("References")]
        [SerializeField] private Image _buttonImage;
        [SerializeField] private TextMeshProUGUI _buttonText;
        [SerializeField] private Image _highlightImage;

        [Header("Animation")]
        [SerializeField] private float _transitionDuration = 0.25f;

        public UnityEvent OnPressed;

       // private IAudioManager _audioManager;
        private Sequence _currentAnimation;

        //[Inject]
        //public void Construct(IAudioManager audioManager)
        //{
        //    _audioManager = audioManager;
        //}

        private void Awake()
        {
            InitializeComponents();
           // SetupEventTrigger();
        }

        private void OnDestroy()
        {
            _currentAnimation?.Kill();
        }

        private void InitializeComponents()
        {
            if (_effectSettings.UseButtonColorEffect)
            {
                _buttonImage ??= GetComponent<Image>();
                if (_buttonImage) _buttonImage.color = _colorSettings.ButtonNormalColor;
            }

            if (_effectSettings.UseTextColorEffect)
            {
                _buttonText ??= GetComponentInChildren<TextMeshProUGUI>();
                if (_buttonText) _buttonText.color = _colorSettings.TextNormalColor;
            }

            if (_effectSettings.UseHighlightEffect && _highlightImage)
            {
               // SetHighlightState(false);
            }
        }

    }

    [System.Serializable]
    public class ButtonEffectSettings
    {
        public bool UseHighlightEffect = true;
        public bool UseButtonColorEffect = true;
        public bool UseTextColorEffect = true;
        public bool UseSoundEffect = true;
    }

    [System.Serializable]
    public class ButtonColorSettings
    {
        public Color ButtonNormalColor = Color.white;
        public Color ButtonHoverColor = Color.gray;
        public Color ButtonPressedColor = Color.black;

        public Color TextNormalColor = Color.black;
        public Color TextHoverColor = Color.gray;
        public Color TextPressedColor = Color.black;
    }
}