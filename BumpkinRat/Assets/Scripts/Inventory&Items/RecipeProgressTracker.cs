using System.Collections.Generic;
using System.Linq;

public class RecipeProgressTracker
{
    public List<Recipe> potentialCraftingCache;

    Dictionary<int, int> recipeStepsTaken;

    public RecipeProgressTracker() 
    {
        potentialCraftingCache = new List<Recipe>();
        recipeStepsTaken = new Dictionary<int, int>();
    }

    public void AddToCache(Dictionary<int, int> active)
    {
        IEnumerable<Recipe> newRecipes = DatabaseContainer.gameData.GetCraftableRecipes(active)
            .Where(r => !potentialCraftingCache.Contains(r));

        potentialCraftingCache.AddRange(newRecipes);
        recipeStepsTaken.AddMany(newRecipes.Select(r => new KeyValuePair<int, int>(r.recipeId, 0)));
    }

    public void RemoveFromCache(KeyValuePair<int, int> alteredValue)
    {
        potentialCraftingCache = potentialCraftingCache.Where(r => !r.RenderedUncraftable(alteredValue)).ToList(); //this is bad fix it
    }

}