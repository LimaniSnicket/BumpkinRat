using System;
using System.Linq;
using UnityEngine;

public class ItemProvisioner : ItemDistrubutionSettings
{
    private IDistributeItems<ItemProvisioner> provisioner; 

    public static event EventHandler<ItemEventArgs> ItemProvisioning;

    public ItemProvisioner(IDistributeItems<ItemProvisioner> prov)
    {
        provisioner = prov;
    }

    public override void Distribute()
    {
        Debug.Log(string.Join("-", ItemsToDrop.Select(s => s.ItemToDropName)));

        if (!ItemsToDrop.ValidList())
        {
            Debug.Log("Item Drops aren't valid.");
            return;
        }

        foreach (ItemDrop drop in ItemsToDrop)
        {
            BroadcastItemProvisioning(drop);
        }
    }

    void BroadcastItemProvisioning(ItemDrop collecting)
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
