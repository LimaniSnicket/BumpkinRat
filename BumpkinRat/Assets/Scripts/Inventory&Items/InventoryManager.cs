using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public string itemDataPath;
    public Inventory activeInventory { get; set; }
    ItemCrafter itemCrafter;
    public GameObject inventoryMenuObject;
    public InventoryMenu inventoryMenu;

    private void OnEnable()
    {
        Collectable.Collected += OnCollectedItem;
        ItemCrafter.CraftedItem += OnCraftedItem;
        itemCrafter = new ItemCrafter();
        activeInventory = new Inventory();
        activeInventory.InitializeInventory();
        inventoryMenu = new InventoryMenu(inventoryMenuObject != null ? inventoryMenuObject : new GameObject());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            inventoryMenu.LoadMenu(activeInventory);
            //itemCrafter.CraftRecipe(DatabaseContainer.gameData.GetRecipe(0), 1);
        }
    }

    void OnCollectedItem(object source, CollectableEventArgs args)
    {
        activeInventory.AdjustInventory(true, args.CollectableName, args.CollectedAmount);
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
    }
}

[Serializable]
public class Inventory
{
    public Dictionary<string, int> inventoryListings;   
    bool inventoryExists => inventoryListings != null;

    public void InitializeInventory()
    {
        inventoryListings = new Dictionary<string, int>();
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
            Debug.Log("Removing " + ing.ID);
            AdjustInventory(false, ing.ID, ing.amount * amt);
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

    void AddToInventory(string itemName, int amount = 1)
    {
        if (ValidInventoryListing(itemName)) {
            inventoryListings[itemName] += amount;
        } else
        {
            inventoryListings.Add(itemName, amount);
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
            }
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

    bool ValidInventoryListing(string name)
    {
        if (!inventoryExists) { return false; }
        return inventoryListings.ContainsKey(name);
    }
}

[Serializable]
public class ItemListing
{
   public Item item { get; set; }
   public int amount { get; set; }
}