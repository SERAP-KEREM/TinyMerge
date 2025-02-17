using UnityEngine;
using Zenject;
using _Main.Data;
using _Main.Items;
using _Main.Tiles;
using _Main.Management;
using _Main.Signals;
using _Main.Factories;
using _Main.Settings;
using System.Collections.Generic;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private List<Item> itemPrefabs;

    public override void InstallBindings()
    {
        // Bind GameData
        Container.Bind<GameData>()
            .AsSingle()
            .NonLazy();

        // Bind ItemManager settings
        Container.Bind<ItemManagerSettings>()
            .AsSingle()
            .NonLazy();

        // Bind TileManager settings
        Container.Bind<TileManagerSettings>()
            .AsSingle()
            .NonLazy();

        // Bind LevelConfig
        Container.Bind<LevelConfig>()
            .FromInstance(levelConfig)
            .AsSingle();

        // Bind Managers
        Container.BindInterfacesAndSelfTo<ItemManager>()
            .AsSingle()
            .NonLazy();

        Container.BindInterfacesAndSelfTo<TileManager>()
            .AsSingle()
            .NonLazy();

        // Bind Item Factories for each item type
        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            int itemId = i; // Capture the index
            Container.BindFactory<Vector3, Item, PlaceholderFactory<Vector3, Item>>()
                .WithId(itemId)
                .FromPoolableMemoryPool<Vector3, Item, ItemPool>(poolBinder => poolBinder
                    .WithInitialSize(10)
                    .FromComponentInNewPrefab(itemPrefabs[itemId])
                    .UnderTransformGroup("Items"));
        }
    }

    class ItemPool : MonoPoolableMemoryPool<Vector3, IMemoryPool, Item>
    {
    }
}