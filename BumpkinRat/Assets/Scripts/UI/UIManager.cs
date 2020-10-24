using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager uiManager;
    public static bool MenuActive => ActiveMenus.ValidList();
    public static MenuType ActiveMenu { get; private set; }

    public static List<MenuType> ActiveMenus { get; private set; }

    private KeyCode exitCurrentMenuKeyCode;

    private void Awake()
    {
        if (uiManager == null) { uiManager = this; } else { Destroy(this); }
        UiMenu.UiEvent += OnUiEvent;
        ActiveMenus = new List<MenuType>();
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        ActiveMenus.HandleInstanceObjectInList(args.menuLoaded, args.load);
    }

    private void OnDisable()
    {
        UiMenu.UiEvent -= OnUiEvent;   
    }
}

[Serializable]
public abstract class UiMenu
{
    public MenuType menuType { get; protected set; }
    protected GameObject gameObject;

    public bool exitDisabled;

    public bool entryDisabled;

    public bool Active { get; private set; }
    public abstract KeyCode ActivateKeyCode { get; }
    public abstract void LoadMenu();
    public abstract void CloseMenu();

    public static event EventHandler<UiEventArgs> UiEvent;
    public event EventHandler<UiEventArgs> TailoredUiEvent;

    public void ChangeMenuStatus()
    {
        if (Active && !exitDisabled)
        {
            CloseMenu();
        } 
        else if(!Active && !entryDisabled)
        {
            LoadMenu();
        }
    }

    public void BroadcastUiEvent(bool load)
    {
        UiEventArgs eventToSend = new UiEventArgs { load = load, menuLoaded = menuType };
        if (UiEvent != null)
        {
            Debug.LogFormat("{0} a menu of type: " + menuType, load ? "Loading": "Closing");
            UiEvent(this, eventToSend) ;
        }

        if(TailoredUiEvent != null)
        {
            TailoredUiEvent(this, eventToSend);
        }

        Active = load;
    }
}

[Serializable]
public class CraftingMenu : UiMenu
{
    public ItemCrafter itemCrafter;
    public override KeyCode ActivateKeyCode => throw new NotImplementedException();

    public UiElementContainer craftingButtonContainer;
    public CraftingMenu(GameObject g)
    {
        gameObject = g;
        menuType = MenuType.Crafting;
        itemCrafter = new ItemCrafter();
    }

    public void SetCraftingActionButtons(GameObject container, GameObject prefab, CraftingUI driver)
    {
        craftingButtonContainer = container.GetOrAddComponent<UiElementContainer>();
        GenerateCraftingActionButtons(prefab, driver);
    }

    void GenerateCraftingActionButtons(GameObject prefab, CraftingUI driver)
    {
        //Rect dimensions = prefab.GetComponent<RectTransform>().rect;

        for (int i = 0; i < Enum.GetValues(typeof(CraftingAction)).Length; i++)
        {
            craftingButtonContainer.SpawnAtAlternatingVerticalPositions(prefab, 100, 150);
            CraftingActionButton craftAction = craftingButtonContainer.GetLastChild().GetComponent<CraftingActionButton>();//CraftingActionButton.GetCraftingButtonFromGameObject(newCraftingActionButton);
            craftAction.SetCraftingActionButton(i, driver);

        }

        craftingButtonContainer.gameObject.SetActive(false);
    }

    public override void CloseMenu()
    {
        Cursor.visible = true;
        craftingButtonContainer.gameObject.SetActive(false);
        itemCrafter.ClearCraftingHistory();
        BroadcastUiEvent(false);
    }

    public override void LoadMenu()
    {
        Cursor.visible = false;
        craftingButtonContainer.gameObject.SetActive(true);
        BroadcastUiEvent(true);
    }
}

[Serializable]
public class InventoryMenu : UiMenu
{
    public override KeyCode ActivateKeyCode => KeyCode.Y;

    public InventoryMenu(GameObject g)
    {
        gameObject = g;
        menuType = MenuType.Inventory;
        gameObject.SetActive(false);
    }

    public override void CloseMenu()
    {
        BroadcastUiEvent(false);
        gameObject.SetActive(false);
    }

    public override void LoadMenu()
    {
        BroadcastUiEvent(true);
        gameObject.SetActive(true);

    }

    public void LoadMenu(Inventory i)
    {
        LoadMenu();
    }

}

[Serializable]
public class DialogueMenu : UiMenu
{
    public TextMeshProUGUI dialogueDisplay { get; protected set; }
    bool acceptInputFromRunner;

    public override KeyCode ActivateKeyCode => KeyCode.None;

    public DialogueMenu(GameObject g)
    {
        gameObject = g;
        menuType = MenuType.Dialogue;
        dialogueDisplay = g.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        dialogueDisplay.text = "";
    }

    public override void LoadMenu()
    {
        BroadcastUiEvent(true);
        dialogueDisplay.text = "";
        acceptInputFromRunner = true;
        Debug.Log("Dialogue Menu ready for input");
    }

    public override void CloseMenu()
    {
        BroadcastUiEvent(false);
        acceptInputFromRunner = false;
        dialogueDisplay.text = "";
    }

    public void UpdateDialogueDisplay(string display)
    {
        if (!acceptInputFromRunner) { return; }
        dialogueDisplay.text = display;
    }
}

public class UiEventArgs : EventArgs
{
    public bool load { get; set; }
    public MenuType menuLoaded { get; set; }

    public KeyCode EscapeKey { get; set; }
}

public interface IUiFunctionality<T> where T: UiMenu
{
    T MenuFunctionObject { get; set; }
}
