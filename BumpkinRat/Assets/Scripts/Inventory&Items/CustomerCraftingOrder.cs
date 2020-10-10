using System;

public class CraftingPrompt
{
    public int[] recipeLookupIds;

    public CraftingPrompt()
    {
        recipeLookupIds = Array.Empty<int>();
    }

    public CraftingPrompt(params int[] recipeIds)
    {
        int length = recipeIds.Length;
        recipeLookupIds = new int[length];
        Array.Copy(recipeIds, recipeLookupIds, length);
    }

    public bool ValidPrompt
    {
        get => recipeLookupIds.CollectionIsNotNullOrEmpty();
    }
    public bool SingleRecipePrompt
    {
        get => recipeLookupIds.CollectionCountEquals(1);
    }

    public Recipe RecipeToCraft(int lookup)
    {
        return DatabaseContainer.gameData.GetRecipe(lookup);
    }

}

[Serializable]
public class CustomerCraftingOrder: CraftingPrompt
{
    public string customerName;
}
