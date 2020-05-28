using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class Item: Identifiable
{
    public string ID;
    public string display;
    public int value;

    public string identifier => ID;
}

[Serializable]
public class Recipe: Identifiable
{
    public string outputID;
    public List<RecipeIngredient> ingredients;

    public string identifier => outputID;
}

[Serializable]
public struct RecipeIngredient
{
    public string ID;
    public int amount;
}

public interface Identifiable
{
    string identifier { get; }
}
