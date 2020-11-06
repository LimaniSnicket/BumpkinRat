using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CraftingUI : MonoBehaviour, IUiFunctionality<CraftingMenu> //driver class
{
    static CraftingUI craftingUI;

    public GameObject craftingButton;

    public GameObject craftingActionButtonParent;

    public GameObject ratPointer;

    public List<CraftingActionButton> craftingActionButtons;

    public static float distraction = 0.25f;

    public CraftingMenu MenuFunctionObject { get; set; }

    public ConversationUi CraftingConversationBehavior => GetComponent<ConversationUi>();

    int numberOfTimesOpened;


    void Awake()
    {
        if (craftingUI == null)
        {
            craftingUI = this;
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
       

        MenuFunctionObject = new CraftingMenu(gameObject);
        MenuFunctionObject.SetCraftingActionButtons(craftingActionButtonParent, craftingButton, this);

        CraftingConversationBehavior.enabled = false; //not in conversation by default

        MenuFunctionObject.TailoredUiEvent += OnCraftingMenuStatusChange;
    }

    void Update()
    {
        Debug.Log(craftingUI.MenuFunctionObject == null);

        if (MenuFunctionObject.entryDisabled)
        {
            return;
        }

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

    public static void SetDisableCraftingMenuEntry(bool value)
    {
        try
        {
            craftingUI.MenuFunctionObject.entryDisabled = value;
        }
        catch (NullReferenceException)
        {
            craftingUI.StartCoroutine(WaitToEditMenuAvailability(true, value));
        }
    }

    public static void SetDisableCraftingMenuExit(bool value)
    {
        craftingUI.StartCoroutine(WaitToEditMenuAvailability(false, value));
    }

    static IEnumerator WaitToEditMenuAvailability(bool entry, bool disable)
    {
        while(craftingUI.MenuFunctionObject == null)
        {
            yield return new WaitForEndOfFrame();
        }
        if (entry)
        {
            craftingUI.MenuFunctionObject.entryDisabled = disable;
        } else
        {
            craftingUI.MenuFunctionObject.exitDisabled = disable;
        }
    }

    public static void LockPlayerInCrafting(bool openMenu = false)
    {
        SetDisableCraftingMenuEntry(false);
        SetDisableCraftingMenuExit(true);

        if (openMenu)
        {
            craftingUI.MenuFunctionObject.LoadMenu();
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
