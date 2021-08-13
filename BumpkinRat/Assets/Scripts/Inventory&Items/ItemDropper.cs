using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDropper : ItemDistrubutionSettings
{
    private readonly IDistributeItems<ItemDropper> dropper; 

    private Transform dropTransform;
    public ItemDropper(IDistributeItems<ItemDropper> dropper, Transform dropTransform = null)
    {
        this.dropper = dropper;
        this.dropTransform = dropTransform;
    }

    public override void Distribute()
    {
        if (dropper == null)
        {
            return;
        }

        InstantiateItemsToDrop();
    }

    void InstantiateItemsToDrop()
    {
        if (!ItemsToDrop.ValidList())
        {
            return;
        }

        if (dropTransform == null)
        {
            dropTransform = PlayerBehavior.PlayerGameObject.transform;
        }

        foreach (ItemDrop drop in ItemsToDrop)
        {
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 3;

            Debug.Log($"Dropping {drop.AmountToDrop} of item: {drop.ItemToDropName}");

            drop.ItemToDropName.InstantiateItemInWorld(new Vector3(randomPos.x, 0, randomPos.z) + dropTransform.position);
        }
    }
}

public interface IDistributeItems<T> where T: ItemDistrubutionSettings
{
    T Distributor { get; }
}

public class ItemDistrubutionSettings
{
    private bool clearOnDistribute;

    private static ItemDropFactory itemDropFactory;
    public List<ItemDrop> ItemsToDrop { get; private set; }
    public bool CanDistribute => ItemsToDrop.ValidList();

    public ItemDistrubutionSettings(params (int, int)[] dropData)
    {
        this.SetItemDropData(dropData);
    }

    public void AddItemToDrop(ItemDrop toDrop)
    {
        ItemsToDrop.Add(toDrop);
    }

    public void SetItemDropData(params (int, int)[] dropData)
    {
        InitializeDropFactoryIfNull();
        var itemDrops = itemDropFactory.GetItemsToDrop(dropData);
        ItemsToDrop = itemDrops;
    }

    public void SetItemDropData(params int[] dropData)
    {
        InitializeDropFactoryIfNull();
        var itemDrops = itemDropFactory.GetItemsToDrop(dropData);
        ItemsToDrop = itemDrops;
    }

    private void InitializeDropFactoryIfNull()
    {
        if(itemDropFactory == null)
        {
            itemDropFactory = new ItemDropFactory();
        }
    }
    public virtual void Distribute() { }
}
