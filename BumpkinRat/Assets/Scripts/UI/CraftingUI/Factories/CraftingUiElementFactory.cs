using System;
using UnityEngine;
public class CraftingUiElementFactory 
{
    private readonly CraftingManager craftingManager;

    private readonly ToolkitMenu toolKitMenu;

    public CraftingUiElementFactory(CraftingManager craftManager, ToolkitMenu toolkit)
    {
        this.craftingManager = craftManager;
        this.toolKitMenu = toolkit;
    }

    public CraftingMenu CreateCraftingMenu()
    {
        return new CraftingMenu(craftingManager);       
    }

    public FocusAreaIndicator CreateFocusAreaIndicator(GameObject instantiated, FocusAreaObject focusArea)
    {
        FocusAreaIndicator indicator = new FocusAreaIndicator(instantiated);
        indicator.gameObject.transform.SetParent(craftingManager.transform);

        Vector2 worldToScreenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, focusArea.transform.position);
        indicator.SetPosition(worldToScreenPoint);

        indicator.SetTMPMessage(focusArea.ToString());

        return indicator;
    }

    public void CreateDefaultCraftingActionWidget(GameObject prefab, UiElementContainer container, bool setContainerInactive = true)
    {
        this.CreateCraftingActionWidgetInternal(container, prefab, CraftingAction.ATTACH);
        
        if (setContainerInactive)
        {
            container.gameObject.SetActive(false);
        }
    }

    public void CreateCraftingActionWidgets(GameObject prefab, UiElementContainer container)
    {
        CraftingAction[] actions = (CraftingAction[])Enum.GetValues(typeof(CraftingAction));
        this.CreateCraftingActionWidgetsInternal(container, prefab, 1, actions);
        container.gameObject.SetActive(false);
    }

    public void CreateCraftingActionWidgetsForToolkit(GameObject prefab, UiElementContainer container, bool withDefaultAction = false, bool clearAllChildren = false)
    {
        var actions = this.toolKitMenu.ActiveToolCraftingActions;

        if (clearAllChildren)
        {
            container.DetachAndClearChildren();
        }

        if (withDefaultAction)
        {
            this.CreateDefaultCraftingActionWidget(prefab, container, setContainerInactive: false);
        }

        this.CreateCraftingActionWidgetsInternal(container, prefab, 0, actions);
    }

    private void CreateCraftingActionWidgetsInternal(UiElementContainer container, GameObject prefab, int start = 0, params CraftingAction[] actions)
    {
        for (int i = start; i < actions.Length; i++)
        {
            this.CreateCraftingActionWidgetInternal(container, prefab, actions[i]);
        }
    }

    private void CreateCraftingActionWidgetInternal(UiElementContainer container, GameObject prefab, CraftingAction action, int? position = null)
    {
        Debug.LogFormat("Creating Crafting action button {0}", action);
        container.SpawnAtAlternatingVerticalPositions(prefab, 100, 150, position: position);
        CraftingActionWidget craftAction = container.GetLastChildComponent<CraftingActionWidget>();

        craftAction.InitializeWidgetComponents();

        craftAction.SetCraftingActionButton(action, craftingManager);
        craftAction.EnableFidgeting();
    }
}  
