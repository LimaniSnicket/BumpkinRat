using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class CraftingActionButton: MonoBehaviour, IPointerEnterHandler
{
    public Button button;
    Image image;
    public CraftingAction craftingAction;

    private CraftingUI craftingMenuBehaviour;

    private static CraftingActionButtonActive craftingActionButtonActivated;

    private Color activeColor, inactiveColor;

    private void Start()
    {
        if(craftingActionButtonActivated == null)
        {
            craftingActionButtonActivated = new CraftingActionButtonActive();
        }

        craftingActionButtonActivated.AddListener(OnButtonActivated);
        image = gameObject.GetOrAddComponent<Image>();
        inactiveColor = image.color;
        activeColor = Color.white;
    }

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

    void SetActivityColor(bool active)
    {
        image.color = active ? activeColor : inactiveColor;
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
        craftingActionButtonActivated.Invoke(craftingAction);
    }

    public static void ResetCraftingButtonActivation()
    {
        craftingActionButtonActivated.Invoke(CraftingAction.NONE);
    }

    void OnButtonActivated(CraftingAction action)
    {
        bool thisButtonActive = action == craftingAction;
        SetActivityColor(thisButtonActive);
        float scaleModifier = thisButtonActive ? 1.3f : 1f;
        transform.localScale = Vector3.one * scaleModifier;
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

    private void OnDestroy()
    {
        try
        {
            craftingActionButtonActivated.RemoveListener(OnButtonActivated);

        } catch (NullReferenceException)
        {

        }

    }


    class CraftingActionButtonActive : UnityEvent<CraftingAction> { }
}
