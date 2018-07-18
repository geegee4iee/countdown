using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Coundown.Spa.ReactJs.Utils
{
    public static class CollectionExtensions
    {
        public static void Merge<Item, Item2>(this Item[] items, Item2[] item2s, Action<Item, Item2> callback)
        {
            for (int i = 0; i < items.Length; i++)
            {
                callback(items[i], item2s[i]);
            }
        }
    }
}
