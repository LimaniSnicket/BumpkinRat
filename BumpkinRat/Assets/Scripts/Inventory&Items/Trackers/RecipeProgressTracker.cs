using System.Collections.Generic;
using System.Linq;
using Items;
using UnityEngine.Events;

public class RecipeProgressTracker
{
    private readonly Dictionary<Recipe, int> recipeStepsTaken;

    private readonly ItemIdToAmountMap activeItemObjectMap;

    public static RecipeProgressCompleted onRecipeCompleted;

    public RecipeProgressTracker() 
    {
        recipeStepsTaken = new Dictionary<Recipe, int>();
        onRecipeCompleted = new RecipeProgressCompleted();
        activeItemObjectMap = new ItemIdToAmountMap();
    }

    public void IncrementItemAmount(int id)
    {
        activeItemObjectMap.IncrementItemAmount(id);
        RefreshPossibleCraftableRecipes();
    }

    public void DecrementItemAmount(int id)
    {
        int amount = activeItemObjectMap.GetDecrementedItemAmount(id);
        FilterUncraftableRecipes(id, amount);
    }

    private void RefreshPossibleCraftableRecipes()
    {
        IEnumerable<Recipe> newRecipes = ItemDataManager.GetRecipesCraftableWith(activeItemObjectMap.ItemMap)
            .Where(r => !recipeStepsTaken.ContainsKey(r));

        recipeStepsTaken.AddMany(newRecipes.Select(r => new KeyValuePair<Recipe, int>(r, 0)));
    }

    private void FilterUncraftableRecipes(int itemId, int amount)
    {
        recipeStepsTaken.FilterOut(r => !r.CraftableWithItemAmount(itemId, amount));
    }

    void FilterUncraftableRecipes(CraftingSequence sequence)
    {
        if (sequence.IsValid())
        {
            int actionId = sequence.actionItemObject.itemObject.Item.itemId;
            int targetId = sequence.targetItemObject.itemObject.Item.itemId;
            recipeStepsTaken.FilterOut(r => !r.CraftableWithItemAmount(actionId, 1) || !r.CraftableWithItemAmount(targetId, 1));
        }
    }

    public void RegisterCraftingSequenceProgress(CraftingSequence sequence)
    {
        foreach(var recipe in recipeStepsTaken.Keys)
        {
            if (recipe.ValidateCraftingSequence(sequence))
            {
                recipeStepsTaken.Increment(recipe);
            }

            if (CompletedSteps(recipe, recipeStepsTaken[recipe]))
            {
                FilterUncraftableRecipes(sequence);
                sequence.DemolishObjects(activeItemObjectMap.ItemMap);
                onRecipeCompleted.Invoke(recipe);
                break;
            }
        }
    }

    bool CompletedSteps(Recipe recipe, int stepsTaken)
    {
        return stepsTaken == recipe.sequences.Count;
    }

    public class RecipeProgressCompleted: UnityEvent<Recipe>
    {

    }

}