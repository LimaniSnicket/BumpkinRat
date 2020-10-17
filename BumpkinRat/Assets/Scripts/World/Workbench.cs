using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workbench : MonoBehaviour, IDistributeItems<ItemPlacer>
{
    public ItemPlacer ItemDistributor { get; set; }
    public List<ItemDrop> ItemDropData { get; set; }

    public Vector3[] spawnPositions;

    private void Start()
    {
        ItemDistributor = new ItemPlacer(this);
        ItemDistributor.spawnPrefab = true;
        ItemDistributor.SetPlacementPositions(spawnPositions);
    }

    public void SpawnItemObject(GameObject obj)
    {
        ItemDistributor.SetPrefab(obj);
    }

    private void OnDestroy()
    {
        
    }
}

