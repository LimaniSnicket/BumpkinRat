using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public string itemDataPath;
    public Inventory activeInventory { get; set; }
    public GameObject inventoryMenuObject;
    public InventoryMenu inventoryMenu;

    private void OnEnable()
    {
        Collectable.Collected += OnCollectedItem;
        ItemCrafter.CraftedItem += OnCraftedItem;

        ItemProvisioner.ItemProvisioning += OnCollectedItem;

        activeInventory = new Inventory();
        activeInventory.InitializeInventory();
        inventoryMenu = new InventoryMenu(inventoryMenuObject != null ? inventoryMenuObject : new GameObject());
    }

    private void Update()
    {
/*        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventoryMenu.LoadMenu(activeInventory);
            //itemCrafter.CraftRecipe(DatabaseContainer.gameData.GetRecipe(0), 1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            inventoryMenu.CloseMenu();
        }*/
    }

    void OnCollectedItem(object source, CollectableEventArgs args)
    {
        activeInventory.AdjustInventory(args);
        //activeInventory.AdjustInventory(args.CollectedItem.itemId, true, args.CollectedAmount);
        //activeInventory.AdjustInventory(true, args.CollectableName, args.CollectedAmount);
        Debug.Log(args.CollectableName + " Collected. Adding to Inventory");
    }

    void OnCraftedItem(object source, CraftingEventArgs args)
    {
        if (!args.craftedRecipe.Craftable(activeInventory)) { Debug.Log("Insufficient materials for crafting, returning..."); return; }
        activeInventory.AdjustInventory(args.craftedRecipe, args.craftedAmount);
        Debug.LogFormat("Crated {0} {1}(s) and adding to Inventory", args.craftedAmount, args.craftedRecipe.identifier);

    }

    private void OnDisable()
    {
        Collectable.Collected -= OnCollectedItem;
        ItemCrafter.CraftedItem -= OnCraftedItem;
        ItemProvisioner.ItemProvisioning -= OnCollectedItem;
    }
}

[Serializable]
public class Inventory
{
    public Dictionary<string, int> inventoryListings;


    //todo make inventory int id based!
    //int id to (string, int) inventory Listing
    Dictionary<int, ItemListing> inventoryListingsByItemId;
    bool inventoryExists => inventoryListings != null;

    bool InventoryValid => inventoryListingsByItemId != null;

    public event EventHandler<InventoryAdjustmentEventArgs> InventoryAdjusted;

    public void InitializeInventory()
    {
        inventoryListings = new Dictionary<string, int>();
        inventoryListingsByItemId = new Dictionary<int, ItemListing>();
    }

    public bool CheckQuantity(string id, int amt = 1)
    {
        if (!Owned(id)) { return false; }
        return inventoryListings[id] >= amt;
    }

    public bool CheckQuantity(Item i, int amt = 1)
    {
        return CheckQuantity(i.identifier, amt);
    }

    public bool CheckQuantity(int itemId, int amount = 1)
    {
        if (!InventoryValid)
        {
            return false;
        }

        return inventoryListingsByItemId.ContainsKey(itemId) && inventoryListingsByItemId[itemId].amount >= amount;
    }

    public List<Item> GetItems(params string[] ids)
    {
        List<Item> returnItems = new List<Item>();
       foreach(string i in ids)
        {
            if (CheckQuantity(i)) { returnItems.Add(i.GetItem()); }
        }
        return returnItems;
    }

    public void AdjustInventory(Recipe crafted, int amt = 1)
    {
        AdjustInventory(true, crafted.identifier, amt);
        foreach(RecipeIngredient ing in crafted.ingredients)
        {
            Debug.Log("Removing " + ing.id);
            AdjustInventory(false, ing.id.GetItem().itemName, ing.amount * amt);
        }
    }

    public void AdjustInventory(CollectableEventArgs args)
    {
        if(args.CollectedItem == null)
        {

        } else
        {
            AddToInventory(args.CollectedItem, args.CollectedAmount);
        }
    }

    public void AdjustInventory(bool add, string itemName, int amount = 1)
    {
        if (add)
        {
            AddToInventory(itemName, amount);
        } else
        {
            RemoveFromInventory(itemName, amount);
        }
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

    void AddToInventory(string itemName, int amount = 1)
    {
        bool addingNew = false;
        if (ValidInventoryListing(itemName)) {
            inventoryListings[itemName] += amount;
        } else
        {
            inventoryListings.Add(itemName, amount);
            addingNew = true;
        }

        InventoryAdjusted.BroadcastEvent(this,
            new InventoryAdjustmentEventArgs {
                ItemToAdjust = itemName,
                NewAmountToDisplay = inventoryListings[itemName].ToString(),
                AmountToAdjustBy = amount,
                Adding = addingNew
            }) ;
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

    void RemoveFromInventory(string itemName, int amountToRemove = 1)
    {
        if (ValidInventoryListing(itemName))
        {
            if(inventoryListings[itemName] > amountToRemove)
            {
                inventoryListings[itemName] -= amountToRemove;
            } else if(inventoryListings[itemName] < amountToRemove)
            {
                Debug.Log("Not enough in inventory");
            } else
            {
                inventoryListings.Remove(itemName);

                InventoryAdjusted.BroadcastEvent(this,
                    new InventoryAdjustmentEventArgs
                    {
                        ItemToAdjust = itemName,
                        Removing = true
                    }) ;

                return;
                      
            }

            InventoryAdjusted.BroadcastEvent(this,
                new InventoryAdjustmentEventArgs
                {
                    ItemToAdjust = itemName,
                    NewAmountToDisplay = inventoryListings[itemName].ToString(),
                    AmountToAdjustBy = -1 * amountToRemove,
                }); ;
        }
    }

    bool Owned(Item i)
    {
        return inventoryListings.ContainsKey(i.identifier);
    }

    bool Owned(string id)
    {
        return inventoryListings.ContainsKey(id);
    }

    bool Owned(int id)
    {
        return InventoryValid && inventoryListingsByItemId.ContainsKey(id);
    }

    bool ValidInventoryListing(string name)
    {
        if (!inventoryExists) { return false; }
        return inventoryListings.ContainsKey(name);
    }
}

[Serializable]
public struct ItemListing
{
   public Item item { get; set; }
   public int amount { get; set; }

    public bool EmptyListing => amount <= 0;

    public void Add(int adding)
    {
        amount += adding;
    }

    public void Remove(int removing)
    {
        amount -= removing;
    }
}

public class InventoryAdjustmentEventArgs: EventArgs
{
    public string ItemToAdjust { get; set; }
    public int AmountToAdjustBy { get; set; }

    public string NewAmountToDisplay { get; set; }
    public bool Adding { get; set; }
    public bool Removing { get; set; }

    public static InventoryAdjustmentEventArgs FromListing(ItemListing listing, char addOrRemove = ' ')
    {
        return new InventoryAdjustmentEventArgs {
            ItemToAdjust = listing.item.DisplayName,
            NewAmountToDisplay = listing.amount.ToString(),
            Adding = addOrRemove.Equals('a'),
            Removing = addOrRemove.Equals('r')
        };
    }
}