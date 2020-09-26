using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ItemPlacer : ItemDistributor
{
    IDistributeItems<ItemPlacer> Placer { get; set; }
    private List<Vector3> placementPositions;

    public ItemPlacer(IDistributeItems<ItemPlacer> placer)
    {
        Placer = placer;
    }

    public void SetItemsAndPlacements(params (string, int, Vector3)[] itemDropPlacements)
    {
        ItemsToDrop = ItemDrop.
            GetListOfItemsToDrop(itemDropPlacements.
            Select(i => (i.Item1, i.Item2)).
            ToArray());

        placementPositions = itemDropPlacements.Select(p => p.Item3).ToList();
    }

    public List<Vector3> GetPlacementPositions()
    {
        if(placementPositions == null)
        {
            placementPositions = new List<Vector3>();
        }

        return placementPositions;
    }

    Vector3 GetPlacementPosition(int index)
    {
        if (placementPositions == null)
        {
            placementPositions = new List<Vector3>();
        }

        return index >= placementPositions.Count ? Vector3.zero : placementPositions[index];
    }

    public void SetPlacementPositions(params Vector3[] vects)
    {
        placementPositions = new List<Vector3>(vects);
    }
    public override void Distribute()
    {
        if (ValidItemDropData)
        {
            return;
        }

        for(int i = 0; i < ItemsToDrop.Count; i++)
        {
            Vector3 position = GetPlacementPosition(i);
            ItemsToDrop[i].itemName.InstantiateItemInWorld(position, ItemsToDrop[i].AmountToDrop);
        }
    }
}
