using System;
using System.Linq;
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

        Debug.Log(string.Join("-", Provisioner.ItemDropData.Select(s => s.ItemToDropName)));


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
                new CollectableEventArgs {
                    ItemToPass = collecting.ToDrop,
                    CollectableName = collecting.ItemToDropName,
                    AmountToPass = collecting.AmountToDrop
                }) ;
        }
    }
}
