using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager uiManager;
    public static bool menuActive { get; private set; }
    public static MenuType activeMenu { get; private set; }

    private KeyCode exitCurrentMenuKeyCode;

    private void Awake()
    {
        if (uiManager == null) { uiManager = this; } else { Destroy(this); }
        UiMenu.UiEvent += OnUiEvent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) { 
            menuActive = !menuActive; 
        }
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        menuActive = args.load;
        Debug.Log("Menu Status: " + args.load);
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

    public abstract KeyCode ActivateKeyCode { get; }
    public abstract void LoadMenu();
    public abstract void CloseMenu();

    public static event EventHandler<UiEventArgs> UiEvent;

    public void BroadcastUiEvent(bool load)
    {
        if (UiEvent != null)
        {
            Debug.LogFormat("{0} a menu of type: " + menuType, load ? "Loading": "Closing");
            UiEvent(this, new UiEventArgs { load = load, menuLoaded = menuType }) ;
        }
    }
}

[Serializable]
public class CraftingMenu : UiMenu
{
    public ItemCrafter itemCrafter;
    public override KeyCode ActivateKeyCode => throw new NotImplementedException();
    public CraftingMenu(GameObject g)
    {
        gameObject = g;
        menuType = MenuType.Crafting;
        itemCrafter = new ItemCrafter();
    }

    public override void CloseMenu()
    {
        BroadcastUiEvent(false);
    }

    public override void LoadMenu()
    {
        BroadcastUiEvent(true);
    }
}

[Serializable]
public class InventoryMenu : UiMenu
{
    TextMeshProUGUI displayInventory;
    public override KeyCode ActivateKeyCode => KeyCode.Y;

    public InventoryMenu(GameObject g)
    {
        gameObject = g;
        menuType = MenuType.Inventory;
        displayInventory = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        displayInventory.gameObject.SetActive(false);

    }

    public override void CloseMenu()
    {
        BroadcastUiEvent(false);
        displayInventory.gameObject.SetActive(false);
    }

    public override void LoadMenu()
    {
        BroadcastUiEvent(true);
    }

    public void LoadMenu(Inventory i)
    {
        LoadMenu();

        displayInventory.gameObject.SetActive(true);

        StringBuilder builder = new StringBuilder();

        foreach(KeyValuePair<string, int> pair in i.inventoryListings)
        {
            builder.AppendLine($"{pair.Key}: {pair.Value}");
        }

        SetDisplayText(builder.ToString());
    }

    void SetDisplayText(string message)
    {
        if(displayInventory != null)
        {
            displayInventory.text = message;
        }
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

public struct CraftingActionButton
{
    public Button button;
    public CraftingAction craftingAction;
}
