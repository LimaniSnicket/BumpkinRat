﻿using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private static UIManager uiManager;
    public static bool MenuActive { get; private set; }
    public static MenuType ActiveMenu { get; private set; }

    private KeyCode exitCurrentMenuKeyCode;

    private void Awake()
    {
        if (uiManager == null) { uiManager = this; } else { Destroy(this); }
        UiMenu.UiEvent += OnUiEvent;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) { 
            MenuActive = !MenuActive; 
        }
    }

    void OnUiEvent(object source, UiEventArgs args)
    {
        MenuActive = args.load;
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

    public bool Active { get; private set; }
    public abstract KeyCode ActivateKeyCode { get; }
    public abstract void LoadMenu();
    public abstract void CloseMenu();

    public static event EventHandler<UiEventArgs> UiEvent;
    public event EventHandler<UiEventArgs> TailoredUiEvent;

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

    GameObject craftingButtonContainer;
    List<CraftingActionButton> craftingActionButtons;
    public CraftingMenu(GameObject g)
    {
        gameObject = g;
        menuType = MenuType.Crafting;
        itemCrafter = new ItemCrafter();
    }

    public void SetCraftingActionButtons(GameObject container, GameObject prefab, CraftingUI driver)
    {
        craftingButtonContainer = container;
        GenerateCraftingActionButtons(prefab, driver);
    }

    void GenerateCraftingActionButtons(GameObject prefab, CraftingUI driver)
    {
        craftingActionButtons = new List<CraftingActionButton>();
        for (int i = 0; i < Enum.GetValues(typeof(CraftingAction)).Length; i++)
        {
            GameObject newCraftingActionButton = GameObject.Instantiate(prefab, craftingButtonContainer.transform);
            CraftingActionButton craftAction = CraftingActionButton.GetCraftingButtonFromGameObject(newCraftingActionButton);
            craftAction.SetCraftingActionButton(i, driver);
            craftAction.SetButtonPosition(new Vector2(-750, -500), Vector2.right * 305);

            craftingActionButtons.Add(craftAction);
        }

        craftingButtonContainer.SetActive(false);
    }

    public override void CloseMenu()
    {
        craftingButtonContainer.SetActive(false);
        BroadcastUiEvent(false);
    }

    public override void LoadMenu()
    {
        craftingButtonContainer.SetActive(true);
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

public interface IUiFunctionality<T> where T: UiMenu
{
    T MenuFunctionObject { get; set; }
}
