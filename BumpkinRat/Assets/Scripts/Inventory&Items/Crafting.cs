using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemCrafter
{
    public static event EventHandler<CraftingEventArgs> CraftedItem;

    public string actionItem, targetItem, actionToTake;

    public CraftingSequence activeSequence;
    Stack<CraftingSequence> completedCraftingSequences; //use this for caching itemObject positions and rotations to undo crafting actions!
    public static bool TakingCraftingAction { get; private set; }

    public static bool CraftingSequenceActive { get; private set; }

    Dictionary<CraftingAction, int> CraftingHistory { get; set; }
    public ItemCrafter() 
    {
        CraftingHistory = new Dictionary<CraftingAction, int>();
        actionItem = "null";
        actionToTake = "NONE";
        targetItem = "null";

        activeSequence = new CraftingSequence();
        completedCraftingSequences = new Stack<CraftingSequence>();

        ItemObject.InteractedWithItemObject += OnInteractedWithItemObject;
    }

    void OnInteractedWithItemObject(object source, ItemObjectEventArgs args)
    {
        activeSequence.SetFromItemObjectEventArgs(args);

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

        host.StartCoroutine(CraftingAction(craftingAction, 0f));
    }

    IEnumerator CraftingAction(CraftingAction craftingAction, float waitTime)
    {
        TakingCraftingAction = true;
        actionToTake = craftingAction.ToString();

        activeSequence.actionTaken = craftingAction;

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
        completedCraftingSequences.Clear();
        activeSequence.ClearSequence();
    }

    public static void BeginCraftingSequence()
    {
        CraftingSequenceActive = true;
    }

    public static void EndCraftingSequence()
    {
        CraftingSequenceActive = false;
    }

    public void EndLocalCraftingSequence()
    {
        if (activeSequence.IsValid())
        { 
            Debug.Log(activeSequence.ToString());
            activeSequence.RegisterSuccessfulSequenceConclusion();
            completedCraftingSequences.Push(activeSequence);
            activeSequence = new CraftingSequence();
        }

        activeSequence.ClearSequence();
    }

    public void UnsubscribeToEvents()
    {
        ItemObject.InteractedWithItemObject += OnInteractedWithItemObject;
    }
}

[Serializable]
public struct CraftingSequence
{
    public ItemObject actionItemObject, targetItemObject;
    public string actionItemAtFocusArea, targetItemAtFocusArea;
    public CraftingAction actionTaken;

    public static event EventHandler CraftingSequenceCompleted;

    public void SetFromItemObjectEventArgs(ItemObjectEventArgs args)
    {
        if(actionItemObject == null)
        {
            actionItemObject = args.InteractedWith;
            actionItemAtFocusArea = args.AtFocusArea.ToString();

        } else
        {
            if (!UniqueItemObject(args.InteractedWith))
            {
                return;
            }

            targetItemObject = args.InteractedWith;
            targetItemAtFocusArea = args.AtFocusArea.ToString();
        }
    }

    public void RegisterSuccessfulSequenceConclusion()
    {
        if (!IsValid())
        {
            return;
        }

        actionItemObject.SuccessfullyCraftedWith(this);
    }

    public void ClearSequence()
    {
        actionItemObject = null;
        targetItemObject = null;
        actionItemAtFocusArea = string.Empty;
        targetItemAtFocusArea = string.Empty;
        actionTaken = CraftingAction.NONE;
    }

    bool UniqueItemObject(ItemObject obj)
    {
        return actionItemObject != obj;
    }

    public bool IsValid()
    {
        return actionItemObject != null && targetItemObject != null && !actionTaken.Equals(CraftingAction.NONE);
    }

    public void BroadcastCraftingSequenceTaken()
    {
        
    }

    public override string ToString()
    {
        return $"Action Item: {actionItemAtFocusArea} --> {actionTaken} --> Target Item: {targetItemAtFocusArea}";
    }
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

    char[] dividers = new char[]{':', ',', '/'};

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