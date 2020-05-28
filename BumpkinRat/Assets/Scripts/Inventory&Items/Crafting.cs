using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrafter
{
    public bool crafting;
    public ItemCrafter()
    {

    }

    public bool CanCraft(Inventory i, ItemListing item)
    {
        return false;
    }
}

public class CraftingEventArgs : EventArgs
{

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