using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour, IUiFunctionality<CraftingMenu>
{
    public GameObject craftingButton;

    public CraftingMenu MenuFunctionObject { get; set; }
    public CraftingActionButton placeButton;

    void Start()
    {
        MenuFunctionObject = new CraftingMenu(gameObject);
        placeButton = new CraftingActionButton(craftingButton, 0);
    }

}

[Serializable]
public struct CraftingActionButton
{
    public Button button;
    public CraftingAction craftingAction;

    public CraftingActionButton(GameObject gameObject, int craftAction)
    {
        button = gameObject.GetComponent<Button>();
        craftingAction = (CraftingAction)craftAction;
        button.onClick.AddListener(OnClickTakeCraftingAction);
    }

    public CraftingActionButton(Button fromButton, int craftAction)
    {
        button = fromButton;
        craftingAction = (CraftingAction)craftAction;
        button.onClick.AddListener(OnClickTakeCraftingAction);
    }

    void OnClickTakeCraftingAction()
    {
        Debug.Log(craftingAction.ToString());
    }
}
