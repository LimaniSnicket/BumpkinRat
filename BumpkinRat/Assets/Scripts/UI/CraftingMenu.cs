using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[Serializable]
public class CraftingMenu : UiMenu
{
    private readonly TextMeshProUGUI craftingSequenceDisplay;
    public override KeyCode ActivateKeyCode => KeyCode.None;
    public UiElementContainer CraftingButtonContainer { get; private set; }

    public OccupiablePositionContainer occupiablePositionContainer;

    private CraftingMenu(GameObject g)
    {
        gameObject = g;
        MenuType = MenuType.Crafting;
    }

    public CraftingMenu(CraftingManager craftingManager): this(craftingManager.gameObject)
    {
        craftingSequenceDisplay = craftingManager.progressDisplay.GetOrAddComponent<TextMeshProUGUI>();

        occupiablePositionContainer = new OccupiablePositionContainer(craftingManager.itemObjectPositions);

        CraftingButtonContainer = craftingManager.craftingActionButtonParent.GetOrAddComponent<UiElementContainer>();
    }

    public void UpdateDisplayWithSequenceProgress()
    {
        UpdateDisplay(ItemCrafter.ActiveCraftingSequenceProgressString);
    }

    public override void CloseMenu()
    {
        Cursor.visible = true;
        InventoryButton.DisableItemSpawning();
        CraftingButtonContainer.gameObject.SetActive(false);
        this.SendClosingMenuEvent();
    }

    public override void LoadMenu()
    {
        Cursor.visible = false;
        InventoryButton.EnableItemSpawning();
        CraftingButtonContainer.gameObject.SetActive(true);
        this.SendLoadingMenuEvent();
    }

    private void UpdateDisplay(string message)
    {
        if (craftingSequenceDisplay == null)
        {
            return;
        }

        craftingSequenceDisplay.text = message;
    }
}
