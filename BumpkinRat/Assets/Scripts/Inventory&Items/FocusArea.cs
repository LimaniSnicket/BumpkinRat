using System;
using UnityEngine;

public class FocusArea : MonoBehaviour
{
    private ItemObject parentItemObject;
    SphereCollider sphereCollider;
    SpriteRenderer spriteRenderer;

    public int focusAreaId;

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

    void OnItemObjectFocusChange(bool inFocus)
    {
        Debug.Log($"Parent Item Object {(inFocus ? "is": "isn't")} Focus");
        //sphereCollider.enabled = inFocus;
        spriteRenderer.color = inFocus ? Color.blue : Color.gray;
        transform.forward = Camera.main.transform.forward * -1;
    }

    void SetFocusAreaSprite()
    {
        spriteRenderer.color = Color.gray;
        transform.forward = Camera.main.transform.forward * -1;
    }
}
