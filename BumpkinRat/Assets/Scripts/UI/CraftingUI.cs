using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class CraftingUI : MonoBehaviour, IUiFunctionality<CraftingMenu>
{
    public GameObject craftingButton;

    public List<CraftingActionButton> craftingActionButtons;

    public CraftingMenu MenuFunctionObject { get; set; }

    public ConversationUi CraftingConversationBehavior => GetComponent<ConversationUi>();

    public ItemObjectUiElement itemObjectA, itemObjectB;

    public ItemObject itemObjectFocus;


    void Start()
    {
        MenuFunctionObject = new CraftingMenu(gameObject);
        GenerateCraftingActionButtons(gameObject);

        CraftingConversationBehavior.enabled = false; //not in conversation by default

        MenuFunctionObject.TailoredUiEvent += OnCraftingMenuStatusChange;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuFunctionObject.CloseMenu();
            MenuFunctionObject.itemCrafter.ClearCraftingHistory();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            MenuFunctionObject.LoadMenu();
        }

        if (!MenuFunctionObject.Active)
        {
            Debug.Log("Crafting not available. Open Menu to craft.");
            itemObjectFocus = null;
            return;
        }

        Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rh = new RaycastHit();
        if (Physics.SphereCast(r, .5f, out rh, Mathf.Infinity))
        {
            if(Input.GetMouseButtonDown(0) && rh.RaycastOnComponentOf<ItemObject>(out itemObjectFocus))
            {
                Debug.Log("Staring Crafting Process?");
            } else
            {
                itemObjectFocus = null;
            }
        } else
        {
            itemObjectFocus = null;
        }
    }

    void OnCraftingMenuStatusChange(object source, UiEventArgs args)
    {
        CraftingConversationBehavior.enabled = args.load;
        Debug.Log($"Toggling Crafting Conversations {(args.load ? "On" : "Off")}");
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
            CraftingActionButton craftAction = CraftingActionButton.GetCraftingButtonFromGameObject(newCraftingActionButton);
            craftAction.SetCraftingActionButton(i, this);
            craftAction.SetButtonPosition(new Vector2(-750, -500), Vector2.right * 305);

            craftingActionButtons.Add(craftAction);
        }
    }

    void SpawnItemObjectButtons()
    {
        itemObjectA = new ItemObjectUiElement(MenuFunctionObject.itemCrafter, transform, new Vector2(-400, -100));
        itemObjectB = new ItemObjectUiElement(MenuFunctionObject.itemCrafter, transform, new Vector2(400, -100));
    }

    private void OnDestroy()
    {
        MenuFunctionObject.TailoredUiEvent -= OnCraftingMenuStatusChange;
    }

}
