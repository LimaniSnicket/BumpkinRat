using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlantingManager : MonoBehaviour
{
    private static PlantingManager plantGod;
    private static List<PlantingSpace> allPlantingSpaces;

    private void Awake()
    {
        Debug.Log("Initialize Plant Manager. Bow down to your plant god, peasant");
        if(plantGod == null) { plantGod = this; } else { Destroy(this); }
        allPlantingSpaces = new List<PlantingSpace>();
    }

    public static void RegisterPlantingSpace(PlantingSpace space, bool add)
    {
        if(allPlantingSpaces == null) { allPlantingSpaces = new List<PlantingSpace>(); }
        if (add)
        {
            if (!allPlantingSpaces.Contains(space))
            {
                allPlantingSpaces.Add(space);
            }
        } else
        {
            if (allPlantingSpaces.Contains(space))
            {
                allPlantingSpaces.Remove(space);
            }
        }
    }
}
