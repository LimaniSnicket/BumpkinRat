using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class ItemCrafter
{
    public static event EventHandler<CraftingEventArgs> CraftedItem;

    public CraftingSequence activeSequence;
    Stack<CraftingSequence> completedCraftingSequences; //use this for caching itemObject positions and rotations to undo crafting actions!
    public static bool TakingCraftingAction { get; private set; }

    public static bool CraftingSequenceActive { get; private set; }

    Dictionary<CraftingAction, int> CraftingHistory { get; set; }

    Dictionary<int, int> activeItemObjects;
    RecipeProgressTracker progressTracker;
    
    public ItemCrafter() 
    {
        CraftingHistory = new Dictionary<CraftingAction, int>();

        activeSequence = new CraftingSequence();
        completedCraftingSequences = new Stack<CraftingSequence>();

        activeItemObjects = new Dictionary<int, int>();

        progressTracker = new RecipeProgressTracker();
        RecipeProgressTracker.onRecipeCompleted.AddListener(PrintRecipe);

        ItemObject.InteractedWithItemObject += OnInteractedWithItemObject;
        ItemObject.PlaceItemBackInInventory += OnItemObjectPlacedBack;
        InventoryButton.InventoryButtonPressed += OnInventoryButtonPressed;
    }

    private void OnItemObjectPlacedBack(object sender, ItemEventArgs e)
    {
        KeyValuePair<int, int> altered;
        activeItemObjects.Decrement(e.ItemToPass.itemId, out altered);
        progressTracker.RemoveFromCache(altered);
    }

    void OnInteractedWithItemObject(object source, ItemObjectEventArgs args)
    {
        activeSequence.SetFromItemObjectEventArgs(args);
    }

    void OnInventoryButtonPressed(object source, InventoryButtonArgs args)
    {
        activeItemObjects.Increment(args.ItemId);
        progressTracker.AddToCache(activeItemObjects);
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
        CraftingHistory.Clear();
        completedCraftingSequences.Clear();
    }

    public static void BeginCraftingSequence()
    {
        CraftingSequenceActive = true;
    }

    public static void EndCraftingSequence()
    {
        CraftingSequenceActive = false;
        CraftingActionButton.ResetCraftingButtonActivation();
    }

    public void EndLocalCraftingSequence()
    {
        if (activeSequence.IsValid())
        { 
            Debug.Log(activeSequence.ToString());
            activeSequence.RegisterSuccessfulSequenceConclusion();
            completedCraftingSequences.Push(activeSequence);

            activeSequence = new CraftingSequence();
            progressTracker.RegisterCraftingSequenceProgress(completedCraftingSequences.Peek(), activeItemObjects);
        }

        activeSequence.ClearSequence();
    }

    void PrintRecipe(Recipe r)
    {
        Debug.Log($"{r.GetOutputItem().DisplayName}:{r.recipeDescription}");
    }

    public void UnsubscribeToEvents()
    {
        ItemObject.InteractedWithItemObject -= OnInteractedWithItemObject;
        InventoryButton.InventoryButtonPressed -= OnInventoryButtonPressed;
        ItemObject.PlaceItemBackInInventory += OnItemObjectPlacedBack;
    }
}

[Serializable]
public struct CraftingSequence
{
    public ItemObject actionItemObject, targetItemObject;
    public string actionItemAtFocusArea, targetItemAtFocusArea;
    public CraftingAction actionTaken;

    public static event EventHandler CraftingSequenceCompleted;
    public CraftingSequenceTaken onSequence; 

    public (KeyValuePair<int, int>, KeyValuePair<int, int>) GetKeyValues()
    {
        KeyValuePair<int, int> defaultPair = new KeyValuePair<int, int>(-1, -1);
        return (actionItemObject == null ? defaultPair : actionItemObject.AsKeyValue, 
            targetItemObject == null ? defaultPair : targetItemObject.AsKeyValue);
    }

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

    public void DemolishObjects(Dictionary<int, int> removeFrom)
    {
        if(actionItemObject != null) { removeFrom.Remove(actionItemObject.itemId); }
        if (targetItemObject != null) { removeFrom.Remove(targetItemObject.itemId); }

        ItemObject.DestroyItemObjects(actionItemObject, targetItemObject);
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

    void OnSequence()
    {
        if(onSequence == null)
        {
            onSequence = new CraftingSequenceTaken();
        }

        onSequence.Invoke();
    }

    public override string ToString()
    {
        return $"Action({actionItemAtFocusArea}) --> {actionTaken} --> Target({targetItemAtFocusArea})";
    }

    public class CraftingSequenceTaken : UnityEvent
    {

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
    THREAD = 4,
    GLUE = 5
}