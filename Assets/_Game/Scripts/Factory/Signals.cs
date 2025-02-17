using UnityEngine;
using _Main.Items;

namespace _Main.Signals
{
    public class ItemSelectedSignal
    {
        public Item Item { get; }
        public ItemSelectedSignal(Item item) => Item = item;
    }

    public class ItemCollectedSignal
    {
        public Item Item { get; }
        public ItemCollectedSignal(Item item) => Item = item;
    }

    public class ItemsMatchedSignal
    {
        public System.Collections.Generic.List<Item> Items { get; }
        public ItemsMatchedSignal(System.Collections.Generic.List<Item> items) => Items = items;
    }

    public class LevelCompleteSignal
    {
    }

    public class LevelFailedSignal
    {
    }
}