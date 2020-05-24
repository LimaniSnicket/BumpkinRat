using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public Inventory activeInventory;
    private void OnEnable()
    {
       if(activeInventory != null) { activeInventory.InitializeInventory(); }
        Collectable.OnCollected += OnCollectedItem;
    }

    void OnCollectedItem(string itemName, int amnt = 1)
    {
        activeInventory.AdjustInventory(true, itemName, amnt);
        Debug.Log(itemName + " Collected. Adding to Inventory");
    }

    private void OnDisable()
    {
        Collectable.OnCollected -= OnCollectedItem;
    }
}

[Serializable]
public class Inventory
{
    public Dictionary<string, int> itemsOwned;
    List<ItemListing> loadedItems;

    bool inventoryExists => itemsOwned != null;

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
            itemsOwned[itemName] += amount;
        } else
        {
            itemsOwned.Add(itemName, amount);
        }
    }

    void RemoveFromInventory(string itemName, int amountToRemove = 1)
    {
        if (ValidInventoryListing(itemName))
        {
            if(itemsOwned[itemName] > amountToRemove)
            {
                itemsOwned[itemName] -= amountToRemove;
            } else if(itemsOwned[itemName] < amountToRemove)
            {
                Debug.Log("Not enough in inventory");
            } else
            {
                itemsOwned.Remove(itemName);
            }
        }
    }

    public void InitializeInventory()
    {
        InitializeInventory(loadedItems);
    }

    void InitializeInventory(List<ItemListing> items)
    {
        if(itemsOwned == null) { itemsOwned = new Dictionary<string, int>(); }
        if (!items.ValidList<ItemListing>()) { return; }
        foreach (ItemListing a in items)
        {
            if (!itemsOwned.ContainsKey(a.itemName)) { itemsOwned.Add(a.itemName, a.amount); }
            else { itemsOwned[a.itemName] = a.amount; }
        }
    }

    bool ValidInventoryListing(string name)
    {
        if (!inventoryExists) { return false; }
        return itemsOwned.ContainsKey(name);
    }
}

[Serializable]
public class ItemListing
{
    public string itemName;
    public int amount;
}
