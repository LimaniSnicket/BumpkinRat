using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PlantingSpace : MonoBehaviour, IComparer<PlanterTile>
{
    private Mesh mesh;
    private Bounds bounds;

    public float tileDimension = 0.25f;
    [SerializeField] 
    private List<PlanterTile> planterTiles;

    private void OnEnable()
    {
        PlantingManager.RegisterPlantingSpace(this, true);
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        planterTiles = new List<PlanterTile>();
        GeneratePlanterTiles();
    }
    
    private (int, int) PlanterDimensions
    {
        get
        {
            tileDimension = Mathf.Max(tileDimension, 0.1f);
            int tileRows = (int)((bounds.extents.y * 2) / tileDimension);
            int tileColumns = (int)((bounds.extents.x * 2) / tileDimension);
            return (tileRows, tileColumns);
        }
    }

    private (int, int, Vector3) PreparePlantGeneration()
    {
        Vector3 s = transform.worldToLocalMatrix * bounds.min;
        Vector3 spawnStart = new Vector3(s.x + tileDimension/2, transform.position.y, s.z - tileDimension/2);
        return (PlanterDimensions.Item1, PlanterDimensions.Item2, spawnStart);
    }

    void GeneratePlanterTiles()
    {
        (int, int, Vector3) dimensions = PreparePlantGeneration();
        
        for (int i =0; i< dimensions.Item1; i++)
        {
            for (int j =0; j< dimensions.Item2; j++)
            {
                Vector3 sp = dimensions.Item3 + new Vector3(tileDimension * i, 0, -tileDimension * j);
                PlanterTile t = new PlanterTile((i, j), (tileDimension, tileDimension));
                t.SetBounds(sp);
                GameObject who_is_she = new GameObject("Who Is She");
                who_is_she.transform.position = sp;
                planterTiles.Add(t);
            }
        }
    }

    void GeneratePlanterTiles(List<PlanterTile> savedTiles)
    {
        (int, int, Vector3) dimensions = PreparePlantGeneration();

        foreach (PlanterTile tile in savedTiles)
        {
            if (tile.ValidTile(PlanterDimensions))
            {
                tile.SetBounds(dimensions.Item3, true);
                planterTiles.Add(tile);
            }
            else
            {
                Debug.LogWarning("A Loaded Planter Tile did not fit in dimensions of its Planting Space.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerBehavior>())
        {
            PlantingManager.RegisterNearbyPlantingSpace(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerBehavior>())
        {
            PlantingManager.RemovePlantingSpaceFromNearbySpaces(this);
        }
    }

    private void OnDisable()
    {
        PlantingManager.RegisterPlantingSpace(this, false);
    }

    void SortPlanterTiles()
    {
        if (planterTiles.ValidList())
        {
            planterTiles.Sort(Compare);
        }
    }

    public int Compare(PlanterTile x, PlanterTile y)
    {
        if (x == null && y == null) { return 0; }
        if (x == null && y != null) { return 1; }
        if (x != null && y == null) { return -1; }
        if(x.occupied == false && y.occupied == true) { return -1; }
        if (x.occupied == true && y.occupied == false) { return 1; }
        return 0;
    }

    public void Plant(string name)
    {
        SortPlanterTiles();
        planterTiles[0].Plant(name);
    }
}

[Serializable]
public class PlanterTile
{
    [SerializeField] float length, width = 0.25f;
    [SerializeField] int row, column;

    public bool occupied;
    public Plant planted;
    public (float, float) TileDimensions => (length, width);
    public (int, int) RowColumnPosition => (row, column);

    public Bounds TileBounds { get; private set; }

    public PlanterTile((int, int) pos, (float, float) dimensions)
    {
        row = pos.Item1;
        column = pos.Item2;
        length = dimensions.Item1;
        width = dimensions.Item2;
    }

    public bool ValidTile((int,int) dim)
    {
        return dim.Item1 <= row && dim.Item2 <= column;
    }

    public void SetBounds(Vector3 center, bool calculate = false)
    {
        Vector3 set = calculate ? center + new Vector3(width * row, 0, -length * column) : center;
        TileBounds = new Bounds(set, new Vector3(width, 1, length));
    }

    public void Plant(string plantName)
    {
        if (occupied) {
            Debug.Log("There's already something planted here, STOOPID");
            return;
        }
        occupied = true;
        planted = new Plant(plantName);
        Debug.Log("Planting " + plantName);
    }
}

