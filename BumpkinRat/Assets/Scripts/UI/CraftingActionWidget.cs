using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections;
using DG.Tweening;

public class CraftingActionWidget: MonoBehaviour, IPointerEnterHandler
{
    private Image image;

    private CraftingAction craftingAction;

    private CraftingManager craftingMenuBehaviour;

    private static CraftingActionWidgetActive craftingActionButtonActivated;

    private Color activeColor, inactiveColor;

    private Vector2 originalPosition;

    private RectTransform rect;

    private TextMeshProUGUI widgetText;

    private const float InactiveScaleFactor = 1;

    private const float ActiveScaleFactor = 1.3f;

    private void Awake()
    {
        if(craftingActionButtonActivated == null)
        {
            craftingActionButtonActivated = new CraftingActionWidgetActive();
        }

        craftingActionButtonActivated.AddListener(OnWidgetActivated);

        image = gameObject.GetOrAddComponent<Image>();

        rect = image.GetComponent<RectTransform>();

        widgetText = GetComponentInChildren<TextMeshProUGUI>();

        originalPosition = rect.localPosition;

        inactiveColor = image.color;
        activeColor = Color.white;

        StartCoroutine(MoveToNewLocationWithinUnitSphere(5));
    }

    public static void ResetAll()
    {
        craftingActionButtonActivated.Invoke(CraftingAction.NONE);
    }

    public void SetCraftingActionButton(int craftAction, CraftingManager crafter)
    {
        craftingAction = (CraftingAction)craftAction;

        craftingMenuBehaviour = crafter;

        widgetText.text = craftingAction.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!ItemCrafter.CraftingSequenceActive)
        {
            return;
        }

        this.TakeWidgetSpecifiedCraftingAction();
    }

    private void TakeWidgetSpecifiedCraftingAction()
    {
        craftingMenuBehaviour.SubmitCraftingActionToItemCrafter(craftingAction);
        craftingActionButtonActivated.Invoke(craftingAction);
    }

    private void OnWidgetActivated(CraftingAction action)
    {
        bool thisButtonActive = action == craftingAction;

        image.color = thisButtonActive ? activeColor : inactiveColor; 

        float scaleModifier = thisButtonActive ? InactiveScaleFactor : ActiveScaleFactor;
        transform.localScale = Vector3.one * scaleModifier;
    }

    private IEnumerator MoveToNewLocationWithinUnitSphere(float duration)
    {
        Vector2 randoSpot = originalPosition + UnityEngine.Random.insideUnitCircle * CraftingManager.DistractionJitter * 100;

        float distance = Vector2.Distance(randoSpot.normalized, rect.localPosition.normalized);


        float timingOffset = UnityEngine.Random.Range(0, 2);

        rect.DOLocalMove(randoSpot, (duration * distance) + timingOffset);
        yield return new WaitForSeconds((duration * distance) + timingOffset);
        yield return StartCoroutine(MoveToNewLocationWithinUnitSphere(duration));
    }

    private void OnDestroy()
    {
        try
        {
            craftingActionButtonActivated.RemoveListener(OnWidgetActivated);

        } catch (NullReferenceException)
        {

        }

    }
    class CraftingActionWidgetActive : UnityEvent<CraftingAction> { }
}
