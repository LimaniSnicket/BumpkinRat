using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CraftingActionButton: MonoBehaviour, IPointerEnterHandler
{
    public Button button;
    public CraftingAction craftingAction;

    private CraftingUI craftingMenuBehaviour;


    public static CraftingActionButton GetCraftingButtonFromGameObject(GameObject gameObject)
    {
        try
        {
            return gameObject.GetComponent<CraftingActionButton>();
        }
        catch (NullReferenceException)
        {
            return gameObject.AddComponent<CraftingActionButton>();
        }
    }

    public void SetCraftingActionButton(int craftAction, CraftingUI crafter)
    {
        SetOrAddButton(gameObject);
        craftingAction = (CraftingAction)craftAction;
        craftingMenuBehaviour = crafter;

        SetButtonTextToCraftingAction();
    }

    public void SetCraftingActionButton(CraftingAction craftAction, CraftingUI crafter)
    {
        SetOrAddButton(gameObject);
        craftingAction = craftAction;
        craftingMenuBehaviour = crafter;

        SetButtonTextToCraftingAction();
    }


    void SetOrAddButton(GameObject gameObject)
    {
        try
        {
            button = gameObject.GetComponent<Button>();
        }
        catch (NullReferenceException)
        {
            button = gameObject.AddComponent<Button>();
        }
    }

    void SetButtonTextToCraftingAction()
    {
        if(button != null)
        {
            button.GetComponentInChildren<TextMeshProUGUI>().text = craftingAction.ToString();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ItemCrafter.CraftingSequenceActive)
        {
            return;
        }

        OnClickTakeCraftingAction();
    }

    void OnClickTakeCraftingAction()
    {
        craftingMenuBehaviour.TakeCraftingActionViaCraftingUI(craftingAction);
    }

    public void SetButtonPosition(Vector2 positionInCanvas)
    {
        if(button != null)
        {
            button.GetComponent<RectTransform>().localPosition = positionInCanvas;
        }
    }

    public void SetButtonPosition(Vector2 startingPosition, Vector2 padding)
    {
        Vector2 position = startingPosition +  padding * (int)craftingAction;
        SetButtonPosition(position);
    }
}
