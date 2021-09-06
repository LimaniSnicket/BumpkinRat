using System;
using UnityEngine;

public class ItemProvisioner : ItemDistributionBase, IItemDistribution
{
    public static event EventHandler<ItemEventArgs> ItemProvisioning;

    public ItemProvisioner()
    {
        this.itemDistributionSettings = new ItemDistributionSettings();
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
        if (!itemDistributionSettings.CanDistribute)
        {
            Debug.Log("Item Drops aren't valid.");
            return;
        }

        foreach (ItemDrop drop in itemDistributionSettings.ItemsToDrop)
        {
            Debug.Log(drop.ItemToDropName);
            BroadcastItemProvisioning(drop);
        }
    }

    private void BroadcastItemProvisioning(ItemDrop collecting)
    {
        if(ItemProvisioning != null)
        {
            ItemProvisioning(this,
                new ItemEventArgs {
                    ItemToPass = collecting.ToDrop,
                    AmountToPass = collecting.AmountToDrop
                }) ;
        }
    }
}
