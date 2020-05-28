using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class DatabaseContainer : MonoBehaviour
{
    public string itemDataPath;
    private static DatabaseContainer database;
    public GameData gameData;

    private void OnEnable()
    {
        if (database == null) { database = this; } else { Destroy(this); }
        InitializeData();
    }
     void InitializeData()
    {
        gameData = itemDataPath.InitializeFromJSON<GameData>();
        gameData.InitializeLookupTables();
    }
}

[Serializable]
public class GameData
{
    [SerializeField] List<Item> ItemData;
    [SerializeField] List<Recipe> RecipeData;

    private Dictionary<string, Item> item_map = new Dictionary<string, Item>();
    private Dictionary<string, Recipe> item_recipe_lookup = new Dictionary<string, Recipe>();

    public void InitializeLookupTables()
    {
        InitializeDataLookupsByID(ItemData, item_map);
        InitializeDataLookupsByID(RecipeData, item_recipe_lookup);
    }

    public Item GetItem(string itemID)
    {
        try
        {
            return item_map[itemID];
        } catch (KeyNotFoundException)
        {
            return new Item { ID = "invalid_item", display = "Invalid Item", value = -1 };
        }
    }

    public bool ValidItem(string itemID)
    {
        return item_map.ContainsKey(itemID);
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
}
