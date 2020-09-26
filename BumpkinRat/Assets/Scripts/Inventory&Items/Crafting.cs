using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCrafter
{
    public bool crafting;
    public static event EventHandler<CraftingEventArgs> CraftedItem;

    public static bool TakingCraftingAction { get; private set; }
    public ItemCrafter() { }

    public void CraftRecipe(Recipe r, int amt)
    {
        if(CraftedItem != null)
        {
            CraftedItem(this, new CraftingEventArgs { craftedRecipe = r, craftedAmount = amt });
        }
    }

    public void TakeCraftingAction(MonoBehaviour host, int craftingAction)
    {
        host.StartCoroutine(CraftingAction(craftingAction, 0.25f));
    }

    IEnumerator CraftingAction(int craftingAction, float waitTime)
    {
        TakingCraftingAction = true;
        Debug.Log("Taking a crafting action: " + ((CraftingAction)craftingAction).ToString());
        yield return new WaitForSeconds(waitTime);
        TakingCraftingAction = false;
    }
}

[Serializable]
public class CraftingInstruction
{
    public int interactingId;
    public int targetId;

}

public class CraftingEventArgs : EventArgs
{
    public Recipe craftedRecipe { get; set; }
    public int craftedAmount { get; set; }
}

public enum CraftingAction
{
    PLACE = 0,
    ATTACH = 1,
    HAMMER = 2,
    THREAD = 3
}