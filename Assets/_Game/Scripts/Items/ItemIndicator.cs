using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TriInspector;

namespace _Main._Items
{
    /// <summary>
    /// Manages the display of an item indicator, including the icon and quantity text.
    /// This class handles updating the UI and animating changes when the quantity decreases.
    /// </summary>
    public class ItemIndicator : MonoBehaviour
    {
        #region Serialized Fields

        [Header("UI Components")]
        [Required, PropertyTooltip("The icon image for the item.")]
        [SerializeField]
        private Image _itemIcon;

        [Required, PropertyTooltip("The text component displaying the quantity.")]
        [SerializeField]
        private TextMeshProUGUI _quantityText;

        [Header("Scale Effects Parameters")]
        [PropertyTooltip("Duration for the scale pop effect when the quantity changes.")]
        [SerializeField]
        private float _scaleEffectDuration = 0.5f;

        [PropertyTooltip("The amount by which the item icon will pop up when the quantity decreases.")]
        [SerializeField]
        private float _scalePopAmount = 1f;

        #endregion

        #region Private Fields

        private int _quantity;

        #endregion

        #region Initialization

        /// <summary>
        /// Initializes the indicator with an icon and a quantity.
        /// </summary>
        /// <param name="icon">The icon to display.</param>
        /// <param name="quantity">The initial quantity to display.</param>
        public void Initialize(Sprite icon, int quantity)
        {
            SetIcon(icon);
            SetQuantity(quantity);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the icon of the indicator.
        /// </summary>
        /// <param name="icon">The icon to display.</param>
        public void SetIcon(Sprite icon)
        {
            if (_itemIcon == null)
            {
                Debug.LogWarning("Item icon Image component is not assigned.", this);
                return;
            }
            _itemIcon.sprite = icon;
        }

        /// <summary>
        /// Sets the quantity and updates the text display.
        /// </summary>
        /// <param name="quantity">The quantity to display.</param>
        public void SetQuantity(int quantity)
        {
            _quantity = Mathf.Max(0, quantity); // Ensure quantity is not negative
            UpdateQuantityText();
        }

        /// <summary>
        /// Decreases the displayed quantity by one and triggers the corresponding animation.
        /// </summary>
        public void DecreaseQuantity()
        {
            if (_quantity <= 0) return;

            _quantity--;
            UpdateQuantityText();
            PlayDecreaseAnimation();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the quantity text to reflect the current quantity.
        /// </summary>
        private void UpdateQuantityText()
        {
            if (_quantityText == null)
            {
                Debug.LogWarning("Quantity TextMeshPro component is not assigned.", this);
                return;
            }
            _quantityText.text = _quantity.ToString();
        }

        /// <summary>
        /// Plays the scale animation when the quantity decreases.
        /// If the quantity is zero, the item fades out.
        /// </summary>
        private void PlayDecreaseAnimation()
        {
            if (_quantity > 0)
            {
                // Play pop effect when quantity is reduced but not yet zero
                transform.DOPunchScale(Vector3.one * _scalePopAmount, _scaleEffectDuration, 5, 0.1f)
                         .OnComplete(() => ResetScale());
            }
            else
            {
                // Fade out and disable the game object when quantity reaches zero
                transform.DOScale(Vector3.zero, _scaleEffectDuration)
                         .OnComplete(() => gameObject.SetActive(false));
            }
        }

        /// <summary>
        /// Resets the scale of the indicator to its original size.
        /// </summary>
        private void ResetScale()
        {
            transform.localScale = Vector3.one;
        }

        #endregion
    }
}