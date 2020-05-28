using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public string itemDataPath;
    public Inventory activeInventory;

    private void OnEnable()
    {
        Collectable.Collected += OnCollectedItem;
    }

    void OnCollectedItem(object source, CollectableEventArgs args)
    {
        activeInventory.AdjustInventory(true, args.CollectableName, args.CollectedAmount);
        Debug.Log(args.CollectableName + " Collected. Adding to Inventory");
    }

    private void OnDisable()
    {
        Collectable.Collected -= OnCollectedItem;
    }
}

[Serializable]
public class Inventory
{
    public Dictionary<string, int> itemsOwned;   
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

    bool ValidInventoryListing(string name)
    {
        if (!inventoryExists) { return false; }
        return itemsOwned.ContainsKey(name);
    }
}

[Serializable]
public class ItemListing
{
   public Item item { get; set; }
    public int amount { get; set; }
 }