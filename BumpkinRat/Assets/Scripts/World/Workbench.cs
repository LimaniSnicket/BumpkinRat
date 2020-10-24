using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Workbench : MonoBehaviour, IDistributeItems<ItemPlacer>
{
    public ItemPlacer ItemDistributor { get; set; }
    public List<ItemDrop> ItemDropData { get; set; } = new List<ItemDrop>();

    public Transform[] spawnPositions;

    public GameObject spawnPrefab;


    private void Start()
    {
        ItemDistributor = new ItemPlacer(this);
        ItemDistributor.spawnPrefab = true;
        ItemDistributor.SetPlacementPositions(spawnPositions.Select(t => t.position).ToArray());

        InventoryButton.InventoryButtonPressed += OnInventoryButtonPressed;

        SpawnItemObject(spawnPrefab);
    }

    void OnInventoryButtonPressed(object source, InventoryButtonArgs args)
    {
        SpawnOnWorkbench(args.ItemToPass);
    }

    public void AddItemToDrop(Item item)
    {
        ItemDropData.Add(ItemDrop.SetFromItem(item));
    }

    public void SpawnOnWorkbench(Item item)
    {
        AddItemToDrop(item);

        ItemDistributor.SetItemsToDrop(ItemDropData);

        ItemDistributor.Distribute();

        ItemDropData.Clear();
    }

    public void SpawnItemObject(GameObject obj)
    {
        ItemDistributor.SetPrefab(obj);
    }

    private void OnDestroy()
    {
        InventoryButton.InventoryButtonPressed -= OnInventoryButtonPressed;
    }

}

