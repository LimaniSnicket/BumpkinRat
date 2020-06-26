using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlantingManager : MonoBehaviour, IComparer<PlantingSpace>
{
    private static PlantingManager plantGod;
    private static List<PlantingSpace> planting_spaces;

    public static List<PlantingSpace> PlantingSpaces { get {
            if (planting_spaces == null) { planting_spaces = new List<PlantingSpace>(); }
            return planting_spaces;
        } }

    private void Awake()
    {
        Debug.Log("Initialize Plant Manager. Bow down to your plant god, peasant");
        if(plantGod == null) { plantGod = this; } else { Destroy(this); }
    }

    public int Compare(PlantingSpace x, PlantingSpace y)
    {
        if (x == null && y == null) { return 0; }
        if (x == null && y != null) { return 1; }
        if (x != null && y == null) { return -1; }
        if (Mathf.Abs(Vector3.Distance(x.transform.position, transform.position))
            <= Mathf.Abs(Vector3.Distance(y.transform.position, transform.position)))
        { return -1; }
        return 1;
    }

    public static void RegisterPlantingSpace(PlantingSpace space, bool add)
    {
        if(planting_spaces == null) { planting_spaces = new List<PlantingSpace>(); }
        if (add)
        {
            if (!planting_spaces.Contains(space))
            {
                planting_spaces.Add(space);
            }
        } else
        {
            if (planting_spaces.Contains(space))
            {
                planting_spaces.Remove(space);
            }
        }
    }
}
