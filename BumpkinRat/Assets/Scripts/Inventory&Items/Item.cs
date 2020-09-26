using System;
using System.Collections.Generic;

[Serializable]
public class Item: Identifiable, IComparable<Item>
{    
    public int itemId;
    public string itemName;
    public string DisplayName => itemName.ToDisplay();
    public int value;

    public string meshPath;
    public string texturePath;

    public string identifier => itemName;

    public virtual bool CraftableItem(GameData data)
    {
        return data.HasRecipe(this);
    }

    public int CompareTo(Item other)
    {
        return itemId.CompareTo(other.itemId);
    }
}

[Serializable]
public class Recipe: Identifiable
{
    public int outputId;
    public string outputName;
    public List<RecipeIngredient> ingredients;
    public string identifier => outputName;

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

    public string[] tags;
    public RecipeIngredient(string id, int amnt) {
        ID = id;
        amount = amnt;
        tags = Array.Empty<string>();
    }
}

public interface Identifiable
{
    string identifier { get; }
}


