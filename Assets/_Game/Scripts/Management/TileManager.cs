using Zenject;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using _Main._Tiles;
using _Main._Items;

namespace _Main._Management
{
    /// <summary>
    /// Manages tiles and their associated items, including alignment, matching, and animations.
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Tile Settings")]
        [Tooltip("List of active tiles in the game.")]
        [SerializeField]
        private List<Tile> _activeTileList;

        [Header("Match Animation Settings")]
        [Tooltip("Duration for the match move animation.")]
        [SerializeField]
        private float _matchMoveAnimationDuration = 0.5f;

        [Tooltip("Duration for the match scale animation.")]
        [SerializeField]
        private float _matchScaleAnimationDuration = 0.25f;

        [Header("Effects")]
        [Tooltip("Particle effect key for item matches.")]
        [SerializeField]
        private string _itemMatchParticleKey = "ItemMatch";

        [Header("Audio Effects")]
        [Tooltip("Audio clip key for item matches.")]
        [SerializeField]
        private string _itemMatchClipKey = "ItemMatch";

        #endregion

        #region Private Fields

        private ParticleManager _particleManager;
        private AudioManager _audioManager;

        #endregion

        #region Dependency Injection

        /// <summary>
        /// Injects dependencies required for the TileManager.
        /// </summary>
        [Inject]
        public void Construct(ParticleManager particleManager, AudioManager audioManager)
        {
            _particleManager = particleManager;
            _audioManager = audioManager;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Aligns matching items on tiles and triggers animations for matched items.
        /// </summary>
        public void AlignMatchingItems()
        {
            bool itemsChanged;
            do
            {
                itemsChanged = false;

                // Collect all items from the tiles
                List<Item> items = _activeTileList
                    .Where(tile => tile.Item != null)
                    .Select(tile => tile.Item)
                    .ToList();

                // Sort items by their type (e.g., ItemId)
                items = items.OrderBy(item => item.ItemId).ToList();

                // Clear all tiles
                foreach (var tile in _activeTileList)
                {
                    tile.Item = null;
                }

                // Reassign sorted items back to tiles
                for (int i = 0; i < items.Count; i++)
                {
                    _activeTileList[i].Item = items[i];
                    items[i].ItemTile = _activeTileList[i];
                }

                // Check for matching items in a row
                for (int i = 0; i < _activeTileList.Count - 2; i++)
                {
                    var tile1 = _activeTileList[i];
                    var tile2 = _activeTileList[i + 1];
                    var tile3 = _activeTileList[i + 2];

                    if (tile1.Item != null && tile2.Item != null && tile3.Item != null &&
                        tile1.Item.ItemId == tile2.Item.ItemId && tile1.Item.ItemId == tile3.Item.ItemId)
                    {
                        // Animate and deactivate the matching items
                        AnimateAndDeactivateItems(tile1.Item, tile2.Item, tile3.Item, tile1, tile2, tile3);

                        // Set the items in the tiles to null
                        tile1.Item = null;
                        tile2.Item = null;
                        tile3.Item = null;

                        itemsChanged = true;
                    }
                }
            }
            while (itemsChanged);
        }

        /// <summary>
        /// Finds an empty tile in the active tile list.
        /// </summary>
        /// <returns>An empty tile, or null if none are found.</returns>
        public Tile FindEmptyTile()
        {
            return _activeTileList.FirstOrDefault(tile => tile != null && tile.Item == null);
        }

        /// <summary>
        /// Clears the item from the specified tile.
        /// </summary>
        /// <param name="tile">The tile to clear.</param>
        public void ClearTile(Tile tile)
        {
            if (tile != null)
            {
                tile.Item = null;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Animates and deactivates the specified items with a custom effect.
        /// </summary>
        /// <param name="item1">The first item to animate and deactivate.</param>
        /// <param name="item2">The second item to animate and deactivate.</param>
        /// <param name="item3">The third item to animate and deactivate.</param>
        /// <param name="tile1">The first tile containing the item.</param>
        /// <param name="tile2">The second tile containing the item.</param>
        /// <param name="tile3">The third tile containing the item.</param>
        private void AnimateAndDeactivateItems(Item item1, Item item2, Item item3,
            Tile tile1, Tile tile2, Tile tile3)
        {
            // Raise the items slightly
            Sequence sequence = DOTween.Sequence();
            sequence.Append(item1.transform.DOMoveY(tile1.transform.position.y + 2, _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item2.transform.DOMoveY(tile2.transform.position.y + 2, _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item3.transform.DOMoveY(tile3.transform.position.y + 2, _matchMoveAnimationDuration).SetEase(Ease.OutQuad));

            // Move items forward
            sequence.Append(item1.transform.DOMoveZ(tile1.transform.position.z + 2, _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item2.transform.DOMoveZ(tile2.transform.position.z + 2, _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item3.transform.DOMoveZ(tile3.transform.position.z + 2, _matchMoveAnimationDuration).SetEase(Ease.OutQuad));

            // Move items towards the center and apply a scale-down effect
            sequence.Append(item1.transform.DOMoveX(item2.transform.position.x, _matchMoveAnimationDuration / 2).SetEase(Ease.InBack));
            sequence.Join(item3.transform.DOMoveX(item2.transform.position.x, _matchMoveAnimationDuration / 2).SetEase(Ease.InBack));
            sequence.Append(item1.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration).SetEase(Ease.InOutBounce));
            sequence.Join(item2.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration).SetEase(Ease.InOutBounce));
            sequence.Join(item3.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration).SetEase(Ease.InOutBounce));

            // Play effects after animation
            sequence.AppendCallback(() =>
            {
                _particleManager.PlayParticleAtPoint(_itemMatchParticleKey, item2.transform.position);
                _audioManager.PlaySound(_itemMatchClipKey);
            });

            // Deactivate items after the animation completes
            sequence.OnComplete(() =>
            {
                item1.gameObject.SetActive(false);
                item2.gameObject.SetActive(false);
                item3.gameObject.SetActive(false);
                AlignMatchingItems(); // Re-sort after deactivating items
            });

            sequence.Play();
        }

        #endregion
    }
}