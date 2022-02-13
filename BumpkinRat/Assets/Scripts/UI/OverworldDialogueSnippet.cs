using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Text;

public class OverworldDialogueSnippet : DraggableFoldout<RectTransform>, IDragHandler
{
    public Image displayImage;
    float dragSpeed;

    public bool Moving { get; private set; }


    void OnEnable()
    {
        textDisplay = displayImage.GetComponentInChildren<TextMeshProUGUI>();
        draggerTransform = GetComponent<RectTransform>();
        foldoutTransform = displayImage.GetComponent<RectTransform>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.V)) && FoldoutStatus.Equals(FoldoutStatus.FOLDED_OUT))
        {
            StartCoroutine(CollapseBack());
        }
    }


    public void MoveTo(Vector2 finalPosition, float duration)
    {
        StartCoroutine(MoveSnippet(finalPosition, duration));
    }


    public void OnDrag(PointerEventData eventData)
    {
        if(CanDragOut() && ValidMouseDirectionDrag(pullDirection, out pullDirectionalVector))
        {
            dragSpeed = 0.5f;
            StartCoroutine(DragOut());
        }
    }

    public override IEnumerator DragOut()
    {
        Vector2 x = originalPosition + pullDirectionalVector * 600; ;
        yield return StartCoroutine(MovingOut(x, Vector2.one, dragSpeed));
        Interactable = false;
    }

    public override IEnumerator CollapseBack()
    {
        Vector2 x = originalPosition;
        yield return StartCoroutine(MovingOut(x, Vector2.zero, 0.5f));
        OverworldDialogueUI.UpdateSnippetText();
        Interactable = true;
    }

    IEnumerator MoveSnippet(Vector2 position, float duration)
    {
        Moving = true;
        draggerTransform.DOLocalMove(position, duration);
        yield return new WaitForSeconds(duration);
        Moving = false;
    }

}

public enum FoldoutStatus
{
    FOLDED_IN = 0,
    FOLDING = 1,
    FOLDED_OUT = 2
}

