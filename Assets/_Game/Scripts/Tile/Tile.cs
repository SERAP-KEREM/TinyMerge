using _Main._Items;
using TriInspector;
using UnityEngine;

namespace _Main._Tiles
{
    /// <summary>
    /// Represents a tile in the game which can hold an item.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        #region Serialized Fields

        [PropertyTooltip("The item currently associated with this tile.")]
        [SerializeField]
        private Item _item;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the item associated with this tile.
        /// </summary>
        public Item Item
        {
            get => _item;
            set => _item = value;
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Initializes the tile by clearing any associated item.
        /// </summary>
        private void Awake()
        {
            _item = null;
        }

        #endregion
    }
}