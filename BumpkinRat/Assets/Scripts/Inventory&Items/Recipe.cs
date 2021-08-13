using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class Recipe: Identifiable
{
    public int id;
    public int outputId;

    public string outputName = "fuck this variable";

    public string recipeDescription;

    public List<RecipeIngredient> ingredients;

    public List<string> sequences;
    public string IdentifiableName => outputName;

    private  Dictionary<int, int> IngredientMap => ingredients.ToDictionary(k => k.id, k => k.amount);

    public bool ValidateCraftingSequence(CraftingSequence sequence)
    {
        return sequences.Contains(sequence.ToString());
    }

    public bool CraftableWithCurrentInventory(Inventory inventory)
    {
       if (!ingredients.ValidList()) { 
           return false; 
       }

       foreach(RecipeIngredient i in ingredients)
        {
            if(!inventory.ContainsMinimunAmountOfItem(i.id, i.amount)) { return false; }
        }

        return true;
    }

    public bool CraftableWithItemAmount(int item, int amount)
    {
        if(!IngredientMap.CollectionIsNotNullOrEmpty() || !IngredientMap.ContainsKey(item))
        {
            return false;
        }
        return IngredientMap[item] > amount;
    }

    public bool Craftable(Dictionary<int, int> ingredientsAvailable)
    {
        if (!ingredientsAvailable.CollectionIsNotNullOrEmpty() || !ingredients.ValidList())
        {
            return false;
        }

        return !ingredientsAvailable.Except(IngredientMap).Any();

    }
}


