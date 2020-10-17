using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DatabaseContainer : MonoBehaviour
{
    public string itemDataPath, plantDataPath;
    private static DatabaseContainer database;
    public static GameData gameData { get; private set; }

    public GameObject basicItemPrefab;

    private void OnEnable()
    {
        if (database == null) { database = this; } else { Destroy(this); }
        InitializeData();
    }
     void InitializeData()
    {
        gameData = itemDataPath.InitializeFromJSON<GameData>();
        //gameData.plantData = plantDataPath.InitializeFromJSON<PlantDataStorage>();
    }

    public static GameObject InstantiateItem(Vector3 position)
    {
        GameObject createItem = Instantiate(database.basicItemPrefab);
        createItem.transform.position = position;
        return createItem;
    }
}

[Serializable]
public class GameData
{
    [SerializeField] List<Item> ItemData;
    [SerializeField] List<Recipe> RecipeData;
    //public PlantDataStorage plantData;

    private Dictionary<string, Item> ItemMap => ItemData.ToDictionary(i => i.itemName);
    private Dictionary<string, Recipe> item_recipe_lookup = new Dictionary<string, Recipe>();

    public Dictionary<int, Item> ItemIdLookup => ItemData.ToDictionary(i => i.itemId);
    public Dictionary<int, Recipe> RecipeIdLookup => RecipeData.ToDictionary(i => i.recipeId);

    public Dictionary<Item, List<Recipe>> ItemToRecipeLookup => 
        ItemData.Where(i => i.craftable).
        ToDictionary(r => r, r => GetRecipesOfOutputId(r.itemId));


    public List<Item> GetItemData() { 
        return ItemData; 
    }

    public void InitializeLookupTables()
    {
        //InitializeDataLookupsByID(ItemData, item_map);
        //InitializeDataLookupsByID(RecipeData, item_recipe_lookup);
    }

    List<Recipe> GetRecipesOfOutputId(int id)
    {
        return RecipeData.Where(r => r.outputId == id).ToList();
    }

    public bool HasRecipe(Item i)
    {
        if(RecipeIdLookup == null)
        {
            return false;
        }

        return GetRecipesOfOutputId(i.itemId).Count > 0;

    }

    public Recipe GetRecipe(int index)
    {
        return RecipeIdLookup[index];
    }

    public Recipe GetRecipe(string lookup)
    {
        if (!item_recipe_lookup.ContainsKey(lookup)) { return new Recipe(); }
        return item_recipe_lookup[lookup];
    }

    public Item GetItem(string itemID)
    {
        try
        {
            return ItemMap[itemID];
        } catch (KeyNotFoundException)
        {
            return new Item { itemName = $"{itemID}_invalid", value = -1 };
        }
    }

    public Item GetItem(int id)
    {
        if (ItemIdLookup == null || !ItemIdLookup.ContainsKey(id))
        {
            return new Item { itemId = id, itemName = $"invalid_item_{id}", value = -1 };
        }

        return ItemIdLookup[id];
    }

    public bool ValidItem(string itemID)
    {
        return ItemMap.ContainsKey(itemID);
    }

   public void SetItemList(List<Item> items)
    {
        if (Application.isEditor)
        {
            ItemData = new List<Item>(items);
        }
    }

    public void AddToGameData(Item i)
    {
        if (Application.isEditor && !ValidItem(i.itemName))
        {
            ItemData.Add(i);
            ItemMap.Add(i.itemName, i);
        }
    }

    public void AddToGameData(Recipe r)
    {
        if (Application.isEditor && !item_recipe_lookup.ContainsKey(r.outputName))
        {
            RecipeData.Add(r);
            item_recipe_lookup.Add(r.outputName, r);
        }
    }
}

[Serializable]
public struct PlantDataStorage
{
    public List<string> plantNames;
}
