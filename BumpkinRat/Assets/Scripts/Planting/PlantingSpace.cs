using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PlantingSpace : MonoBehaviour, IComparer<PlanterTile>
{
    Mesh mesh;
    Bounds bounds;

    public float tileDimension = 0.25f;
    public PlanterTile[,] planterTiles;
    [SerializeField] List<PlanterTile> planterTileList;

    private void OnEnable()
    {
        PlantingManager.RegisterPlantingSpace(this, true);
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        planterTileList = new List<PlanterTile>();
        GeneratePlanterTiles();
    }

    (int, int) planterDimensions
    {
        get
        {
            tileDimension = Mathf.Max(tileDimension, 0.1f);
            int tileRows = (int)((bounds.extents.y * 2) / tileDimension);
            int tileColumns = (int)((bounds.extents.x * 2) / tileDimension);
            planterTiles = new PlanterTile[tileRows, tileColumns];
            return (tileRows, tileColumns);
        }
    }

    (int, int, Vector3) PreparePlantGeneration()
    {
        Vector3 s = transform.worldToLocalMatrix * bounds.min;
        Vector3 spawnStart = new Vector3(s.x + tileDimension/2, transform.position.y, s.z - tileDimension/2);
        return (planterDimensions.Item1, planterDimensions.Item2, spawnStart);
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
                planterTiles[i, j] = t;
                planterTileList.Add(t);
            }
        }
    }

    void GeneratePlanterTiles(List<PlanterTile> savedTiles)
    {
        (int, int, Vector3) dimensions = PreparePlantGeneration();

        foreach (PlanterTile tile in savedTiles)
        {
            if (tile.ValidTile(planterDimensions))
            {
                tile.SetBounds(dimensions.Item3, true);
                planterTiles[tile.rowColumnIndex.Item1, tile.rowColumnIndex.Item2] = tile;
                planterTileList.Add(tile);
            }
            else
            {
                Debug.LogWarning("A Loaded Planter Tile did not fit in dimensions of its Planting Space.");
            }
        }
    }

    private void OnDisable()
    {
        PlantingManager.RegisterPlantingSpace(this, false);
    }

    void SortPlanterTiles()
    {
        if (planterTileList.ValidList())
        {
            planterTileList.Sort(Compare);
        }
    }

    public int Compare(PlanterTile x, PlanterTile y)
    {
        if (x == null && y == null) { return 0; }
        if (x == null && y != null) { return 1; }
        if (x != null && y == null) { return -1; }
        if (Mathf.Abs(Vector3.Distance(x.tileBounds.center, transform.position))
            <= Mathf.Abs(Vector3.Distance(y.tileBounds.center, transform.position)))
        { return -1; }
        return 1;
    }
}

[Serializable]
public class PlanterTile
{
    [SerializeField] float length, width = 0.25f;
    public bool occupied;
    public Plant planted;
    public (float, float) tileDimensions => (length, width);
    [SerializeField] int row, column;
    public (int, int) rowColumnIndex => (row, column);

    public Bounds tileBounds { get; private set; }
    [SerializeField] Vector3 center;

    public PlanterTile() { }
    public PlanterTile(int r, int c)
    {
        row = r;
        column = c;
    }

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
        tileBounds = new Bounds(set, new Vector3(width, 1, length));
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

[Serializable]
public class Plant
{
    string name;
    public Plant() { }
    public Plant(string s)
    {
        name = s;
    }
}
