using System;
using UnityEngine;
using Items;

public class ItemObjectBehaviour : MonoBehaviour, IContainFocusArea
{
    public ItemObject itemObject;
    public FocusAreaHandler FocusAreaHandler { get; set; } = new FocusAreaHandler();

    public int ItemObjectId => itemObject.Item.itemId;

    public virtual void ForceDestroy()
    {

    }

    protected void ForceDestroy<T>(IOccupyPositions<T> occupier) where T: Transform
    {
        OccupiablePosition.Release(occupier);
        Destroy(gameObject);
    }
}

[Serializable]
public class ItemObject
{
    public Item Item { get; private set; }

    public static event EventHandler<ItemObjectEventArgs> InteractedWithItemObject;
    public static event EventHandler<ItemEventArgs> PlaceItemBackInInventory;

    public ItemObject() { }

    public ItemObject(int id)
    {
        Item = ItemDataManager.GetItemById(id);
    }

    public void BroadcastPlaceBack()
    {
        PlaceItemBackInInventory.BroadcastEvent(this, new ItemEventArgs { ItemToPass = Item });
    }

    public void BroadcastInteractedWith(IFocusArea focus, ItemObjectBehaviour parent)
    {
        InteractedWithItemObject.BroadcastEvent(this, new ItemObjectEventArgs
        {
            InteractedWith = parent,
            AtFocusArea = focus
        });
    }
}
