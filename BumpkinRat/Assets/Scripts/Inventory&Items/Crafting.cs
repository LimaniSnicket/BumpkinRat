using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrafter
{
    public bool crafting;
    public static event EventHandler<CraftingEventArgs> CraftedItem;
    public ItemCrafter() { }

    public void CraftRecipe(Recipe r, int amt)
    {
        if(CraftedItem != null)
        {
            CraftedItem(this, new CraftingEventArgs { craftedRecipe = r, craftedAmount = amt });
        }
    }
}

public class CraftingEventArgs : EventArgs
{
    public Recipe craftedRecipe { get; set; }
    public int craftedAmount { get; set; }
}
//    public string craftingPath;
//    public CraftingData craftingData;
//    private void OnEnable()
//    {
//        if(static_crafting == null) { static_crafting = this; } else { Destroy(this); }
//        craftingData = craftingPath.InitializeFromJSON<CraftingData>();
//    }
//}

//[Serializable]
//public class CraftingData
//{
//    public IEnumerable<CraftingRecipeData> recipeData;
//    public List<CraftingRecipeData> craftable;
//}

//[Serializable]
//public class CraftingRecipeData
//{
//    public string recipeName;
//    public List<ItemListing> itemsNeeded;
//}

//public class CraftedEventArgs : EventArgs
//{
//    public string recipeName;
//}