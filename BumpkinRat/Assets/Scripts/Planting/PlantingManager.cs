using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlantingManager : MonoBehaviour, IComparer<PlantingSpace>
{
    private static PlantingManager plantGod;
    private static List<PlantingSpace> plantingSpaces;

    // change to available planting spaces?
    public static List<PlantingSpace> PlantingSpaces {
        get 
        {
            if (plantingSpaces == null) 
            { 
                plantingSpaces = new List<PlantingSpace>(); 
            }
            return plantingSpaces;
        } 
    }

    public static List<PlantingSpace> NearbyPlantingSpaces { get; private set; } = new List<PlantingSpace>();

    public static List<string> plantNames = new List<string> { "plant a", "plant b" };

    private void Awake()
    {
        Debug.Log("Initialize Plant Manager. Bow down to your plant god, peasant");
        if (plantGod == null) { 
            plantGod = this; 
        } 
        else 
        { 
            Destroy(this); 
        }
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
        if (Mathf.Abs(Vector3.Distance(x.transform.position, PlayerBehavior.PlayerPosition))
            <= Mathf.Abs(Vector3.Distance(y.transform.position, PlayerBehavior.PlayerPosition)))
        { return -1; }
        return 1;
    }

    public static void RegisterPlantingSpace(PlantingSpace space, bool add)
    {
        plantingSpaces.HandleInstanceObjectInList(space, add);
    }

    public static void RegisterNearbyPlantingSpace(PlantingSpace space)
    {
        NearbyPlantingSpaces.HandleInstanceObjectInList(space, true);
    }

    public static void RemovePlantingSpaceFromNearbySpaces(PlantingSpace space)
    {
        NearbyPlantingSpaces.HandleInstanceObjectInList(space, false);
    }

    public static string GetRandomPlant()
    {
        if (plantNames.ValidList())
        {
            int r = 0;//UnityEngine.Random.Range(0, DatabaseContainer.gameData.plantData.plantNames.Count);
            return plantNames[r];
        }
        return "Default Plant";
    }
}
