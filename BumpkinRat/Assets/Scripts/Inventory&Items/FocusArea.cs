using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FocusArea : MonoBehaviour
{
    private ItemObject parentItemObject;
    SphereCollider sphereCollider;
    SpriteRenderer spriteRenderer;
    TextMeshPro numberDisplay;

    public int focusAreaId;

    bool Selected { get; set; }
    internal bool IsFocus { get; set; }

    Vector3 originalScale;

    private void OnEnable()
    {
        AssignParentItemObject();
        sphereCollider = GetComponent<SphereCollider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        numberDisplay = GetComponentInChildren<TextMeshPro>();
        originalScale = transform.localScale;

        SetNumberDisplay();
    }
    void AssignParentItemObject()
    {
        try
        {
            parentItemObject = GetComponentInParent<ItemObject>();
            parentItemObject.RegisterFocusArea(this);
        }
        catch (NullReferenceException)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        spriteRenderer.color = parentItemObject.NumberOfInFocusAreas() > 0 ? Color.cyan : Color.gray;
        transform.forward = Camera.main.transform.forward * -1;
        if(Selected && !ItemCrafter.CraftingSequenceActive) { Selected = false; }
    }


    Coroutine exitProcess;

    private void OnMouseOver()
    {
        if (!Selected)
        {
            transform.localScale = originalScale.PulseVector3(1.2f, 0.1f, 2, 3f);

        } else
        {
            transform.localScale = originalScale;
            return;
        }

        if (!IsFocus && parentItemObject.NumberOfInFocusAreas() == 0)
        {
            IsFocus = true;
            if (ItemCrafter.CraftingSequenceActive)
            {
                parentItemObject.BroadcastInteractionWithFocusArea(this);
            }
        }

        if(exitProcess != null)
        {
            StopCoroutine(exitProcess);
        }

    }

    private void OnMouseDown()
    { 
        ItemCrafter.BeginCraftingSequence();
        parentItemObject.BroadcastInteractionWithFocusArea(this);
        Selected = true;
    }

    private void OnMouseExit()
    {
        exitProcess = StartCoroutine(StartExitProcess());
    }

    IEnumerator StartExitProcess()
    {
        yield return new WaitForSeconds(0.2f);
        IsFocus = false;
        transform.localScale = originalScale;
    }

    void SetNumberDisplay()
    {
        try
        {
            numberDisplay.text = focusAreaId.ToString();
        }
        catch (NullReferenceException)
        {

        }
    }

    public override string ToString()
    {
        return $"Id:{parentItemObject.itemId} FA:{focusAreaId}";
    }
}
