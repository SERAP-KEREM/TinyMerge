using Zenject;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using TriInspector;
using _Main._Tiles;
using _Main._Items;
using System.Collections;

namespace _Main._Management
{
    /// <summary>
    /// Manages tiles and their associated items, including alignment, matching, and animations.
    /// </summary>
    public class TileManager : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Tile Configuration")]
        [SerializeField, Tooltip("List of active tiles in the game."), Required]
        private List<Tile> _activeTileList;

        [Header("Match Animation Settings")]
        [SerializeField, Tooltip("Duration for the match move animation."), Range(0.1f, 2f)]
        private float _matchMoveAnimationDuration = 0.5f;

        [SerializeField, Tooltip("Duration for the match scale animation."), Range(0.1f, 1f)]
        private float _matchScaleAnimationDuration = 0.25f;

        [SerializeField, Tooltip("Height to which items are raised during the match animation.")]
        private float _matchMoveHeight = 2f;

        [SerializeField, Tooltip("Forward distance to which items are moved during the match animation.")]
        private float _matchMoveForward = 2f;

        [Header("Effects")]
        [SerializeField, Tooltip("Particle effect key for item matches."), Required]
        private string _itemMatchParticleKey = "ItemMatch";

        [SerializeField, Tooltip("Audio clip key for item matches."), Required]
        private string _itemMatchClipKey = "ItemMatch";

        #endregion

        #region Private Fields

        [Inject, Required]
        private ParticleManager _particleManager;

        [Inject, Required]
        private AudioManager _audioManager;

        private bool _isProcessingMatch = false;

        #endregion

        #region Public Methods

        /// <summary>
        /// Aligns matching items on tiles and triggers animations for matched items.
        /// </summary>
        public void AlignMatchingItems()
        {
            if (_isProcessingMatch) return;
            StartCoroutine(AlignItemsRoutine());
        }

        /// <summary>
        /// Finds an empty tile in the active tile list.
        /// </summary>
        public Tile FindEmptyTile() =>
            _activeTileList.FirstOrDefault(tile => tile != null && tile.Item == null);

        /// <summary>
        /// Clears the item from the specified tile.
        /// </summary>
        public void ClearTile(Tile tile)
        {
            if (tile != null)
                tile.Item = null;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Coroutine that aligns items and processes matches.
        /// </summary>
        private IEnumerator AlignItemsRoutine()
        {
            _isProcessingMatch = true;

            bool matchFound;
            do
            {
                matchFound = false;
                SortItems();

                // Check for matches
                for (int i = 0; i < _activeTileList.Count - 2; i++)
                {
                    if (CheckForMatch(i, out MatchData matchData))
                    {
                        yield return StartCoroutine(ProcessMatchRoutine(matchData));
                        matchFound = true;
                        break;
                    }
                }

                yield return new WaitForSeconds(0.1f); // Small delay between checks
            }
            while (matchFound);

            _isProcessingMatch = false;
        }

        /// <summary>
        /// Sorts items on tiles based on their ItemId.
        /// </summary>
        private void SortItems()
        {
            var items = _activeTileList
                .Where(tile => tile.Item != null)
                .Select(tile => tile.Item)
                .OrderBy(item => item.ItemId)
                .ToList();

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
        }

        /// <summary>
        /// Checks if there is a match starting at the given index.
        /// </summary>
        private bool CheckForMatch(int startIndex, out MatchData matchData)
        {
            var tile1 = _activeTileList[startIndex];
            var tile2 = _activeTileList[startIndex + 1];
            var tile3 = _activeTileList[startIndex + 2];

            matchData = new MatchData(tile1, tile2, tile3);
            return matchData.IsValid && matchData.IsMatching;
        }

        /// <summary>
        /// Processes a match by animating, deactivating, and clearing the matched items.
        /// </summary>
        private IEnumerator ProcessMatchRoutine(MatchData match)
        {
            // Play match animation
            var sequence = CreateMatchAnimation(match);

            // Wait for animation to complete
            yield return new WaitForSeconds(sequence.Duration());

            // Play effects
            _particleManager.PlayParticleAtPoint(_itemMatchParticleKey, match.CenterPosition);
            _audioManager.PlaySound(_itemMatchClipKey);

            // Deactivate items and clear tiles
            match.DeactivateItems();
            match.ClearTiles();
        }

        /// <summary>
        /// Creates the animation sequence for a match.
        /// </summary>
        private Sequence CreateMatchAnimation(MatchData match)
        {
            Sequence sequence = DOTween.Sequence();

            // Move up
            sequence.Append(DOTween.Sequence()
                .Join(match.Item1.transform.DOMoveY(match.Tile1.transform.position.y + _matchMoveHeight, _matchMoveAnimationDuration))
                .Join(match.Item2.transform.DOMoveY(match.Tile2.transform.position.y + _matchMoveHeight, _matchMoveAnimationDuration))
                .Join(match.Item3.transform.DOMoveY(match.Tile3.transform.position.y + _matchMoveHeight, _matchMoveAnimationDuration))
                .SetEase(Ease.OutQuad));

            // Move forward
            sequence.Append(DOTween.Sequence()
                .Join(match.Item1.transform.DOMoveZ(match.Tile1.transform.position.z + _matchMoveForward, _matchMoveAnimationDuration))
                .Join(match.Item2.transform.DOMoveZ(match.Tile2.transform.position.z + _matchMoveForward, _matchMoveAnimationDuration))
                .Join(match.Item3.transform.DOMoveZ(match.Tile3.transform.position.z + _matchMoveForward, _matchMoveAnimationDuration))
                .SetEase(Ease.OutQuad));

            // Move to center and scale down
            sequence.Append(DOTween.Sequence()
                .Join(match.Item1.transform.DOMoveX(match.Item2.transform.position.x, _matchMoveAnimationDuration / 2))
                .Join(match.Item3.transform.DOMoveX(match.Item2.transform.position.x, _matchMoveAnimationDuration / 2))
                .SetEase(Ease.InBack));

            sequence.Append(DOTween.Sequence()
                .Join(match.Item1.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration))
                .Join(match.Item2.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration))
                .Join(match.Item3.transform.DOScale(Vector3.zero, _matchScaleAnimationDuration))
                .SetEase(Ease.InOutBounce));

            sequence.Play();
            return sequence;
        }

        #endregion

        #region Helper Classes

        /// <summary>
        /// Represents a set of three tiles and their associated items for matching logic.
        /// </summary>
        private class MatchData
        {
            public readonly Tile Tile1, Tile2, Tile3;
            public readonly Item Item1, Item2, Item3;

            public bool IsValid => Item1 != null && Item2 != null && Item3 != null;
            public bool IsMatching => IsValid &&
                Item1.ItemId == Item2.ItemId && Item1.ItemId == Item3.ItemId;

            public Vector3 CenterPosition => Item2.transform.position;

            public MatchData(Tile t1, Tile t2, Tile t3)
            {
                Tile1 = t1;
                Tile2 = t2;
                Tile3 = t3;
                Item1 = t1.Item;
                Item2 = t2.Item;
                Item3 = t3.Item;
            }

            public void DeactivateItems()
            {
                if (Item1 != null) Item1.gameObject.SetActive(false);
                if (Item2 != null) Item2.gameObject.SetActive(false);
                if (Item3 != null) Item3.gameObject.SetActive(false);
            }

            public void ClearTiles()
            {
                Tile1.Item = null;
                Tile2.Item = null;
                Tile3.Item = null;
            }
        }

        #endregion
    }
}