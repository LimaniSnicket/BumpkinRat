using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine.Events;

public class RecipeProgressTracker
{
    Dictionary<Recipe, int> recipeStepsTaken;
    Dictionary<int, int> activeItemObjects;

    public int? RecipesInProgress => recipeStepsTaken.Count;

    public static RecipeProgressCompleted onRecipeCompleted;

    public RecipeProgressTracker() 
    {
        recipeStepsTaken = new Dictionary<Recipe, int>();
        activeItemObjects = new Dictionary<int, int>();
        onRecipeCompleted = new RecipeProgressCompleted();
    }

    public void AddToCache(int key, int value)
    {

    }

    public void AddToCache(Dictionary<int, int> active)
    {
        IEnumerable<Recipe> newRecipes = DatabaseContainer.gameData.GetCraftableRecipes(active)
            .Where(r => !recipeStepsTaken.ContainsKey(r));

        recipeStepsTaken.AddMany(newRecipes.Select(r => new KeyValuePair<Recipe, int>(r, 0)));
    }

    public void RemoveFromCache(KeyValuePair<int, int> alteredValue)
    {
        recipeStepsTaken.FilterOut(r => !r.RenderedUncraftable(alteredValue));
    }

    void RemoveFromCache((KeyValuePair<int, int>, KeyValuePair<int, int>) alteredValues)
    {
        recipeStepsTaken.FilterOut(r => !r.RenderedUncraftable(alteredValues.Item1) || !r.RenderedUncraftable(alteredValues.Item2));
    }

    public void RegisterCraftingSequenceProgress(CraftingSequence sequence, Dictionary<int, int> itemObjects)
    {
        for(int i = 0; i < recipeStepsTaken.Count; i++)
        {
            Recipe r = recipeStepsTaken.ElementAt(i).Key;

            if (r.ValidateCraftingSequence(sequence))
            {
                recipeStepsTaken.Increment(r);
            }

            if (CompletedSteps(recipeStepsTaken.ElementAt(i))){
                RemoveFromCache(sequence.GetKeyValues());
                sequence.DemolishObjects(itemObjects);
                onRecipeCompleted.Invoke(r);
                break;
            }
        }
    }

    bool CompletedSteps(KeyValuePair<Recipe, int> pair)
    {
        return pair.Value == pair.Key.craftingSequences.Count;
    }

    public class RecipeProgressCompleted: UnityEvent<Recipe>
    {

    }

}