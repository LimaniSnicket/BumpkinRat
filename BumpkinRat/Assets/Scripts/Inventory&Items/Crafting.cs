using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemCrafter
{
    public static event EventHandler<CraftingEventArgs> CraftedItem;

    public string actionItem, targetItem;

    public static bool TakingCraftingAction { get; private set; }

    Dictionary<CraftingAction, int> CraftingHistory { get; set; }
    public ItemCrafter() 
    {
        CraftingHistory = new Dictionary<CraftingAction, int>();
        actionItem = "null";
        targetItem = "null";
    }

    public void CraftRecipe(Recipe r, int amt)
    {
        if(CraftedItem != null)
        {
            CraftedItem(this, new CraftingEventArgs { craftedRecipe = r, craftedAmount = amt });
        }
    }

    public void TakeCraftingAction(MonoBehaviour host, CraftingAction craftingAction)
    {
        if (TakingCraftingAction)
        {
            return;
        }

        host.StartCoroutine(CraftingAction(craftingAction, 0.25f));
    }

    IEnumerator CraftingAction(CraftingAction craftingAction, float waitTime)
    {
        TakingCraftingAction = true;
        Debug.Log("Taking a crafting action: " + (craftingAction).ToString());
        CacheAction(craftingAction);
        yield return new WaitForSeconds(waitTime);
        TakingCraftingAction = false;
    }

    void CacheAction(CraftingAction action)
    {
        if (CraftingHistory == null)
        {
            CraftingHistory = new Dictionary<CraftingAction, int>();
        }

        CraftingHistory.Increment(action);
    }

    public void ClearCraftingHistory()
    {
        targetItem = "null";
        actionItem = "null";
        CraftingHistory.Clear();
    }

    public void SetItemTargets(string item)
    {
        if (actionItem.Equals("null"))
        {
            actionItem = item;
        }
        else
        {
            targetItem = item;
        }

        Debug.Log($"{actionItem} on {targetItem}");
    }
}

[Serializable]
public class CustomerCraftingOrder
{
    public int recipeLookup;

    public int craftingActions;
}

[Serializable]
public class CraftingInstruction : IComparable<CraftingInstruction>
{
    public int interactingId;
    public int targetId;

    public string[] craftingActionsToTake;

    public Dictionary<CraftingAction, int> ConvertedActionsTaken
    {
        get
        {
            Dictionary<CraftingAction, int> converted = new Dictionary<CraftingAction, int>();

            if (craftingActionsToTake.ValidArray())
            {
                for(int i = 0; i < craftingActionsToTake.Length; i++)
                {
                    KeyValuePair<CraftingAction, int> convertAction = ConvertStringToCraftingActionIntPair(craftingActionsToTake[i]);
                    converted.Add(convertAction.Key, convertAction.Value);
                }
            }

            return converted;
        }
    }

    char[] dividers => new char[]{':', ',', '/'};

    public int CompareTo(CraftingInstruction other)
    {
        int comp = other.interactingId.CompareTo(interactingId);
        if(comp == 0)
        {
            comp = other.targetId.CompareTo(targetId);
            if(comp == 0)
            {

            }
        }
        return comp;
    }

    KeyValuePair<CraftingAction, int> ConvertStringToCraftingActionIntPair(string converting)
    {
        bool properlyFormatted = converting.IndexOfAny(dividers) >= 0;
        return properlyFormatted 
            ? new KeyValuePair<CraftingAction, int>()
            : new KeyValuePair<CraftingAction, int>(CraftingAction.NONE, 0);
    }
}

public class CraftingEventArgs : EventArgs
{
    public Recipe craftedRecipe { get; set; }
    public int craftedAmount { get; set; }
}

public enum CraftingAction
{
    NONE = 0,
    PLACE = 1,
    ATTACH = 2,
    HAMMER = 3,
    THREAD = 4
}