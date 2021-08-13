using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropFactory
{
    public List<ItemDrop> GetItemsToDrop(params (int, int)[] dropData)
    {
        return dropData.Select(drop => new ItemDrop(drop.Item1, drop.Item2)).ToList();
    }

    public List<ItemDrop> GetItemsToDrop(int[] drops)
    {
        return drops.Select(d => new ItemDrop(d, 1)).ToList();
    }
}
