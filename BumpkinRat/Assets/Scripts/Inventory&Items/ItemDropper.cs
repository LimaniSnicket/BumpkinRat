using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDropper : ItemDistributionBase, IItemDistribution
{
    private readonly IDistributeItems dropper; 

    private Transform dropTransform;

    public ItemDropper(IDistributeItems dropper, Transform dropTransform = null)
    {
        this.dropper = dropper;
        this.dropTransform = dropTransform;

        this.itemDistributionSettings = new ItemDistributionSettings();
    }

    public void Distribute()
    {
        if (dropper == null)
        {
            return;
        }

        this.InstantiateItemsToDrop();
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

    private void InstantiateItemsToDrop()
    {
        if (!this.itemDistributionSettings.CanDistribute)
        {
            return;
        }

        if (dropTransform == null)
        {
            dropTransform = PlayerBehavior.PlayerGameObject.transform;
        }

        this.IterateOverItemDropData(DropItem);
    }

    private void DropItem(ItemDrop drop)
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 3;

        drop.ItemToDropName.InstantiateItemInWorld(new Vector3(randomPos.x, 0, randomPos.z) + dropTransform.position);
    }
}

public class ItemDistributionBase
{
    protected ItemDistributionSettings itemDistributionSettings;
    protected void AddItemDropToItemDistributionSettings(ItemDrop toDrop)
    {
        itemDistributionSettings.AddItemToDrop(toDrop);
    }

    protected void AddItemsToDropToItemDistributionSettings( params (int, int)[] dropData)
    {
        itemDistributionSettings.AddItemsToDrop(dropData);
    }

    protected void AddItemsToDropToItemDistributionSettings(params int[] dropData)
    {
        itemDistributionSettings.AddItemsToDrop(dropData);
    }

    protected void IterateOverItemDropData(Action<ItemDrop> callOnItemDrop)
    {
        foreach(var itemDrop in itemDistributionSettings.ItemsToDrop)
        {
            callOnItemDrop(itemDrop);
        }
    }

    protected void IterateOverItemDropDataUntil(Action<ItemDrop> callOnItemDrop, Func<bool> breakCondition)
    {
        foreach (var itemDrop in itemDistributionSettings.ItemsToDrop)
        {
            callOnItemDrop(itemDrop);

            if (breakCondition())
            {
                break;
            }
        }
    }
}

public interface IItemDistribution
{
    void AddItemToDrop(ItemDrop toDrop);

    void AddItemsToDrop(params (int, int)[] dropData);

    void AddItemsToDrop(params int[] dropData);

    void Distribute();
}

public interface IDistributeItems
{
    IItemDistribution ItemDistributor { get; }
}

public class ItemDistributionSettings
{
    private readonly bool clearDataOnDistribute;

    private static ItemDropFactory itemDropFactory;
    public List<ItemDrop> ItemsToDrop { get; private set; }
    public bool CanDistribute => ItemsToDrop.ValidList();

    public ItemDistributionSettings(): this(false)
    {
    }

    public ItemDistributionSettings(bool clearDataOnDistribute)
    {
        this.clearDataOnDistribute = clearDataOnDistribute;
        this.InitializeDropFactoryIfNull();
        ItemsToDrop = new List<ItemDrop>();
    }

    public void AddItemToDrop(ItemDrop toDrop)
    {
        ItemsToDrop.Add(toDrop);
    }

    public void AddItemsToDrop(params (int, int)[] dropData)
    {
        this.InitializeDropFactoryIfNull();
        var itemDrops = itemDropFactory.GetItemsToDrop(dropData);
        ItemsToDrop.AddRange(itemDrops);
    }

    public void AddItemsToDrop(params int[] dropData)
    {
        this.InitializeDropFactoryIfNull();
        var itemDrops = itemDropFactory.GetItemsToDrop(dropData);
        ItemsToDrop.AddRange(itemDrops);
    }

    public void Cleanup()
    {
        if (clearDataOnDistribute)
        {
            ItemsToDrop.Clear();
        }
    }
    private void InitializeDropFactoryIfNull()
    {
        if(itemDropFactory == null)
        {
            itemDropFactory = new ItemDropFactory();
        }
    }
}
