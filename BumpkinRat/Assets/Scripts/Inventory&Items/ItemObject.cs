using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    public int itemId;

    Item item;

    public int numberOfPositions;

    public string[] positionOccupiedBy;
    
    public bool MouseHoveringOnItemObject { get; private set; }
    Dictionary<int, FocusArea> FocusAreaLookup { get; set; }

    public static event EventHandler<ItemObjectEventArgs> InteractedWithItemObject;

    private void Start()
    {
        positionOccupiedBy = Enumerable.Repeat("empty", Math.Max(numberOfPositions, 1)).ToArray();
        SetFocusAreaDictionary();
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

        InteractedWithItemObject.BroadcastEvent(this, new ItemObjectEventArgs { InteractedWith = this });
        MouseHoveringOnItemObject = true;
    }

    private void OnMouseExit()
    {
        gameObject.BroadcastMessage("OnItemObjectFocusChange", false);

        MouseHoveringOnItemObject = false;
    }

    private void OnMouseDown()
    {
        ItemCrafter.BeginCraftingSequence();
        InteractedWithItemObject.BroadcastEvent(this, new ItemObjectEventArgs { InteractedWith = this });
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
        FocusAreaLookup.Add(focusArea.focusAreaId, focusArea);
    }
}

public class ItemObjectEventArgs: EventArgs
{
    public ItemObject InteractedWith { get; set; }
}
