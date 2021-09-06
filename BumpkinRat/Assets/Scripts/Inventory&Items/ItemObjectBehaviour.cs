using System;
using UnityEngine;
using Items;

public class ItemObjectBehaviour : MonoBehaviour, IContainFocusArea
{
    public Item Item { get; private set; }
    public FocusAreaHandler FocusAreaHandler { get; set; } = new FocusAreaHandler();

    public int ItemObjectId => Item.itemId;

    public static event EventHandler<ItemObjectEventArgs> InteractedWithItemObject;
    public static event EventHandler<ItemEventArgs> PlaceItemBackInInventory;

    public void SetItemFromId(int id)
    {
        Item = ItemDataManager.GetItemById(id);
    }

    public void BroadcastPlaceBack()
    {
        PlaceItemBackInInventory.BroadcastEvent(this, new ItemEventArgs { ItemToPass = Item });
    }

    public void BroadcastInteractedWith(IFocusArea focus)
    {
        InteractedWithItemObject.BroadcastEvent(this, new ItemObjectEventArgs
        {
            InteractedWith = this,
            AtFocusArea = focus
        });
    }

    public virtual void ForceDestroy() { }

    protected void ForceDestroy(IOccupyPositions occupier)
    {
        if (occupier.Occupied != null)
        {
            occupier.Occupied.ReleaseOccupier(occupier);
        }
        Destroy(gameObject);
    }
}