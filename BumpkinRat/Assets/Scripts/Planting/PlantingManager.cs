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
    public static List<PlantingSpace> NearbyPlantingSpaces;

    public static List<string> plantNames;

    private void Awake()
    {
        Debug.Log("Initialize Plant Manager. Bow down to your plant god, peasant");
        if(plantGod == null) { plantGod = this; } else { Destroy(this); }
        NearbyPlantingSpaces = new List<PlantingSpace>();
    }

    public static PlantingSpace NearestPlantingSpace
    {
        get
        {
            if (NearbyPlantingSpaces.ValidList())
            {
                return NearbyPlantingSpaces[0];
            }
            return null;
        }
    }

    public int Compare(PlantingSpace x, PlantingSpace y)
    {
        if (x == null && y == null) { return 0; }
        if (x == null && y != null) { return 1; }
        if (x != null && y == null) { return -1; }
        if (Mathf.Abs(Vector3.Distance(x.transform.position, PlayerBehavior.playerPosition))
            <= Mathf.Abs(Vector3.Distance(y.transform.position, PlayerBehavior.playerPosition)))
        { return -1; }
        return 1;
    }

    public static void RegisterPlantingSpace(PlantingSpace space, bool add)
    {
        if(planting_spaces == null) { planting_spaces = new List<PlantingSpace>(); }
        planting_spaces.HandleInstanceObjectInList(space, add);
    }

    public static void RegisterNearbyPlantingSpace(PlantingSpace space, bool add)
    {
        if(NearbyPlantingSpaces == null) { NearbyPlantingSpaces = new List<PlantingSpace>(); }
        NearbyPlantingSpaces.HandleInstanceObjectInList(space, add);
    }

    public static string GetRandomPlant()
    {
        if (plantNames.ValidList())
        {
            int r = UnityEngine.Random.Range(0, plantNames.Count);
            return plantNames[r];
        }
        return "";
    }
}
