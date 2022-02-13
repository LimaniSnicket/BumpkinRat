using System;
using System.Collections;
using System.Collections.Generic;
using BumpkinRat.Crafting;
using Items;
using UnityEngine;

public class ItemCrafter
{
    public static event EventHandler<CraftingEventArgs> CraftedItem;

    private static CraftingSequence activeSequence;

    private readonly Stack<CraftingSequence> completedCraftingSequences; //use this to undo crafting actions?
    public static bool TakingCraftingAction { get; private set; }

    public static bool CraftingSequenceActive => activeSequence.InProgress;

    public static string ActiveCraftingSequenceProgressString => activeSequence.GetSequenceProgressDisplay();

//    Dictionary<CraftingAction, int> CraftingHistory { get; set; }

    private readonly RecipeProgressTracker progressTracker;
    
    public ItemCrafter() 
    {
        // CraftingHistory = new Dictionary<CraftingAction, int>();

        activeSequence = new CraftingSequence();
        completedCraftingSequences = new Stack<CraftingSequence>();

        progressTracker = new RecipeProgressTracker();
        RecipeProgressTracker.onRecipeCompleted.AddListener(PrintRecipe);

        ItemObjectBehaviour.InteractedWithItemObject += OnInteractedWithItemObject;
        ItemObjectBehaviour.PlaceItemBackInInventory += OnItemObjectPlacedBack;
        InventoryButton.InventoryButtonPressed += OnInventoryButtonPressed;
    }

    private void OnItemObjectPlacedBack(object sender, ItemEventArgs e)
    {
        progressTracker.DecrementItemAmount(e.ItemToPass.itemId);
    }

    private void OnInteractedWithItemObject(object source, ItemObjectEventArgs args)
    {
        activeSequence.AddToCraftingSequence(args.AtFocusArea, args.InteractedWith);
    }

    private void OnInventoryButtonPressed(object source, ItemEventArgs args)
    {
        progressTracker.IncrementItemAmount(args.ItemToPass.itemId);
    }

    public void ExecuteCraftingAction(MonoBehaviour host, CraftingAction craftingAction)
    {
        if (TakingCraftingAction)
        {
            return;
        }

        host.StartCoroutine(TakeCraftingAction(craftingAction, 0f));
    }

    private IEnumerator TakeCraftingAction(CraftingAction craftingAction, float waitTime)
    {
        TakingCraftingAction = true;

        activeSequence.actionTaken = craftingAction;

        yield return new WaitForSeconds(waitTime);
        TakingCraftingAction = false;
    }

    public void ClearCraftingHistory()
    {
        completedCraftingSequences.Clear();
    }

    public static void BeginCraftingSequence(IFocusArea focusArea, ItemObjectBehaviour itemObject)
    {
        activeSequence.AddToCraftingSequence(focusArea, itemObject);   
    }

    public static void EndCraftingSequence()
    {
        CraftingActionWidget.ResetAll();
    }

    public void StoreCompletedCraftingSequence()
    {
        if (activeSequence.IsValid())
        { 
            Debug.Log(activeSequence.ToString());
            activeSequence.RegisterSuccessfulSequenceConclusion();
            completedCraftingSequences.Push(activeSequence);

            activeSequence = new CraftingSequence();
            progressTracker.RegisterCraftingSequenceProgress(completedCraftingSequences.Peek());
        }

        activeSequence.ClearSequence();
    }

    private void PrintRecipe(Recipe r)
    {
        CustomerOrderManager.EvaluateRecipeBasedOnCustomerOrder(r);
        string itemName = ItemDataManager.GetItemById(r.outputId).DisplayName;
        Debug.Log($"{itemName}:{r.recipeDescription}");
    }

    public void UnsubscribeToEvents()
    {
        ItemObjectBehaviour.InteractedWithItemObject -= OnInteractedWithItemObject;
        InventoryButton.InventoryButtonPressed -= OnInventoryButtonPressed;
        ItemObjectBehaviour.PlaceItemBackInInventory -= OnItemObjectPlacedBack;
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
    ATTACH = 1,
    HAMMER = 2,
    THREAD = 3,
    GLUE = 4
}