using FullSerializer;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class ItemDropper : ItemDistributor
{
    IDistributeItems<ItemDropper> Dropper { get; set; }
    Transform DropTransform { get; set; }
    public ItemDropper(IDistributeItems<ItemDropper> dropper)
    {
        Dropper = dropper;
    }
    public override void Distribute()
    {
        if (Dropper == null)
        {
            return;
        }

        SetItemsToDrop(Dropper.ItemDropData);

        InstantiateItemsToDrop();
    }

    void InstantiateItemsToDrop()
    {
        if (!ItemsToDrop.ValidList())
        {
            return;
        }

        foreach (ItemDrop drop in ItemsToDrop)
        {
            Vector3 randomPos = UnityEngine.Random.insideUnitSphere * 3;
            UnityEngine.Debug.Log($"Dropping {drop.AmountToDrop} of item: {drop.ItemToDropName}");
            ValidateDropTransform();
            drop.ItemToDropName.InstantiateItemInWorld(new Vector3(randomPos.x, 0, randomPos.z) + DropTransform.position);
        }
    }

    void ValidateDropTransform()
    {
        if(DropTransform == null)
        {
            DropTransform = PlayerBehavior.PlayerGameObject.transform;
        }
    }

    public void SetDropTransform(Transform tran)
    {
        DropTransform = tran;
    }
}
[Serializable]
public class ItemDrop {

    public string itemName;
    public int amountToDrop;
    public string ItemToDropName => itemName;
    public int AmountToDrop => Math.Max(0, amountToDrop);

    public Item ToDrop
    {
        get
        {
            return DatabaseContainer.gameData.GetItem(ItemToDropName);
        }
    }

    public static List<ItemDrop> GetListOfItemsToDrop(params (string, int)[] drops)
    {
        return drops.Select(d => new ItemDrop { itemName = d.Item1, amountToDrop = d.Item2 }).ToList();
    }

}

public interface IDistributeItems<T> where T: ItemDistributor
{
    T ItemDistributor { get; set; }

    List<ItemDrop> ItemDropData { get; set; }
}

public abstract class ItemDistributor
{
    public List<ItemDrop> ItemsToDrop { get; set; }

    public void SetItemsToDrop((string, int)[] dropData)
    {
        ItemsToDrop = ItemDrop.GetListOfItemsToDrop(dropData);
    }

    public void SetItemsToDrop(List<ItemDrop> dropData)
    {
        ItemsToDrop = new List<ItemDrop>(dropData);
    }

    public List<Item> GetItemsToDropFromGameData()
    {
        return ItemsToDrop.Select(i => i.ToDrop).ToList();
    }

    public abstract void Distribute();

}