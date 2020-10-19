using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour, IUiFunctionality<CraftingMenu> //driver class
{
    public GameObject craftingButton;

    public GameObject craftingActionButtonParent;

    public GameObject ratPointer;

    public List<CraftingActionButton> craftingActionButtons;

    public CraftingMenu MenuFunctionObject { get; set; }

    public ConversationUi CraftingConversationBehavior => GetComponent<ConversationUi>();

    int numberOfTimesOpened;


    void Start()
    {
        MenuFunctionObject = new CraftingMenu(gameObject);
        MenuFunctionObject.SetCraftingActionButtons(craftingActionButtonParent, craftingButton, this);

        CraftingConversationBehavior.enabled = false; //not in conversation by default

        MenuFunctionObject.TailoredUiEvent += OnCraftingMenuStatusChange;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.C))
        {
            MenuFunctionObject.ChangeMenuStatus();
        }

        if (!MenuFunctionObject.Active)
        {
            return;
        }

        ratPointer.transform.position = Input.mousePosition;


        if (Input.GetMouseButtonUp(0))
        {
            OnMouseButtonUpEndCraftingSequence();
        }
    }

    void OnCraftingMenuStatusChange(object source, UiEventArgs args)
    {
        CraftingConversationBehavior.enabled = args.load;
        numberOfTimesOpened.IncrementIfTrue(args.load);
    }

    public void TakeCraftingActionViaCraftingUI(CraftingAction action)
    {
        MenuFunctionObject.itemCrafter.TakeCraftingAction(this, action);
    }

    private void OnMouseButtonUpEndCraftingSequence()
    {
        Debug.Log("End Crafting Sequence");
        ItemCrafter.EndCraftingSequence();
        MenuFunctionObject.itemCrafter.EndLocalCraftingSequence();
    }

    private void OnDestroy()
    {
        MenuFunctionObject.TailoredUiEvent -= OnCraftingMenuStatusChange;
    }

}
