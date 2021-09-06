using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusAreaUI : MonoBehaviour, IFocusArea, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
{
    ItemObjectUiElement parent;
    public FocusArea FocusArea { get; set; } = new FocusArea();

    private bool hovering;


    void Update()
    {
        if (parent == null)
        {
            try
            {
                parent = GetComponentInParent<ItemObjectUiElement>();
            }
            catch (NullReferenceException){ }
        }
    }

    public void SetDetails(FocusAreaUiDetails details)
    {
        GetComponent<RectTransform>().localPosition = details.PositionOnUi;
        FocusArea.focusAreaId = details.id;
        SetFocusAreaString(details.name.ToDisplay());
    }

    private void SetFocusAreaString(string message)
    {
        FocusArea.displayText = GetComponentInChildren < TextMeshProUGUI >();
        FocusArea.SetTextMesh(message);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovering = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hovering = true;
        if (ItemCrafter.CraftingSequenceActive)
        {
            parent.BroadcastInteractedWith(this);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ItemCrafter.CraftingSequenceActive)
        {
            Debug.Log("To-do: Finish crafting sequence if active sequence");
            ItemCrafter.EndCraftingSequence();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ItemCrafter.BeginCraftingSequence(this, parent);
    }
}
