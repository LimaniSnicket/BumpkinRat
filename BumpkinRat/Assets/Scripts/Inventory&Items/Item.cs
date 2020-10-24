using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[Serializable]
public class Item: Identifiable, IComparable<Item>
{    
    public int itemId;
    public string itemName;
    public string DisplayName => itemName.ToDisplay();
    public bool craftable;
    public int value;

    public string prefabPath;
    //public string texturePath;

    public string identifier => itemName;

    public bool PrefabPathExists => prefabPath != null && prefabPath != string.Empty;

    private static Dictionary<int, ItemObject> ItemObjectCache { get; set; }

    bool CheckCache()
    {
        if(ItemObjectCache == null)
        {
            return false;
        }

        return ItemObjectCache.ContainsKey(itemId);
    }

    public int CompareTo(Item other)
    {
        return itemId.CompareTo(other.itemId);
    }

    //try and make a cache!
    public ItemObject SpawnFromPrefabPath()
    {
        if(ItemObjectCache == null)
        {
            ItemObjectCache = new Dictionary<int, ItemObject>();
        }
        try
        {

            if (CheckCache())
            {
                return ItemObjectCache[itemId];
            }

            ItemObject itemObj = (ItemObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(ItemObject));
            ItemObjectCache.Add(itemId, itemObj);
            return itemObj;

        } catch (NullReferenceException)
        {
            return new GameObject("Invalid_Item_Object", typeof(ItemObject)).GetComponent<ItemObject>();
        }
    }
}

[Serializable]
public class Recipe: Identifiable
{
    public int recipeId;
    public int outputId;
    public string outputName = "fuck this variable";

    public string recipeDescription;

    public List<RecipeIngredient> ingredients;

    public List<string> craftingSequences;
    public string identifier => outputName;

    public Dictionary<int, int> IngredientMap => ingredients.ToDictionary(k => k.id, k => k.amount);

    public bool Craftable(Inventory inventory)
    {
       if (!ingredients.ValidList()) { 
           return false; 
       }

       foreach(RecipeIngredient i in ingredients)
        {
            Item ingredient = i.id.GetItem();
            if(!inventory.CheckQuantity(ingredient, i.amount)) { return false; }
        }

        return true;
    }

    public bool RenderedUncraftable(KeyValuePair<int, int> alteredValue)
    {
        if(!IngredientMap.CollectionIsNotNullOrEmpty() || !IngredientMap.ContainsKey(alteredValue.Key))
        {
            return false;
        }
        return IngredientMap[alteredValue.Key] > alteredValue.Value;
    }

    public bool Craftable(Dictionary<int, int> ingredientsAvailable)
    {
        if (!ingredientsAvailable.CollectionIsNotNullOrEmpty() || !ingredients.ValidList())
        {
            return false;
        }

        return !ingredientsAvailable.Except(IngredientMap).Any();

    }

    public Item GetOutputItem()
    {
        return DatabaseContainer.gameData.GetItem(outputId);
    }
}

[Serializable]
public struct RecipeIngredient
{
    public int id;
    public int amount;

    public string[] tags;
    public RecipeIngredient(int id, int amnt) {
        this.id = id;
        amount = amnt;
        tags = Array.Empty<string>();
    }

    public RecipeIngredient(string id, int amnt)
    {
        this.id = amnt;
        amount = amnt;
        tags = Array.Empty<string>();
    }
}

public interface Identifiable
{
    string identifier { get; }
}

public class ItemEventArgs: EventArgs
{
    public Item ItemToPass { get; set; }

    public int AmountToPass { get; set; } = 1;
}


