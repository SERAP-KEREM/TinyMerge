using UnityEngine;
using Zenject;
using _Main.Items;
using _Main.Tiles;

namespace _Main.Factories
{
    public class ItemFactory : PlaceholderFactory<Vector3, Item>
    {
    }

    public class TileFactory : PlaceholderFactory<Vector3, Tile>
    {
    }

    public class ItemPool : MonoPoolableMemoryPool<Vector3, IMemoryPool, Item> { }
    public class TilePool : MonoPoolableMemoryPool<Vector3, IMemoryPool, Tile> { }
    public class ItemFactoryWithId : PlaceholderFactory<Vector3, Item>
    {
    }
}