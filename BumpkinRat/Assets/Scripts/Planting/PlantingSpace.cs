using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class PlantingSpace : MonoBehaviour
{
    Mesh mesh;
    Bounds bounds;

    public int rows, columns;
    public PlanterTile[,] planterTiles;

    private void OnEnable()
    {
        PlantingManager.RegisterPlantingSpace(this, true);
        mesh = GetComponent<MeshFilter>().mesh;
        bounds = mesh.bounds;
        GeneratePlanterTiles();
    }

    (int, int) planterDimensions
    {
        get
        {
            rows = Mathf.Max(1, rows);
            columns = Mathf.Max(1, columns);
            int tileRows = (int)((bounds.extents.y * 2) / 0.25f);
            int tileColumns = (int)((bounds.extents.x * 2) / 0.25f);
            planterTiles = new PlanterTile[tileRows, tileColumns];
            return (tileRows, tileColumns);
        }
    }

    (int, int, Vector3) PreparePlantGeneration()
    {
        Vector3 spawnStart = new Vector3(-bounds.extents.x + 0.25f / 2, transform.position.y, bounds.extents.z - 0.25f / 2);
        return (planterDimensions.Item1, planterDimensions.Item2, spawnStart);
    }

    void GeneratePlanterTiles()
    {
        (int, int, Vector3) dimensions = PreparePlantGeneration();
        
        for (int i =0; i< dimensions.Item1; i++)
        {
            for (int j =0; j< dimensions.Item2; j++)
            {
                Vector3 sp = dimensions.Item3 + new Vector3(0.25f * i, 0, -0.25f * j);
                PlanterTile t = new PlanterTile(i, j);
                t.SetBounds(sp);
                planterTiles[i, j] = t;
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
}

[Serializable]
public class PlanterTile
{
    [SerializeField] float length, width = 0.25f;
    public bool occupied;
    public Plant planted;
    public Vector2 tileDimensions => new Vector2(length, width);
    [SerializeField] int row, column;
    public (int, int) rowColumnIndex => (row, column);

    public Bounds tileBounds { get; private set; }

    public PlanterTile() { }
    public PlanterTile(int r, int c)
    {
        row = r;
        column = c;
    }

    public bool ValidTile((int,int) dim)
    {
        return dim.Item1 <= row && dim.Item2 <= column;
    }

    public void SetBounds(Vector3 center, bool calculate = false)
    {
        Vector3 set = calculate ? center + new Vector3(width * row, 0, -length * column) : center;
        tileBounds = new Bounds(set, new Vector3(width, 1, length));
        Debug.Log(tileBounds.extents);
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
