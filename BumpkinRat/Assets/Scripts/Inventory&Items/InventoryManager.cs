using System;
using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public string itemDataPath;
    public Inventory activeInventory { get; set; }
    public GameObject inventoryMenuObject;

    private void OnEnable()
    {
        Collectable.Collected += OnCollectedItem;
        ItemCrafter.CraftedItem += OnCraftedItem;

        ItemProvisioner.ItemProvisioning += OnCollectedItem;
        InventoryButton.FinalPossiblePress += OnInventoryButtonPressed;

        ItemObject.PlaceItemBackInInventory += OnPlaceItemObjectBack;

        activeInventory = new Inventory();
        activeInventory.InitializeInventory();
    }

    void OnCollectedItem(object source, CollectableEventArgs args)
    {
        activeInventory.AdjustInventory(args);
        //activeInventory.AdjustInventory(args.CollectedItem.itemId, true ,args.CollectedAmount);
        Debug.Log(args.CollectableName + " Collected. Adding to Inventory");
    }

    void OnCraftedItem(object source, CraftingEventArgs args)
    {
        if (!args.craftedRecipe.Craftable(activeInventory)) { Debug.Log("Insufficient materials for crafting, returning..."); return; }
        activeInventory.AdjustInventory(args.craftedRecipe, args.craftedAmount);
        Debug.LogFormat("Crated {0} {1}(s) and adding to Inventory", args.craftedAmount, args.craftedRecipe.identifier);

    }

    void OnInventoryButtonPressed(object source, InventoryButtonArgs args)
    {
        activeInventory.AdjustInventory(args.ItemId, false);
    }

    void OnPlaceItemObjectBack(object source, ItemEventArgs args)
    {
        activeInventory.AdjustInventory(args);
    }

    private void OnDestroy()
    {
        Collectable.Collected -= OnCollectedItem;
        ItemCrafter.CraftedItem -= OnCraftedItem;
        ItemProvisioner.ItemProvisioning -= OnCollectedItem;
        InventoryButton.FinalPossiblePress -= OnInventoryButtonPressed;

        ItemObject.PlaceItemBackInInventory += OnPlaceItemObjectBack;

    }
}

[Serializable]
public class Inventory
{
    Dictionary<int, ItemListing> inventoryListingsByItemId;
    bool InventoryValid => inventoryListingsByItemId != null;

    public event EventHandler<InventoryAdjustmentEventArgs> InventoryAdjusted;

    public void InitializeInventory()
    {
        inventoryListingsByItemId = new Dictionary<int, ItemListing>();
    }

    public bool CheckQuantity(Item i, int amt = 1)
    {
        return CheckQuantity(i.itemId, amt);
    }

    public bool CheckQuantity(int itemId, int amount = 1)
    {
        if (!InventoryValid)
        {
            return false;
        }

        return inventoryListingsByItemId.ContainsKey(itemId) && inventoryListingsByItemId[itemId].amount >= amount;
    }

    public void AdjustInventory(Recipe crafted, int amt = 1)
    {
        AdjustInventory(crafted.outputId, true, amt);
        foreach(RecipeIngredient ing in crafted.ingredients)
        {
            Debug.Log("Removing " + ing.id);
            AdjustInventory(ing.id.GetItem().itemId, false, ing.amount * amt);
        }
    }

    public void AdjustInventory(ItemEventArgs args)
    {
        AddToInventory(args.ItemToPass, args.AmountToPass);
    }

    public void AdjustInventory(int itemId, bool add, int amount = 1)
    {
        if (add)
        {
            AddToInventory(itemId, amount);
        } else
        {
            RemoveFromInventory(itemId, amount);
        }
    }


    void AddToInventory(Item item, int amount = 1)
    {
        bool addingNew = !Owned(item.itemId);

        if (addingNew)
        {
            inventoryListingsByItemId.Add(item.itemId, new ItemListing { item = item, amount = amount });
            InventoryAdjusted.BroadcastEvent(this,
                InventoryAdjustmentEventArgs.FromListing(inventoryListingsByItemId[item.itemId], 'a'));
        }
        else
        {
            AddToInventory(item.itemId, amount, true);
        }
    }

    void AddToInventory(int itemId, int amount = 1, bool bypass = false)
    {
        bool addingNew = !Owned(itemId) && !bypass;

        if (addingNew)
        {
            Item adding = itemId.GetItem();
            inventoryListingsByItemId.Add(itemId, new ItemListing { item = adding, amount = amount });


        } else
        {
            inventoryListingsByItemId[itemId].Add(amount);
        }

        char add = addingNew ? 'a' : ' ';

        InventoryAdjusted.BroadcastEvent(this,
            InventoryAdjustmentEventArgs.FromListing(inventoryListingsByItemId[itemId], add));
    }

    void RemoveFromInventory(int itemId, int amount = 1)
    {
        if (Owned(itemId))
        {
            inventoryListingsByItemId[itemId].Remove(amount);

            bool remove = inventoryListingsByItemId[itemId].EmptyListing;
            char removeChar = ' ';
            ItemListing listing = inventoryListingsByItemId[itemId];

            if (remove)
            {
                inventoryListingsByItemId.Remove(itemId);
                removeChar = 'r';
            }

            InventoryAdjusted.BroadcastEvent(this,
                InventoryAdjustmentEventArgs.FromListing(listing, removeChar));

        }
    }

    bool Owned(int id)
    {
        return InventoryValid && inventoryListingsByItemId.ContainsKey(id);
    }
}

[Serializable]
public class ItemListing
{
   public Item item { get; set; }
    internal int amount;

    public int ListingAmount => amount;

    private bool empty;
    public bool EmptyListing
    {
        get
        {
            empty = amount <= 0;
            return empty;
        }
    }

    public event EventHandler ItemListingEmpty;

    public event EventHandler ItemListingChanged;

    public void Add(int adding)
    {
        amount += adding;
        Broadcast();
    }

    public void Remove(int removing)
    {
        amount -= removing;
        Broadcast();
    }

    void Broadcast()
    {
        if(ItemListingChanged != null)
        {
            ItemListingChanged(this, new EventArgs());
        }
    }

    public override string ToString()
    {
        return $"{item.DisplayName}: {amount}";
    }
}

public class InventoryAdjustmentEventArgs: EventArgs
{
    public ItemListing Listing { get; set; }
    public string ItemToAdjust { get; set; } = string.Empty;
    public string NewAmountToDisplay { get; set; } = "0";
    public bool Adding { get; set; }
    public bool Removing { get; set; }

    public static InventoryAdjustmentEventArgs FromListing(ItemListing listing, char addOrRemove = ' ')
    {
        return new InventoryAdjustmentEventArgs {
            Listing = listing,
            ItemToAdjust = listing.item.DisplayName,
            NewAmountToDisplay = listing.amount.ToString(),
            Adding = addOrRemove.Equals('a'),
            Removing = addOrRemove.Equals('r')
        };
    }
}