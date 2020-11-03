using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data.SqlTypes;

public class ItemPlacer : ItemDistributor
{
    IDistributeItems<ItemPlacer> Placer { get; set; }
    private List<Vector3> placementPositions;

    List<OccupiedPosition> occupiedPositions = new List<OccupiedPosition>();

    public bool spawnPrefab;
    bool CanSpawnPrefab => spawnPrefab && prefab != null;
    GameObject prefab;

    public ItemPlacer(IDistributeItems<ItemPlacer> placer)
    {
        Placer = placer;
        clearOnDistribute = true;
        RecipeProgressTracker.onRecipeCompleted.AddListener(PlaceFromRecipe);
    }

    public void SetPrefab(GameObject p)
    {
        prefab = p;
    }

    public List<Vector3> GetPlacementPositions()
    {
        if(placementPositions == null)
        {
            placementPositions = new List<Vector3>();
        }

        return placementPositions;
    }


    public void SetPlacementPositions(params Vector3[] vects)
    {
        placementPositions = new List<Vector3>(vects);
        int index = 0;
        Vector3 eulers = Vector3.zero;
        occupiedPositions = placementPositions.Select(p => new OccupiedPosition 
        { position = p,
          eulers = eulers + (Vector3.up * 90 * index),
          positionIndex = index++
        }).ToList();
    }

    int Next()
    {
        if (!occupiedPositions.ValidList())
        {
            return -1;
        }

        foreach(OccupiedPosition pos in occupiedPositions)
        {
            if (!pos.Occupied)
            {
                return pos.positionIndex;
            }
        }

        return -1;
    }

    public override void Distribute()
    {
        if (!ValidItemDropData)
        {
            return;
        }

        for(int i = 0; i < ItemsToDrop.Count; i++)
        {
            int next = Next();
            if (next > -1)
            {
                ItemObject itemObject = ItemsToDrop[i].ToDrop.SpawnFromPrefabPath();
                occupiedPositions[next].Occupy(itemObject);
            } else
            {
                Debug.LogWarning("Can no longer spawn items here!");
                break;
            }
        }

        if (clearOnDistribute)
        {
            ItemsToDrop.Clear();
        }
    }

    void PlaceFromRecipe(Recipe recipe)
    {
        Item i = recipe.GetOutputItem();
        ItemDrop drop = ItemDrop.SetFromItem(i);
        ItemsToDrop.Add(drop);
        Distribute();
    }
}

public class OccupiedPosition
{
    public Vector3 position;

    public Vector3 eulers;
    public bool Occupied { get; set; }

    public int positionIndex;

    public void Occupy(Transform t)
    {
        Occupied = true;
        t.position = position;
    }

    public void Occupy(ItemObject itemObject)
    {
        Occupied = true;

        //GameObject instantiate = GameObject.Instantiate(itemObject.gameObject);

        float yOffset = itemObject.GetComponent<MeshFilter>().mesh.bounds.extents.y;

        itemObject.transform.position = position + Vector3.up * yOffset;

        itemObject.transform.rotation = Quaternion.Euler(eulers);

        itemObject.Occupied = this;

        //instantiate.GetComponent<ItemObject>().Occupied = this;
    }

    public static void Release(IOccupyPositions occupying)
    {
        if(occupying.Occupied != null)
        {
            occupying.Occupied.Occupied = false;
            occupying.Occupied = null;
        }
    }
}


public interface IOccupyPositions
{
    OccupiedPosition Occupied { get; set; }

}
