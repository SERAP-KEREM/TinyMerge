using UnityEngine;
using _Main.Items;
using Zenject;

namespace _Main.Tiles
{
   
    public class Tile : MonoBehaviour, IPoolable<Vector3, IMemoryPool>
    {

        public class Factory : PlaceholderFactory<Vector3, Tile> { }
        private IItem _currentItem;
        private IMemoryPool _pool;
        private bool _isOccupied;

       
        public bool IsOccupied => _isOccupied;
        public IItem CurrentItem => _currentItem;
        public Vector3 Position => transform.position;

        public void OnSpawned(Vector3 position, IMemoryPool pool)
        {
            _pool = pool;
            transform.position = position;
            _isOccupied = false;
            _currentItem = null;
        }

        public void OnDespawned()
        {
            _pool = null;
            _currentItem = null;
            _isOccupied = false;
        }

        public bool TryPlaceItem(IItem item)
        {
            if (_isOccupied) return false;

            _currentItem = item;
            _isOccupied = true;
            return true;
        }

        public IItem RemoveItem()
        {
            var item = _currentItem;
            _currentItem = null;
            _isOccupied = false;
            return item;
        }

        public void Recycle()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }
    }
}