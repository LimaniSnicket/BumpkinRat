using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour, IDistributeItems
{
    public IItemDistribution ItemDistributor { get; set; }

    public Transform[] spawnPositions;

    public GameObject spawnPrefab;

    public GameObject customerQueueHeadPosition;

    private void Start()
    {
        ItemDistributor = new ItemPlacer(this);
        //Distributor.spawnPrefab = true;
    }

    void OnInventoryButtonPressed(object source, ItemEventArgs args)
    {
        SpawnOnWorkbench(args.ItemToPass);
    }

    public void SpawnOnWorkbench(Item item)
    {
        ItemDrop toDrop = new ItemDrop(item, 1);

        ItemDistributor.AddItemToDrop(toDrop);

        ItemDistributor.Distribute();
    }

    private void OnDestroy()
    {
        //InventoryButton.InventoryButtonPressed -= OnInventoryButtonPressed;
    }

}
