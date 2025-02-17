using UnityEngine;
using DG.Tweening;
using _Main.Tiles;
using Zenject;

namespace _Main.Items
{

    public class Item : MonoBehaviour, IPoolable<Vector3, IMemoryPool>, IItem
    {   
        public class Factory : PlaceholderFactory<Vector3, Item> { }
        [Header("Item Properties")]
        [SerializeField] private ItemData itemData;

        [Header("Movement Settings")]
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private Vector3 positionOffset = Vector3.up;
        [SerializeField] private Ease moveEase = Ease.OutBack;

        [Header("Scale Settings")]
        [SerializeField] private float normalScale = 1f;
        [SerializeField] private float selectedScale = 1.25f;
        [SerializeField] private float collectedScale = 0.5f;
        [SerializeField] private float scaleAnimDuration = 0.2f;

        private Renderer _renderer;
        private IMemoryPool _pool;
        private Sequence _currentSequence;
        private bool _isCollectable = true;
        private Tile _currentTile;

        public int ItemId => itemData.itemId;
        public bool IsCollectable => _isCollectable;
        public Tile CurrentTile
        {
            get => _currentTile;
            set
            {
                _currentTile = value;
                if (_currentTile != null)
                {
                    MoveToPosition(_currentTile.transform.position + positionOffset);
                }
            }
        }

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            ResetState();
        }

        public void OnSpawned(Vector3 position, IMemoryPool pool)
        {
            _pool = pool;
            transform.position = position;
            _isCollectable = true;
            ResetState();
        }

        public void OnDespawned()
        {
            _currentSequence?.Kill();
            _currentTile = null;
            _pool = null;
        }

        public void Select()
        {
            AnimateScale(selectedScale);
            _renderer.material = itemData.selectedMaterial;
        }

        public void Deselect()
        {
            AnimateScale(normalScale);
            _renderer.material = itemData.defaultMaterial;
        }

        public void Collect()
        {
            _isCollectable = false;
            _currentSequence = DOTween.Sequence()
                .Append(transform.DOScale(Vector3.one * collectedScale, scaleAnimDuration))
                .Join(transform.DORotate(Vector3.right * 90f, scaleAnimDuration))
                .SetEase(Ease.InBack);
        }

        public void Recycle()
        {
            if (_pool != null)
            {
                _pool.Despawn(this);
            }
        }

        private void MoveToPosition(Vector3 targetPosition)
        {
            transform.DOMove(targetPosition, moveDuration)
                    .SetEase(moveEase);
        }

        private void AnimateScale(float targetScale)
        {
            transform.DOScale(Vector3.one * targetScale, scaleAnimDuration)
                    .SetEase(Ease.OutBack);
        }

        private void ResetState()
        {
            transform.localScale = Vector3.one * normalScale;
            transform.rotation = Quaternion.identity;
            _renderer.material = itemData.defaultMaterial;
        }
    }

    [System.Serializable]
    public class ItemData
    {
        public int itemId;
        public Sprite icon;
        public Material defaultMaterial;
        public Material selectedMaterial;
        [TextArea] public string description;
    }

    public interface IItem
    {
        int ItemId { get; }
        bool IsCollectable { get; }
        void Select();
        void Deselect();
        void Collect();
        void Recycle();
    }
}