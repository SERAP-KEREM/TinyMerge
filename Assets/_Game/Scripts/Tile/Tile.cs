using _Game.Scripts.Items;
using UnityEngine;

namespace _Game.Scripts.Tiles
{
    /// <summary>
    /// Represents a tile in the game which can hold an item.
    /// </summary>
    public class Tile : MonoBehaviour
    {
        [SerializeField]
        private Item _item;

        /// <summary>
        /// Gets or sets the item associated with this tile.
        /// </summary>
        public Item Item
        {
            get { return _item; }
            set { _item = value; }
        }

        private void Awake()
        {
            _item = null;
        }
    }
}