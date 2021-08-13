using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class GameData
{
    [SerializeField] 
    private List<Item> itemData;

    [SerializeField]
    private List<Recipe> recipeData;
    //public PlantDataStorage plantData;

    public List<Item> ItemData => itemData;

    private Dictionary<string, Item> ItemMap => itemData.ToDictionary(i => i.itemName);

    private Dictionary<string, Recipe> item_recipe_lookup = new Dictionary<string, Recipe>();

    public Dictionary<int, Recipe> RecipeIdLookup => recipeData.ToDictionary(i => i.id);

    private IdToObjectMap<Item> idToItem;
    private IdToObjectMap<Recipe> idToRecipe;

    public void SetIdToObjectMappings()
    {
        idToItem = new IdToObjectMap<Item>(itemData.ToDictionary(i => i.itemId));
        idToRecipe = new IdToObjectMap<Recipe>(recipeData.ToDictionary(i => i.id));
    }

    List<Recipe> GetRecipesOfOutputId(int id)
    {
        return recipeData.Where(r => r.outputId == id).ToList();
    }

    public bool HasRecipe(Item i)
    {
        if(RecipeIdLookup == null)
        {
            return false;
        }

        return GetRecipesOfOutputId(i.itemId).Count > 0;

    }

    public Recipe GetRecipe(int id)
    {
        return idToRecipe.GetObjectFromMap(id);
    }

    public bool ItemExists(string itemID)
    {
        return ItemMap.ContainsKey(itemID);
    }

    public void AddToGameData(Item i)
    {
        if (Application.isEditor && !ItemExists(i.itemName))
        {
            itemData.Add(i);
            ItemMap.Add(i.itemName, i);
        }
    }

    public void AddToGameData(Recipe r)
    {
        if (Application.isEditor && !item_recipe_lookup.ContainsKey(r.outputName))
        {
            recipeData.Add(r);
            item_recipe_lookup.Add(r.outputName, r);
        }
    }
}
