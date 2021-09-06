using System;
using UnityEngine;
public class CraftingUiElementFactory 
{
    private readonly CraftingManager craftingManager;

    public CraftingUiElementFactory(CraftingManager craftManager)
    {
        craftingManager = craftManager;
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

    public void CreateCraftingActionWidgets(GameObject prefab, UiElementContainer container)
    {
        for (int i = 1; i < Enum.GetValues(typeof(CraftingAction)).Length; i++)
        {
            container.SpawnAtAlternatingVerticalPositions(prefab, 100, 150);
            CraftingActionWidget craftAction = container.GetLastChild().GetComponent<CraftingActionWidget>();
            craftAction.SetCraftingActionButton(i, craftingManager);
            craftAction.EnableFidgeting();
        }

        container.gameObject.SetActive(false);
    }
}
