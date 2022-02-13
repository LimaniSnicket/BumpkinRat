using System;
using System.Collections.Generic;
using UnityEngine.Events;

[Serializable]
public class CraftingSequence
{
    public ItemObjectBehaviour actionItemObject, targetItemObject;
    private string actionItemAtFocusArea, targetItemAtFocusArea;
    public CraftingAction actionTaken;

    public bool InProgress => actionItemObject != null;
    private string ActionItemProgress => actionItemObject != null ? $"{actionItemObject.Item.DisplayName} --> " : string.Empty;
    private string ActionTakenString => actionTaken.Equals(CraftingAction.NONE) ? string.Empty : $"{actionTaken.ToString()} --> ";
    private string TargetItemProgress => targetItemObject != null ? targetItemObject.Item.DisplayName : string.Empty;

    public static event EventHandler CraftingSequenceCompleted;
    public CraftingSequenceTaken onSequence;

    public void AddToCraftingSequence(IFocusArea focus, ItemObjectBehaviour obj)
    {
        if (actionItemObject == null)
        {
            actionItemObject = obj;
            actionItemAtFocusArea = focus.FocusArea.ToString();
        }
        else if (!actionItemObject.name.Equals(obj.name) && !actionTaken.Equals(CraftingAction.NONE))
        {
            targetItemObject = obj;
            targetItemAtFocusArea = focus.FocusArea.ToString();
        }
    }

    public void DemolishObjects(Dictionary<int, int> removeFrom)
    {
        RemoveItemObject(removeFrom, actionItemObject);
        RemoveItemObject(removeFrom, targetItemObject);
        ClearSequence();
    }

    private void RemoveItemObject(Dictionary<int, int> removingFrom, ItemObjectBehaviour toDestroy)
    {
        if (toDestroy != null)
        {
            removingFrom.Remove(toDestroy.ItemObjectId);
            toDestroy.ForceDestroy();
        }
    }

    // ToDO
    public void RegisterSuccessfulSequenceConclusion()
    {
        if (!IsValid())
        {
            return;
        }

    }

    public void ClearSequence()
    {
        actionItemObject = null;
        targetItemObject = null;
        actionItemAtFocusArea = string.Empty;
        targetItemAtFocusArea = string.Empty;
        actionTaken = CraftingAction.NONE;
    }

    public bool IsValid()
    {
        return targetItemObject != null;
    }

    public string GetSequenceProgressDisplay()
    {
        return $"{ActionItemProgress}{ActionTakenString}{TargetItemProgress}";
    }
    public override string ToString()
    {
        return $"Act({actionItemAtFocusArea})>{actionTaken}>Targ({targetItemAtFocusArea})";
    }

    public class CraftingSequenceTaken : UnityEvent
    {

    }
}
