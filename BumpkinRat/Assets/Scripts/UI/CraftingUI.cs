using System;
using System.Linq;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour, IUiFunctionality<CraftingMenu>
{
    public GameObject craftingButton;

    public List<CraftingActionButton> craftingActionButtons;

    public CraftingMenu MenuFunctionObject { get; set; }

    public ItemObjectUiElement itemObjectA, itemObjectB;

    void Start()
    {
        MenuFunctionObject = new CraftingMenu(gameObject);
        GenerateCraftingActionButtons(gameObject);
        itemObjectA = new ItemObjectUiElement(MenuFunctionObject.itemCrafter, transform, new Vector2(-400, -100));
        itemObjectB = new ItemObjectUiElement(MenuFunctionObject.itemCrafter, transform, new Vector2(400, -100));

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuFunctionObject.itemCrafter.ClearCraftingHistory();
        }
    }

    public void TakeCraftingActionViaCraftingUI(CraftingAction action)
    {
        MenuFunctionObject.itemCrafter.TakeCraftingAction(this, action);
    }

    public void GenerateCraftingActionButtons(GameObject parent)
    {
        craftingActionButtons = new List<CraftingActionButton>();
        for (int i = 0; i < Enum.GetValues(typeof(CraftingAction)).Length; i++)
        {
            GameObject newCraftingActionButton = Instantiate(craftingButton, transform);
            CraftingActionButton craftAction = new CraftingActionButton(newCraftingActionButton, i, this);
            craftAction.SetButtonPosition(new Vector2(-400, -500), Vector2.right * 330);

            craftingActionButtons.Add(craftAction);
        }
    }

}

[Serializable]
public class CraftingActionButton
{
    public Button button;
    public CraftingAction craftingAction;

    CraftingUI craftingMenuBehaviour;

    public CraftingActionButton(GameObject gameObject, int craftAction, CraftingUI crafter)
    {
        SetOrAddButton(gameObject);
        craftingAction = (CraftingAction)craftAction;
        craftingMenuBehaviour = crafter;

        SetButtonTextToCraftingAction();
        button.onClick.AddListener(OnClickTakeCraftingAction);
    }

    public CraftingActionButton(GameObject gameObject, CraftingAction craftAction, CraftingUI crafter)
    {
        SetOrAddButton(gameObject);
        craftingAction = craftAction;
        craftingMenuBehaviour = crafter;

        SetButtonTextToCraftingAction();
        button.onClick.AddListener(OnClickTakeCraftingAction);
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
