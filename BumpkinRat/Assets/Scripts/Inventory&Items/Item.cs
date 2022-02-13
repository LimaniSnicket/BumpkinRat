using System;
using UnityEngine;

[Serializable]
public class Item: Identifiable
{
    public int itemId;

    public string itemName;
    public string DisplayName => itemName;

    public bool craftable;
    public int value;

    public string prefabName;
    //public string spritePath;

    public FocusAreaUiDetails[] focusAreaUiDetails;
    public Recipe[] craftingRecipe;

    public bool IsCraftable => craftingRecipe.CollectionIsNotNullOrEmpty();

    public int FocusAreaCount => focusAreaUiDetails.ValidArray() ? focusAreaUiDetails.Length : 0;

    public string IdentifiableName => itemName;

    public FocusAreaUiDetails GetFocusAreaUiDetailsAtIndex(int index)
    {
        return focusAreaUiDetails[index];
    }
}

[Serializable]
public struct RecipeIngredient
{
    public int id;
    public int amount;

    public string[] tags;

    public RecipeIngredient(string id, int amnt)
    {
        this.id = int.Parse(id);
        amount = amnt;
        tags = Array.Empty<string>();
    }
}

[Serializable]
public struct FocusAreaUiDetails
{
    public int id;
    public string name;
    [SerializeField] int x, y;

    public Vector2 PositionOnUi => new Vector2(x, y);
}

public interface Identifiable
{
    string IdentifiableName { get; }
}

public class ItemEventArgs: EventArgs
{
    public Item ItemToPass { get; set; }

    public int AmountToPass { get; set; } = 1;
}


