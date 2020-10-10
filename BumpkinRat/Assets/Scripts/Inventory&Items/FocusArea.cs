using System;
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
        if (IsFocus)
        {

            
        }
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

    private void OnMouseOver()
    {
        if (!IsFocus)
        {
            IsFocus = true;
            if (ItemCrafter.CraftingSequenceActive)
            {
                parentItemObject.BroadcastInteractionWithFocusArea(this);

            }
        }
    }

    private void OnMouseDown()
    { 
        ItemCrafter.BeginCraftingSequence();
        parentItemObject.BroadcastInteractionWithFocusArea(this);  
    }

    private void OnMouseExit()
    {
        IsFocus = false;
    }

    public override string ToString()
    {
        return $"{parentItemObject.name} Id:{parentItemObject.itemId} FA:{focusAreaId}";
    }
}
