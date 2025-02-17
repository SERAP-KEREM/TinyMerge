using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Zenject;
using System.Linq;
using _Main.Tiles;
using _Main.Items;
using _Main.Signals;
using _Main.Settings;

namespace _Main.Management
{
    public class TileManager : IInitializable
    {
        private const float MATCH_ANIMATION_DURATION = 0.5f;
        private const float MATCH_RISE_HEIGHT = 2f;

        private readonly List<Tile> _tiles = new List<Tile>();
        private readonly Tile.Factory _tileFactory;
        private readonly SignalBus _signalBus;
        private readonly TileManagerSettings _settings;


        public TileManager(
           TileManagerSettings settings,
           Tile.Factory tileFactory)
        {
            _settings = settings;
            _tileFactory = tileFactory;
        }

        public void Initialize()
        {
            CreateTileGrid();
        }

        private void CreateTileGrid()
        {
            for (int x = 0; x < _settings.gridSize.x; x++)
            {
                for (int z = 0; z < _settings.gridSize.y; z++)
                {
                    Vector3 position = new Vector3(
                        x * _settings.tileSpacing.x,
                        _settings.tileHeight,
                        z * _settings.tileSpacing.y
                    );

                    var tile = _tileFactory.Create(position);
                    _tiles.Add(tile);
                }
            }
        }

        public void HandleMatch(List<Item> matchedItems)
        {
            if (!matchedItems.Any()) return;

            var tiles = matchedItems.Select(item => item.CurrentTile).ToList();
            AnimateAndDeactivateItems(matchedItems, tiles);
        }

        private void AnimateAndDeactivateItems(List<Item> items, List<Tile> tiles)
        {
            var sequence = DOTween.Sequence();
            var centerPos = CalculateCenterPosition(items);

            foreach (var item in items)
            {
                sequence.Join(CreateItemMatchAnimation(item, centerPos));
            }

            sequence.OnComplete(() =>
            {
                _signalBus.Fire(new ItemsMatchedSignal(items));
                DeactivateItems(items, tiles);
            });
        }

        private Sequence CreateItemMatchAnimation(Item item, Vector3 centerPos)
        {
            return DOTween.Sequence()
                .Append(item.transform.DOMoveY(item.transform.position.y + MATCH_RISE_HEIGHT, MATCH_ANIMATION_DURATION)
                    .SetEase(Ease.OutQuad))
                .Join(item.transform.DOMove(centerPos, MATCH_ANIMATION_DURATION)
                    .SetEase(Ease.InBack))
                .Join(item.transform.DOScale(Vector3.zero, MATCH_ANIMATION_DURATION * 0.5f)
                    .SetEase(Ease.InOutBounce));
        }

        private Vector3 CalculateCenterPosition(List<Item> items)
        {
            return items.Aggregate(Vector3.zero, (current, item) => current + item.transform.position) / items.Count;
        }

        private void DeactivateItems(List<Item> items, List<Tile> tiles)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Recycle();
                tiles[i].RemoveItem();
            }
        }

        public Tile FindEmptyTile() => _tiles.FirstOrDefault(t => !t.IsOccupied);

        [System.Serializable]
        public class Settings
        {
            public Vector2Int gridSize = new Vector2Int(4, 4);
            public Vector2 tileSpacing = new Vector2(2f, 2f);
        }
    }
}