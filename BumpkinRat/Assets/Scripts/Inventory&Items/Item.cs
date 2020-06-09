using System;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;

[Serializable]
public class Item: Identifiable
{
    public string ID;
    public string display;
    public int value;
    public bool craftable;

    public string identifier => ID;
    public bool CraftableItem(GameData data)
    {
        return data.HasRecipe(this);
    }
}

[Serializable]
public class Recipe: Identifiable
{
    public string outputID;
    public List<RecipeIngredient> ingredients;

    public string identifier => outputID;

    public bool Craftable(Inventory inventory)
    {
        if (!ingredients.ValidList()) { return false; }
       foreach(RecipeIngredient i in ingredients)
        {
            if(!inventory.CheckQuantity(i.ID.GetItem(), i.amount)) { return false; }
        }
        return true;
    }
}

[Serializable]
public struct RecipeIngredient
{
    public string ID;
    public int amount;
    public RecipeIngredient(string id, int amnt) {
        ID = id;
        amount = amnt;
    }
}

public interface Identifiable
{
    string identifier { get; }
}
