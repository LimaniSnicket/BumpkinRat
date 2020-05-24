using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crafting: MonoBehaviour
{
    public string craftingData;
    public CraftingRecipeData craftingRecipeData;
    private void OnEnable()
    {
        craftingData.InitializeFromJSON(craftingRecipeData);
    }
}

[Serializable]
public class CraftingRecipeData
{
   
}

[Serializable]
public class CraftingRecipe
{
    public string recipeName;
    public List<ItemListing> neededItemListings;
}