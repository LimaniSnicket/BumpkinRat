using System;
using System.Collections.Generic;
using Items;

[Serializable]
public class Inventory
{
    private readonly Dictionary<int, ItemListing> inventoryListingsByItemId;
    public bool InventoryValid => inventoryListingsByItemId != null;

    public event EventHandler<InventoryAdjustmentEventArgs> InventoryAdjusted;

    public Inventory()
    {
        inventoryListingsByItemId = new Dictionary<int, ItemListing>();
    }

    public bool ContainsMinimunAmountOfItem(int itemId, int minimum = 1)
    {
        if (!InventoryValid)
        {
            return false;
        }

        return inventoryListingsByItemId.ContainsKey(itemId) && inventoryListingsByItemId[itemId].ListedAmount >= minimum;
    }

    public void AddCraftedRecipeOutputToInventory(Recipe crafted, int amt = 1)
    {
        Item recipeOutput = ItemDataManager.GetItemById(crafted.outputId);
        AddItemToInventory(recipeOutput, amt);

        foreach (RecipeIngredient ingredient in crafted.ingredients)
        {
            RemoveFromInventory(ingredient.id, ingredient.amount * amt);
        }
    }

    public void AddToInventory(ItemEventArgs args)
    {
        AddItemToInventory(args.ItemToPass, args.AmountToPass);
    }

    private void AddItemToInventory(Item item, int amount)
    {
        bool addingNew = !Owned(item.itemId);

        ItemListing listing = this.GetItemListingForId(addingNew, item.itemId);

        listing.Add(amount);

        InventoryAdjustment adjustment = addingNew ? InventoryAdjustment.ADD : InventoryAdjustment.UPDATE;
        var adjustmentArgs = new InventoryAdjustmentEventArgs(listing, adjustment);

        InventoryAdjusted.BroadcastEvent(this, adjustmentArgs);
    }

    public void RemoveFromInventory(int itemId, int amount = 1)
    {
        if (Owned(itemId))
        {
            ItemListing listing = inventoryListingsByItemId[itemId];

            listing.Remove(amount);

            bool remove = listing.IsEmpty;

            var adjustment = InventoryAdjustment.UPDATE;

            if (remove)
            {
                inventoryListingsByItemId.Remove(itemId);
                adjustment = InventoryAdjustment.REMOVE;
            }

            var adjustmentArgs = new InventoryAdjustmentEventArgs(listing, adjustment);

            InventoryAdjusted.BroadcastEvent(this, adjustmentArgs);
        }
    }

    private bool Owned(int id)
    {
        return InventoryValid && inventoryListingsByItemId.ContainsKey(id);
    }

    private ItemListing GetItemListingForId(bool createNewListing, int id, int amount = 0)
    {
        if (createNewListing)
        {
            var itemListing = new ItemListing(ItemDataManager.GetItemById(id), amount);
            inventoryListingsByItemId.Add(id, itemListing);
        }

        return inventoryListingsByItemId[id];
    }
}

public enum InventoryAdjustment
{
    NONE = 0,
    UPDATE = 1,
    ADD = 2,
    REMOVE = 3
}