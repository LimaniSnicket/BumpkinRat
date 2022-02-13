using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    public void OnClickToggleButtonEnable(Button btn)
    {
        bool toggleTo = !btn.interactable;
        btn.interactable = toggleTo;
    }

    private void OnUiEvent(object source, UiEventArgs args)
    {
        ActiveMenus.HandleInstanceObjectInList(args.MenuTypeLoaded, args.Load);
    }

    private void OnDisable()
    {
        UiMenu.UiEvent -= OnUiEvent;   
    }
}

[Serializable]
public class InventoryMenu : UiMenu
{
    public override KeyCode ActivateKeyCode => KeyCode.Y;

    public InventoryMenu(GameObject g)
    {
        gameObject = g;
        MenuType = MenuType.Inventory;
        gameObject.SetActive(false);
    }

    public override void CloseMenu()
    {
        this.SendClosingMenuEvent();
        gameObject.SetActive(false);
    }

    public override void LoadMenu()
    {
        this.SendLoadingMenuEvent();
        gameObject.SetActive(true);

    }
}

[Serializable]
public class DialogueMenu : UiMenu
{
    private readonly TextMeshProUGUI dialogueDisplay;
    bool acceptInputFromRunner;

    public override KeyCode ActivateKeyCode => KeyCode.None;

    public DialogueMenu(GameObject g)
    {
        gameObject = g;
        MenuType = MenuType.Dialogue;
        dialogueDisplay = g.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        dialogueDisplay.text = "";
    }

    public override void LoadMenu()
    {
        this.SendLoadingMenuEvent();
        dialogueDisplay.text = "";
        acceptInputFromRunner = true;
        Debug.Log("Dialogue Menu ready for input");
    }

    public override void CloseMenu()
    {
        this.SendClosingMenuEvent();
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
    public string UiMenuName { get; set; }
    public bool Load { get; set; }
    public MenuType MenuTypeLoaded { get; set; }
}

public interface IUiFunctionality<T> where T: UiMenu
{
    T UiMenu { get; }
}
