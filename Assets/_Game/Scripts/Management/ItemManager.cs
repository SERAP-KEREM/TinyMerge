using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using _Game.Scripts.Data;
using _Game.Scripts.Items;
using _Game.Scripts.Tiles;

using UnityEngine.Playables;
using SerapKeremGameTools.Game._Interfaces;
using Zenject;
using _Game.Scripts._helpers;

namespace _Game.Scripts.Management
{
    /// <summary>
    /// Manages the creation, categorization, and collection of items in the game.
    /// Ensures items are spawned, organized, and handled according to the level configuration.
    /// </summary>
    public class ItemManager : MonoBehaviour, ICollector
    {
        [Header("Level Configuration")]
        [SerializeField, Tooltip("Configuration data for the level, including items.")]
        private GameData _gameData;

        [Header("Item Lists")]
        [SerializeField, HideInInspector]
        private List<Item> _generatedItems = new List<Item>();
        [SerializeField, HideInInspector]
        private List<Item> _activeItems = new List<Item>();
        public List<Item> ActiveItems { get => _activeItems; private set => _activeItems = value; }

        [SerializeField, HideInInspector]
        private List<Item> _activeRequiredItems = new List<Item>();
        [SerializeField, HideInInspector]
        private List<Item> _activeNormalItems = new List<Item>();
        [SerializeField, HideInInspector]
        private List<Item> _collectedItems = new List<Item>();

        [Header("Item Creation Settings")]
        [SerializeField, Tooltip("Time interval between creating each item."), Range(0.001f, 0.1f)]
        private float _itemCreationInterval = 0.1f;

        [SerializeField, Tooltip("The minimum rotation values (x, y, z) for the randomly generated item.")]
        private Vector3 _minRotation = new Vector3(0f, 0f, 0f);

        [SerializeField, Tooltip("The maximum rotation values (x, y, z) for the randomly generated item.")]
        private Vector3 _maxRotation = new Vector3(360f, 360f, 360f);

        [Header("Spawn Area Settings")]
        [SerializeField, Tooltip("Minimum and maximum spawn positions on the horizontal axis.")]
        private Vector2 _horizontalSpawnRange = new Vector2(-5f, 5f);
        [SerializeField, Tooltip("Minimum and maximum spawn positions on the vertical axis.")]
        private Vector2 _verticalSpawnRange = new Vector2(-5f, 5f);
        [SerializeField, Tooltip("Minimum and maximum spawn positions on the vertical axis.")]
        private Vector2 _upwardSpawnRange = new Vector2(5f, 10f);
        [SerializeField, Tooltip("Height at which items are spawned.")]
        private float _spawnHeight = 1f;

        [Header("Effects")]
        [Header("Particle Effects")]
        [SerializeField, Tooltip("")]
        private string _itemRecycleParticleKey = "ItemRecycle";
        [SerializeField, Tooltip("")]
        private string _itemDestroyParticleKey = "ItemDestroy";
        [Header("Audio Effects")]
        [SerializeField, Tooltip("")]
        private string _itemRecycleClipKey = "ItemRecycle";
        [SerializeField, Tooltip("")]
        private string _itemDestroyClipKey = "ItemDestroy";


        private TileManager _tileManager;
        private LevelManager _levelManager;
        private ParticleManager _particleManager;




        [Inject]
        public void Construct(TileManager tileManager, LevelManager levelManager,
            ParticleManager particleManager, GameData gameData) // GameData ekle
        {
            _tileManager = tileManager;
            _levelManager = levelManager;
            _particleManager = particleManager;
            _gameData = gameData; // GameData'y? inject et
        }


        private void Start()
        {
            SpawnItemsRoutine();
        }

        /// <summary>
        /// Coroutine that spawns items at intervals, ensuring the total number matches the configuration.
        /// </summary>
        private void SpawnItemsRoutine()
        {
            var itemCreationTracker = _gameData.CurrentLevel.ItemDataList.
                ToDictionary(itemData => itemData, itemData => 0);

            int totalItemsToCreate = _gameData.CurrentLevel.ItemDataList.
                Sum(itemData => GetValidatedItemCount(itemData.ItemCount));

            for (int i = 0; i < totalItemsToCreate; i++)
            {
                var itemData = GetRandomAvailableItemData(itemCreationTracker);
                CreateItem(itemData, itemCreationTracker[itemData] + 1);
                itemCreationTracker[itemData]++;
            }

            CategorizeItems();
        }

        /// <summary>
        /// Ensures the item count is a multiple of three.
        /// </summary>
        /// <param name="requestedCount">Requested item count.</param>
        /// <returns>Adjusted count, multiple of three.</returns>
        private int GetValidatedItemCount(int requestedCount)
        {
            return Mathf.CeilToInt(requestedCount / 3f) * 3;
        }

        /// <summary>
        /// Selects a random item data that has not yet reached its creation limit.
        /// </summary>
        /// <param name="itemCreationTracker">Tracker for item creation counts.</param>
        /// <returns>Randomly selected item data.</returns>
        private LevelConfig.ItemData GetRandomAvailableItemData(Dictionary<LevelConfig.ItemData,
            int> itemCreationTracker)
        {
            LevelConfig.ItemData randomItemData;
            do
            {
                randomItemData = _gameData.CurrentLevel.ItemDataList[Random.Range(0, _gameData.CurrentLevel.ItemDataList.Count)];
            } while (itemCreationTracker[randomItemData] >= GetValidatedItemCount(randomItemData.ItemCount));

            return randomItemData;
        }
        /// <summary>
        /// Spawns an item with a random rotation and assigns it a unique name, then adds it to the active list.
        /// </summary>
        /// <param name="itemData">Data of the item to create.</param>
        /// <param name="itemNumber">Item's number in the current set.</param>
        private void CreateItem(LevelConfig.ItemData itemData, int itemNumber)
        {
            _spawnHeight = Random.Range(_upwardSpawnRange.x, _upwardSpawnRange.y);

            Vector3 spawnPosition = new Vector3(
                Random.Range(_horizontalSpawnRange.x, _horizontalSpawnRange.y),
                _spawnHeight,
                Random.Range(_verticalSpawnRange.x, _verticalSpawnRange.y)
            );

            Quaternion randomRotation = Quaternion.Euler(
                Random.Range(_minRotation.x, _maxRotation.x),
                Random.Range(_minRotation.y, _maxRotation.y),
                Random.Range(_minRotation.z, _maxRotation.z)
            );

            Item newItem = Instantiate(itemData.ItemPrefab, spawnPosition, randomRotation, transform);
            newItem.name = $"{itemData.ItemPrefab.GetType().Name}_{itemNumber}";

            _generatedItems.Add(newItem);
            _activeItems.Add(newItem);
        }

        /// <summary>
        /// Categorizes active items into required and normal lists based on their configuration.
        /// </summary>
        private void CategorizeItems()
        {
            _activeRequiredItems = _activeItems
                .Where(item => _gameData.CurrentLevel.ItemDataList.Any(data => data.ItemPrefab.ItemId == item.ItemId && data.IsRequired))
                .ToList();

            _activeNormalItems = _activeItems
                .Except(_activeRequiredItems)
                .ToList();
        }

        /// <summary>
        /// Collects an item and moves it to the collected list. Updates related managers accordingly.
        /// </summary>
        /// <param name="collectable">The item to collect.</param>
        public void Collect(ICollectable collectable)
        {
            if (collectable is not Item collectedItem || !collectedItem.IsCollectable) return;

            Tile emptyTile = _tileManager.FindEmptyTile();
            if (emptyTile == null)
            {
                Debug.Log("No empty tile available to place the item.");
                _levelManager.LevelFail();
                return;
            }

            collectedItem.ItemTile = emptyTile;
            emptyTile.Item = collectedItem;

            collectedItem.Collect();
            UpdateItemListsOnCollect(collectedItem);

            _levelManager.UpdateItemCollection(collectedItem);
            _tileManager.AlignMatchingItems();
        }

        /// <summary>
        /// Updates item lists when an item is collected.
        /// </summary>
        /// <param name="collectedItem">The collected item.</param>
        private void UpdateItemListsOnCollect(Item collectedItem)
        {
            _activeItems.Remove(collectedItem);
            _collectedItems.Add(collectedItem);

            if (_activeRequiredItems.Remove(collectedItem) == false)
            {
                _activeNormalItems.Remove(collectedItem);
            }
        }

        /// <summary>
        /// Recycles the last collected item, moving it back to the active list.
        /// </summary>
        public void RecycleLastCollectedItem()
        {
            if (_collectedItems.Any())
            {
                Item _lastCollectedItem = _collectedItems.Last();
                RecycleItem(_lastCollectedItem);

                _particleManager.PlayParticleAtPoint(_itemRecycleParticleKey,
                    _lastCollectedItem.transform.position);

               // GlobalBinder.singleton.AudioManager.PlaySound(_itemRecycleClipKey);
            }
        }

        /// <summary>
        /// Recycles an item, moving it from the collected list back to the active list.
        /// </summary>
        /// <param name="recycledItem">The item to be recycled.</param>
        private void RecycleItem(Item recycledItem)
        {
            _collectedItems.Remove(recycledItem);
            _activeItems.Add(recycledItem);

            if (_gameData.CurrentLevel.ItemDataList.Any(data => data.ItemPrefab.ItemId == recycledItem.ItemId &&
                data.IsRequired))
            {
                _activeRequiredItems.Add(recycledItem);
            }
            else
            {
                _activeNormalItems.Add(recycledItem);
            }

            recycledItem.Recycle();
           _tileManager.ClearTile(recycledItem.ItemTile);
            recycledItem.ItemTile = null;
        }

        /// <summary>
        /// Deactivates up to 3 required items with the same ID.
        /// </summary>
        public void DeactivateRandomRequiredItems()
        {
            if (!_activeRequiredItems.Any()) return;

            DeactivateItems(_activeRequiredItems, "required");
        }

        /// <summary>
        /// Deactivates up to 3 normal (non-required) items with the same ID.
        /// </summary>
        public void DeactivateRandomNormalItemsById()
        {
            if (!_activeNormalItems.Any()) return;

            DeactivateItems(_activeNormalItems, "normal");
        }

        /// <summary>
        /// Deactivates up to 3 items from the given list, based on matching item ID.
        /// </summary>
        /// <param name="itemList">List of items to deactivate from.</param>
        /// <param name="itemType">Type of items (for logging purposes).</param>
        private void DeactivateItems(List<Item> itemList, string itemType)
        {
            var randomItem = itemList[Random.Range(0, itemList.Count)];
            int itemId = randomItem.ItemId;

            var itemsToDeactivate = itemList
                .Where(item => item.ItemId == itemId)
                .Take(3)
                .ToList();

            foreach (var item in itemsToDeactivate)
            {
                _levelManager.UpdateItemCollection(item);

                _particleManager.PlayParticleAtPoint(_itemDestroyParticleKey,
                    item.transform.position);

               // GlobalBinder.singleton.AudioManager.PlaySound(_itemDestroyClipKey);

                item.gameObject.SetActive(false);
                _activeItems.Remove(item);
                itemList.Remove(item);
            }

            Debug.Log($"Deactivated {itemsToDeactivate.Count} {itemType} items with ID: {itemId}");
        }

        /// <summary>
        /// Draws the spawn area in the editor for visualization purposes.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = new Vector3(
                (_horizontalSpawnRange.x + _horizontalSpawnRange.y) / 2,
                _spawnHeight,
                (_verticalSpawnRange.x + _verticalSpawnRange.y) / 2
            );
            Vector3 size = new Vector3(
                Mathf.Abs(_horizontalSpawnRange.y - _horizontalSpawnRange.x),
                 Mathf.Abs(_upwardSpawnRange.y - _upwardSpawnRange.x),
                Mathf.Abs(_verticalSpawnRange.y - _verticalSpawnRange.x)
            );
            Gizmos.DrawWireCube(center, size);
        }
    }
}