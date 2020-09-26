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
        gameData.InitializeLookupTables();
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
    public PlantDataStorage plantData;

    private Dictionary<string, Item> ItemMap => ItemData.ToDictionary(i => i.itemName);
    private Dictionary<string, Recipe> item_recipe_lookup = new Dictionary<string, Recipe>();

    public Dictionary<int, Item> ItemIdLookup => ItemData.ToDictionary(i => i.itemId);


    public List<Item> GetItemData() { 
        return ItemData; 
    }

    public void InitializeLookupTables()
    {
        //InitializeDataLookupsByID(ItemData, item_map);
        InitializeDataLookupsByID(RecipeData, item_recipe_lookup);
    }

    public bool HasRecipe(Item i)
    {
        if(item_recipe_lookup == null) { return false; }
        return item_recipe_lookup.ContainsKey(i.itemName);
    }

  public Recipe GetRecipe(int index)
    {
        return item_recipe_lookup.ElementAt(Mathf.Clamp(index, 0, item_recipe_lookup.Count - 1)).Value;
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

    void InitializeDataLookupsByID<T>(List<T> l, Dictionary<string, T> d) where T : Identifiable
    {
        if (l.ValidList())
        {
            foreach(var listObj in l)
            {
                string id = listObj.identifier;
                d.Add(id, listObj);
            }
        }
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
