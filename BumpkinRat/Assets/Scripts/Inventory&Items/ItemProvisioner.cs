using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemProvisioner : ItemDistributor
{
    IDistributeItems<ItemProvisioner> Provisioner { get; set; }

    public static event EventHandler<CollectableEventArgs> ItemProvisioning;

    public ItemProvisioner(IDistributeItems<ItemProvisioner> prov)
    {
        Provisioner = prov;
    }
    public override void Distribute()
    {
        SetItemsToDrop(Provisioner.ItemDropData);

        if (!ItemsToDrop.ValidList())
        {
            return;
        }

        foreach(ItemDrop drop in ItemsToDrop)
        {
            BroadcastItemProvisioning(drop);
        }
    }

    void BroadcastItemProvisioning(ItemDrop collecting)
    {
        if(ItemProvisioning != null)
        {
            ItemProvisioning(this, 
                new CollectableEventArgs { 
                    CollectableName = collecting.itemName, 
                    CollectedAmount = collecting.AmountToDrop 
                });
        }
    }
}
