using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemObject : MonoBehaviour, IOccupyPositions
{
    public int itemId;

    Item item;

    public bool Altered { get; private set; }

    public int numberOfPositions;

    public string[] positionOccupiedBy;
    
    public bool MouseHoveringOnItemObject { get; private set; }
    Dictionary<int, FocusArea> FocusAreaLookup { get; set; }
    public OccupiedPosition Occupied { get; set; }

    public static event EventHandler<ItemObjectEventArgs> InteractedWithItemObject;
    public static event EventHandler<ItemEventArgs> PlaceItemBackInInventory;

    Dictionary<CraftingSequence, ItemObjectSnapshot> localItemObjectHistoryCache;

    private void Start()
    {
        positionOccupiedBy = Enumerable.Repeat("empty", Math.Max(numberOfPositions, 1)).ToArray();
        localItemObjectHistoryCache = new Dictionary<CraftingSequence, ItemObjectSnapshot>();
        SetFocusAreaDictionary();
    }

    private void Update()
    {
        if(NumberOfInFocusAreas() > 0 && Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Put item back into inventory!");
            PlaceItemBackInInventory.BroadcastEvent(this, new ItemEventArgs { ItemToPass = itemId.GetItem() });
            OccupiedPosition.Release(this);
            Destroy(gameObject);
        }
    }

    public void SetFromItem(Item item)
    {
        itemId = item.itemId;
        this.item = item;
    }

    private void OnMouseEnter()
    {
        gameObject.BroadcastMessage("OnItemObjectFocusChange", true);

        if (!ItemCrafter.CraftingSequenceActive)
        {
            return;
        }

        //InteractedWithItemObject.BroadcastEvent(this, new ItemObjectEventArgs { InteractedWith = this });
        MouseHoveringOnItemObject = true;
    }

    private void OnMouseExit()
    {
        if (NumberOfInFocusAreas() <= 0)
        {
            gameObject.BroadcastMessage("OnItemObjectFocusChange", false);

            MouseHoveringOnItemObject = false;
        }
    }

    public void ConnectFocusAreas(int thisFocusId, FocusArea other)
    {

    }

    void SetFocusAreaDictionary()
    {
        if (FocusAreaLookup == null)
        {
            FocusAreaLookup = new Dictionary<int, FocusArea>();
        }
    }

    internal void RegisterFocusArea(FocusArea focusArea)
    {
        SetFocusAreaDictionary();

        int id = focusArea.focusAreaId;

        while (FocusAreaLookup.ContainsKey(id))
        {
            id++;
        }

        focusArea.focusAreaId = id;

        FocusAreaLookup.Add(focusArea.focusAreaId, focusArea);

    }

    internal void BroadcastInteractionWithFocusArea(FocusArea focus)
    {
        ItemCrafter.BeginCraftingSequence();
        InteractedWithItemObject.BroadcastEvent(this, new ItemObjectEventArgs { InteractedWith = this, AtFocusArea = focus });
    }

    internal int NumberOfInFocusAreas()
    {
        if(FocusAreaLookup == null) { return -1; }

        return FocusAreaLookup.Where(k => k.Value.IsFocus).Count();
    }

    private void OnCollisionStay(Collision collision)
    {

    }

    internal void SuccessfullyCraftedWith(CraftingSequence craftingSequence)
    {
        localItemObjectHistoryCache.Add(craftingSequence, ItemObjectSnapshot.TakeSnapshot(transform));
    }

}

public class ItemObjectEventArgs: EventArgs
{
    public bool PutBack { get; set; }
    public ItemObject InteractedWith { get; set; }
    public FocusArea AtFocusArea { get; set; }
}

[Serializable]
public struct ItemObjectSnapshot
{
    Vector3 position;
    Vector3 scale;
    Quaternion rotation;

    public static ItemObjectSnapshot TakeSnapshot(Transform t)
    {
        return new ItemObjectSnapshot
        {
            position = t.position,
            scale = t.localScale,
            rotation = t.rotation
        };
    }

    public void RevertToSnapshot(Transform t)
    {
        t.position = position;
        t.rotation = rotation;
        t.localScale = scale;
    }

}
