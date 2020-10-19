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
        occupiedPositions = placementPositions.Select(p => new OccupiedPosition { position = p, positionIndex = index++ }).ToList();
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
                GameObject g = GameObject.Instantiate(prefab);
                occupiedPositions[next].Occupy(g.transform);
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
}

public class OccupiedPosition: INullable
{
    public Vector3 position;
    public bool Occupied { get; set; }

    public int positionIndex;

    public bool IsNull => isNull;

    bool isNull;

    public void Occupy(Transform t)
    {
        Occupied = true;
        t.position = position;
    }

    public static OccupiedPosition Null => new OccupiedPosition { isNull = true };
}
