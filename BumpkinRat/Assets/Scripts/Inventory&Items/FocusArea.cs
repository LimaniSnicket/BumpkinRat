using System;
using System.Collections;
using UnityEngine;

public class FocusArea : MonoBehaviour
{
    private ItemObject parentItemObject;
    SphereCollider sphereCollider;
    SpriteRenderer spriteRenderer;

    public int focusAreaId;

    internal bool IsFocus { get; set; }

    private void OnEnable()
    {
        AssignParentItemObject();
        sphereCollider = GetComponent<SphereCollider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        spriteRenderer.color = parentItemObject.NumberOfInFocusAreas() > 0 ? Color.blue : Color.gray;
    }

    void OnItemObjectFocusChange(bool inFocus)
    {
        sphereCollider.enabled = true;
        spriteRenderer.color = inFocus ? Color.blue : Color.gray;
        transform.forward = Camera.main.transform.forward * -1;
    }

    void SetFocusAreaSprite()
    {
        spriteRenderer.color = Color.gray;
        transform.forward = Camera.main.transform.forward * -1;
    }


    Coroutine exitProcess;

    private void OnMouseOver()
    {
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

        transform.forward = Camera.main.transform.forward * -1;

        transform.localScale = Vector3.one + MathfX.PulseVector3(0.3f, 0.3f, 1, 2f);
    }

    private void OnMouseDown()
    { 
        ItemCrafter.BeginCraftingSequence();
        parentItemObject.BroadcastInteractionWithFocusArea(this);  
    }

    private void OnMouseExit()
    {
        exitProcess = StartCoroutine(StartExitProcess());
    }

    IEnumerator StartExitProcess()
    {
        yield return new WaitForSeconds(0.2f);
        IsFocus = false;
        transform.localScale = Vector3.one;
    }

    public override string ToString()
    {
        return $"{parentItemObject.name} Id:{parentItemObject.itemId} FA:{focusAreaId}";
    }
}
