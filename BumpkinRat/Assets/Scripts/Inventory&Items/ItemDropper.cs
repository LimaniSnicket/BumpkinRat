using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ItemDropper
{
    public ItemDrop[] dropping;

    void Drop()
    {
        foreach(ItemDrop i in dropping)
        {
            Debug.Log(i.toDrop.display);
        }
    }
}
[Serializable]
public class ItemDrop {

    public Item ToDrop { get; set; }
    public int AmountToDrop { get; set; }

    public Item toDrop;
    public int amount;

}

public interface IDropItem
{
    ItemDropper ItemDropper { get; set; }
    string[] DropData { get; }
}