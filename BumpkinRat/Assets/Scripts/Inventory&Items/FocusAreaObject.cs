using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FocusAreaObject : MonoBehaviour, IFocusArea
{
    private ItemObjectWorldElement parentFocusAreaContainer;

    SphereCollider sphereCollider;
    SpriteRenderer spriteRenderer;
    TextMeshPro numberDisplay;

    public int focusAreaId;

    private bool selected;
    private bool isFocus;
    public FocusArea FocusArea { get; set; }

    Vector3 originalScale;

    private void OnEnable()
    {
        parentFocusAreaContainer = GetComponentInParent<ItemObjectWorldElement>();
        sphereCollider = GetComponent<SphereCollider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        numberDisplay = GetComponentInChildren<TextMeshPro>();
        originalScale = transform.localScale;

        SetNumberDisplay();

        CraftingManager.AddFocusAreaScreenIndicator(this);
    }

    private void Update()
    {
        spriteRenderer.color = parentFocusAreaContainer.FocusAreaHandler.NumberOfInFocusAreas() > 0 ? Color.cyan : Color.gray;
        transform.forward = Camera.main.transform.forward * -1;
        if(selected && !ItemCrafter.CraftingSequenceActive) { selected = false; }
    }


    Coroutine exitProcess;

    private void OnMouseOver()
    {
        if (!selected)
        {
            CraftingPointer.OnFocusAreaHover(this);
            //transform.localScale = originalScale.PulseVector3(1.2f, 0.1f, 2, 3f);

        } else
        {
            transform.localScale = originalScale;
            return;
        }

        if (!isFocus && parentFocusAreaContainer.FocusAreaHandler.NumberOfInFocusAreas() == 0)
        {
            isFocus = true;
            if (ItemCrafter.CraftingSequenceActive)
            {
                parentFocusAreaContainer.BroadcastInteractionWithFocusArea(this);
            }
        }

        if(exitProcess != null)
        {
            StopCoroutine(exitProcess);
        }

    }

    private void OnMouseDown()
    { 
        // ItemCrafter.BeginCraftingSequence();
        parentFocusAreaContainer.BroadcastInteractionWithFocusArea(this);
        //ItemCrafter.BeginCraftingSequence(this.FocusArea, this);
        selected = true;
    }

    private void OnMouseExit()
    {
        exitProcess = StartCoroutine(StartExitProcess());
        CraftingPointer.OnHoverEnd();
    }

    IEnumerator StartExitProcess()
    {
        yield return new WaitForSeconds(0.2f);
        isFocus = false;
        transform.localScale = originalScale;
    }

    void SetNumberDisplay()
    {
        try
        {
            numberDisplay.text = "#" + focusAreaId.ToString();
        }
        catch (NullReferenceException)
        {

        }
    }

    public override string ToString()
    {
        return $"Id:{parentFocusAreaContainer.itemObject.Item.itemId} FA:{focusAreaId}";
    }

    private void OnDestroy()
    {
        CraftingManager.TryRemoveFocusAreaScreenIndicator(this, true);
    }
}

[Serializable]
public class FocusArea
{
    public int focusAreaId;
    public bool Selected { get; private set; }

    public bool IsFocus { get; set; }

    public TMP_Text displayText;

    private IContainFocusArea ParentObject { get; set; }

    public FocusArea() { }

    public void InitializeParentAndId(IContainFocusArea container, int id)
    {
        ParentObject = container;
        focusAreaId = id;
    }

    public void SetTextMesh(string message) 
    {
        displayText.text = message;
    }

    public override string ToString()
    {
        return $"Id:{ParentObject.ItemObjectId} FA:{focusAreaId}";
    }
}

public interface IFocusArea 
{
    FocusArea FocusArea { get; set; }
}