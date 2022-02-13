using UnityEngine;
using Items;

public class ItemPlacer : ItemDistributionBase, IItemDistribution
{
    private readonly IDistributeItems placer;

    private Vector3[] placementPositions = new Vector3[0];

    private OccupiablePositionContainer occupiablePositionContainer;

    public ItemPlacer(IDistributeItems placer)
    {
        this.placer = placer;
        occupiablePositionContainer = new OccupiablePositionContainer(placementPositions);

        this.itemDistributionSettings = new ItemDistributionSettings();
        //RecipeProgressTracker.onRecipeCompleted.AddListener(PlaceFromRecipe);
    }

    public void AddItemsToDrop(params (int, int)[] dropData)
    {
        this.AddItemsToDropToItemDistributionSettings(dropData);
    }

    public void AddItemsToDrop(params int[] dropData)
    {
        this.AddItemsToDropToItemDistributionSettings(dropData);
    }

    public void AddItemToDrop(ItemDrop toDrop)
    {
        this.AddItemDropToItemDistributionSettings(toDrop);
    }

    public void Distribute()
    {
        if (!this.itemDistributionSettings.CanDistribute)
        {
            return;
        }

        this.IterateOverItemDropDataUntil(PlaceItem, NextPositionInvalid);

        itemDistributionSettings.Cleanup();
    }

    private void PlaceItem(ItemDrop itemDrop)
    {
        var position = occupiablePositionContainer.GetNext();
        if (position != null)
        {
            var itemObject = ItemDataManager.SpawnFromPrefabPath(itemDrop.ToDrop);
            position.Occupy(itemObject);
        }
        else
        {
            Debug.LogWarning("Can no longer spawn items here!");
        }
    }

    private bool NextPositionInvalid()
    {
        return !occupiablePositionContainer.HasNext();
    }
}