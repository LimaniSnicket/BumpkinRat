using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager uiManager;
    public static bool menuActive { get; private set; }
    public static MenuType activeMenu { get; private set; }

    private void OnEnable()
    {
        if(uiManager == null) { uiManager = this; } else { Destroy(this); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) { menuActive = !menuActive; }
    }
}

[Serializable]
public class UiMenu
{
    public virtual MenuType menuType => MenuType.Default;
    protected GameObject gameObject;
    public UiMenu() { }
    public UiMenu(GameObject g)
    {
        gameObject = g;
    }

    public virtual void LoadMenu()
    {

    }
}

public class CraftingMenu : UiMenu
{
    public override MenuType menuType => MenuType.Crafting;
    public CraftingMenu(GameObject g)
    {
        gameObject = g;
    }

    public override void LoadMenu()
    {
        base.LoadMenu();
    }
}
