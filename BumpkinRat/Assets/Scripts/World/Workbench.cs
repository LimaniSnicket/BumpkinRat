using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour, IDistributeItems<ItemPlacer>
{
    public ItemPlacer Distributor { get; set; }

    public Transform[] spawnPositions;

    public GameObject spawnPrefab;

    public GameObject customerQueueHeadPosition;

    private void Start()
    {
        Distributor = new ItemPlacer(this);
        Distributor.spawnPrefab = true;

        SpawnItemObject(spawnPrefab);
    }

    void OnInventoryButtonPressed(object source, ItemEventArgs args)
    {
        SpawnOnWorkbench(args.ItemToPass);
    }

    public void SpawnOnWorkbench(Item item)
    {
        ItemDrop toDrop = new ItemDrop(item, 1);

        Distributor.AddItemToDrop(toDrop);

        Distributor.Distribute();
    }

    public void SpawnItemObject(GameObject obj)
    {
        Distributor.SetPrefabToPlace(obj);
    }

    private void OnDestroy()
    {
        //InventoryButton.InventoryButtonPressed -= OnInventoryButtonPressed;
    }

}
