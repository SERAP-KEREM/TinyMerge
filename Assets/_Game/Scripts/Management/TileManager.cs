using Zenject;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using _Game.Scripts.Tiles;
using _Game.Scripts.Items;
using _Game.Scripts._helpers;

namespace _Game.Scripts.Management
{
    public class TileManager : MonoBehaviour
    {
        [Header("Tile Settings")]
        [SerializeField] private List<Tile> _activeTileList;

        [Header("Match Animation Settings")]
        [SerializeField] private float _matchMoveAnimationDuration = 0.5f;
        [SerializeField] private float _matchScaleAnimationDuration = 0.25f;
        //[SerializeField] private float _moveUpDistance = 2f;
        //[SerializeField] private float _moveForwardDistance = 2f;

        [Header("Effects")]
        [SerializeField] private string _itemMatchParticleKey = "ItemMatch";
        [Header("Audio Effects")]
        [SerializeField, Tooltip("")]
        private string _itemMatchClipKey = "ItemMatch";

        private ParticleManager _particleManager;
        private bool _isProcessingMatch;

        [Inject]
        public void Construct(ParticleManager particleManager)
        {
            _particleManager = particleManager;
        }

        public void AlignMatchingItems()
        {
            bool itemsChanged;

            do
            {
                itemsChanged = false;

                // Collect all items from the tiles
                List<Item> items = _activeTileList.Where(tile => tile.Item != null)
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

                    // Animate the item moving to its new position
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
        /// Animates and deactivates the specified items with a custom effect.
        /// </summary>
        /// <param name="item1">The first item to animate and deactivate.</param>
        /// <param name="item2">The second item to animate and deactivate.</param>
        /// <param name="item3">The third item to animate and deactivate.</param>
        /// <param name="position2">The target position for the second item.</param>
        /// <param name="position3">The target position for the third item.</param>
        private void AnimateAndDeactivateItems(Item item1, Item item2, Item item3,
            Tile tile1, Tile tile2, Tile tile3)
        {
            // Raise the items slightly
            Sequence sequence = DOTween.Sequence();

            sequence.Append(item1.transform.DOMoveY(tile1.transform.position.y + 2,
                _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item2.transform.DOMoveY(tile2.transform.position.y + 2,
                _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item3.transform.DOMoveY(tile3.transform.position.y + 2,
                _matchMoveAnimationDuration).SetEase(Ease.OutQuad));

            // Move items towards the forward
            sequence.Append(item1.transform.DOMoveZ(tile1.transform.position.z + 2,
                _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item2.transform.DOMoveZ(tile2.transform.position.z + 2,
                _matchMoveAnimationDuration).SetEase(Ease.OutQuad));
            sequence.Join(item3.transform.DOMoveZ(tile3.transform.position.z + 2,
                _matchMoveAnimationDuration).SetEase(Ease.OutQuad));

            // Move items towards the center and apply a scale down effect
            sequence.Append(item1.transform.DOMoveX(item2.transform.position.x, _matchMoveAnimationDuration / 2).
                SetEase(Ease.InBack));
            sequence.Join(item3.transform.DOMoveX(item2.transform.position.x, _matchMoveAnimationDuration / 2).
                SetEase(Ease.InBack));

            sequence.Append(item1.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration).
                SetEase(Ease.InOutBounce));
            sequence.Join(item2.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration).
                SetEase(Ease.InOutBounce));
            sequence.Join(item3.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration).
                SetEase(Ease.InOutBounce));

            sequence.AppendCallback(() =>
            {
                _particleManager.PlayParticleAtPoint(_itemMatchParticleKey,
                    item2.transform.position);

               // GlobalBinder.singleton.AudioManager.PlaySound(_itemMatchClipKey);
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

        public Tile FindEmptyTile()
        {
            return _activeTileList.FirstOrDefault(tile => tile != null && tile.Item == null);
        }

        public void ClearTile(Tile tile)
        {
            if (tile != null)
            {
                tile.Item = null;
            }
        }
    }
}