using System;
using System.Collections.Generic;

[Serializable]
public class Item: Identifiable, IComparable<Item>
{    
    public int itemId;
    public string itemName;
    public string DisplayName => itemName.ToDisplay();
    public bool craftable;
    public int value;

    //public string meshPath;
    //public string texturePath;

    public string identifier => itemName;

    public int CompareTo(Item other)
    {
        return itemId.CompareTo(other.itemId);
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
    public string identifier => outputName;

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


