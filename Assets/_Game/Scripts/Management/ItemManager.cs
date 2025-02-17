using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;
using _Main.Data;
using _Main.Items;
using _Main.Signals;
using System.ComponentModel;
using _Main.Factories;

namespace _Main.Management
{

    public class ItemManager : IInitializable
    {
        private readonly GameData _gameData;
        private readonly TileManager _tileManager;
        private readonly SignalBus _signalBus;
        private readonly Item.Factory _itemFactory;
        private readonly Settings _settings;
        private readonly IInstantiator _instantiator;
        private readonly Dictionary<int, ItemFactoryWithId> _itemFactories;
        private readonly DiContainer _container;

        private readonly List<Item> _activeItems = new List<Item>();
        private readonly List<Item> _requiredItems = new List<Item>();
        private readonly List<Item> _normalItems = new List<Item>();
        private readonly List<Item> _collectedItems = new List<Item>();
        private readonly List<Item> _matchCandidates = new List<Item>();

        [Inject]
        public ItemManager(
             GameData gameData,
             TileManager tileManager,
             SignalBus signalBus,
             DiContainer container,
             [Inject(Id = "ItemFactories")] List<ItemFactoryWithId> itemFactories)
        {
            _gameData = gameData;
            _tileManager = tileManager;
            _signalBus = signalBus;
            _container = container;
            _itemFactories = new Dictionary<int, ItemFactoryWithId>();

            foreach (var itemData in gameData.CurrentLevel.ItemDataList)
            {
                var factory = itemFactories.FirstOrDefault(f =>
                    _container.IsValidating ||
                    f == container.ResolveIdAll<ItemFactoryWithId>(itemData.ItemPrefab.ItemId).FirstOrDefault());

                if (factory != null)
                {
                    _itemFactories.Add(itemData.ItemPrefab.ItemId, factory);
                }
            }
        }
        public void Initialize()
        {
            SpawnInitialItems();
            SubscribeToSignals();
        }

        private void SubscribeToSignals()
        {
            _signalBus.Subscribe<ItemSelectedSignal>(OnItemSelected);
            _signalBus.Subscribe<ItemCollectedSignal>(OnItemCollected);
        }
        private int GetValidatedItemCount(int requestedCount)
        {
            return Mathf.CeilToInt(requestedCount / 3f) * 3;
        }
        private void SpawnInitialItems()
        {
            foreach (var itemData in _gameData.CurrentLevel.ItemDataList)
            {
                SpawnItemSet(itemData);
            }
        }

        private Dictionary<LevelConfig.ItemData, int> CreateItemDistribution()
        {
            return _gameData.CurrentLevel.ItemDataList.ToDictionary(
                itemData => itemData,
                itemData => GetValidatedItemCount(itemData.ItemCount)
            );
        }

        private void SpawnItemSet(LevelConfig.ItemData itemData)
        {
            if (!_itemFactories.TryGetValue(itemData.ItemPrefab.ItemId, out var factory))
            {
                Debug.LogError($"No factory found for item ID: {itemData.ItemPrefab.ItemId}");
                return;
            }

            int itemCount = GetValidatedItemCount(itemData.ItemCount);
            for (int i = 0; i < itemCount; i++)
            {
                Vector3 spawnPosition = CalculateRandomSpawnPosition();
                var item = factory.Create(spawnPosition);
                _activeItems.Add(item);
            }
        }

        private Vector3 CalculateRandomSpawnPosition()
        {
            return new Vector3(
                Random.Range(-5f, 5f),
                Random.Range(5f, 10f),
                Random.Range(-5f, 5f)
            );
        }

        private void CategorizeItems()
        {
            _requiredItems.Clear();
            _normalItems.Clear();

            foreach (var item in _activeItems)
            {
                if (IsRequiredItem(item))
                {
                    _requiredItems.Add(item);
                }
                else
                {
                    _normalItems.Add(item);
                }
            }
        }

        private bool IsRequiredItem(Item item)
        {
            return _gameData.CurrentLevel.ItemDataList
                .Any(data => data.ItemPrefab.ItemId == item.ItemId && data.IsRequired);
        }
        private void OnItemCollected(ItemCollectedSignal signal)
        {
            var item = signal.Item;
            if (!item.IsCollectable) return;

            var emptyTile = _tileManager.FindEmptyTile();
            if (emptyTile == null)
            {
                _signalBus.Fire(new LevelFailedSignal());
                return;
            }

            item.CurrentTile = emptyTile;
            emptyTile.TryPlaceItem(item);

            UpdateItemListsOnCollect(item);
            CheckForMatches();
        }

        private void UpdateItemListsOnCollect(Item item)
        {
            _activeItems.Remove(item);
            _collectedItems.Add(item);

            if (_requiredItems.Contains(item))
            {
                _requiredItems.Remove(item);
            }
            else
            {
                _normalItems.Remove(item);
            }
        }
        private void OnItemSelected(ItemSelectedSignal signal)
        {
            if (!signal.Item.IsCollectable) return;

            _matchCandidates.Add(signal.Item);
            CheckForMatches();
        }

        private void CheckForMatches()
        {
            if (_matchCandidates.Count < 3) return;

            var matches = FindMatches();
            if (matches.Any())
            {
                HandleMatches(matches);
                _matchCandidates.Clear();
            }
        }

        private List<Item> FindMatches()
        {
            return _matchCandidates
                .GroupBy(item => item.ItemId)
                .Where(group => group.Count() >= 3)
                .SelectMany(group => group)
                .ToList();
        }

        private void HandleMatches(List<Item> matches)
        {
            _tileManager.HandleMatch(matches);
            UpdateCollectionProgress();
        }

        private void UpdateCollectionProgress()
        {
            if (IsLevelComplete())
            {
                _signalBus.Fire(new LevelCompleteSignal());
            }
        }

        private bool IsLevelComplete()
        {
            return _requiredItems.All(item => _collectedItems.Contains(item));
        }

        [System.Serializable]
        public class Settings
        {
            public Bounds spawnBounds = new Bounds(Vector3.zero, new Vector3(10, 5, 10));
            public int minimumMatchCount = 3;
        }
    }
}