using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleDisplay
{
    private Image backing;
    public TextMeshProUGUI TextMesh { get; set; }

    private RectTransform rectTransform;

    private Color activeColor, inactiveColor;

    public Vector3 LocalPosition => rectTransform.localPosition;

    public Sprite BackingSprite => backing.sprite;

    public RectTransform BubbleRect => rectTransform;

    public RectTransform TextMeshRect => TextMesh.GetComponent<RectTransform>();

    private const string Ellipses = ". . .";

    public BubbleDisplay(GameObject prefab)
    {
        backing = prefab.GetOrAddComponent<Image>();
        TextMesh = backing.GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = backing.GetComponent<RectTransform>();
        activeColor = Color.white;
    }

    public BubbleDisplay(GameObject prefab, bool raycastTarget) : this (prefab)
    {
        backing.raycastTarget = raycastTarget;
    }

    public void Copy(BubbleDisplay bubble)
    {
        SetBackingImage(bubble.BackingSprite);
        rectTransform.localPosition = bubble.rectTransform.localPosition;
        rectTransform.localScale = bubble.rectTransform.localScale;
    }

    public void SetDisplayToEllipses()
    {
        SetDisplayString(Ellipses);
    }

    public void SetDisplayString(string message)
    {
        TextMesh.text = message;
    }

    public void ApplyConversationAesthetic(ConversationAesthetic aesthetic, bool isResponse = true, bool setInactive = true)
    {
        activeColor = aesthetic.GetBubbleColor(isResponse);
        inactiveColor = new Color(activeColor.r, activeColor.g, activeColor.b, 0.5f);

        if (setInactive)
        {
            backing.color = inactiveColor;
        } else
        {
            backing.color = activeColor;
        }

        TextMesh.color = aesthetic.GetTextColor(isResponse);

    }

    public void SetToInactiveState(float inactiveScale, float tweenSpeed)
    {
        this.SetActivityState(false, inactiveScale, tweenSpeed);
    }

    public void SetToActiveState(float activeScale, float tweenSpeed)
    {
        this.SetActivityState(true, activeScale, tweenSpeed);
    }

    private void SetActivityState(bool isActive, float scale, float tweenSpeed)
    {
        Vector3 scaleVect = Vector3.one * scale;

        SwapBackingColors(isActive);
        ScaleRectTransform(scaleVect, tweenSpeed);
    }

    private void SwapBackingColors(bool isActive)
    {
        backing.color = isActive ? activeColor : inactiveColor;
    }

    public void SetFromOrientation(ConversationBubbleOrientation orientation)
    {
        SetBackingImage(orientation.SpriteForOrientation);
        AdjustTmpTransform(orientation.TmpLocalPosition, orientation.TmpEulers);
    }

    public void SetBackingImage(Sprite s)
    {
        backing.sprite = s;
    }

    public void ScaleRectTransform(Vector3 scale, float tweenSpeed)
    {
        rectTransform.DOScale(scale, tweenSpeed);
    }

    public void MoveRectTransform(Vector2 position, float tweenSpeed)
    {
        rectTransform.DOLocalMove(position, tweenSpeed);
    }

    public void SetLocalBubblePosition(Vector2 pos, bool isResponse)
    {
        rectTransform.localPosition = isResponse ? new Vector2(-pos.x, pos.y) : pos;
    }

    public void AdjustTmpTransform(Vector3 pos, Vector3 euler)
    {
        TextMeshRect.localEulerAngles = euler;
        TextMeshRect.localPosition = pos;
    }

    public IEnumerator ShrinkAndDestroy(Vector3 localJump, Vector3 localRotate, bool response = false)
    {
        float scaleFactor = BubbleRect.localScale.magnitude;

        BubbleRect.DOShakeScale(0.4f, 0.4f * scaleFactor, 15);

        if (!response)
        {
            BubbleRect.DOLocalJump(localJump, 0.5f, 1, 1f);
            BubbleRect.DOBlendableLocalRotateBy(localRotate, 0.5f);
            yield return new WaitForSeconds(0.65f);
        }

        ScaleRectTransform(Vector3.zero, 0.5f);
        yield return new WaitForSeconds(0.53f);
    }

    public IEnumerator DoJiggleJump(Vector3 jumpTo)
    {
        float scaleFactor = BubbleRect.localScale.magnitude;

        BubbleRect.DOShakeScale(0.4f, 0.4f * scaleFactor, 15);

        BubbleRect.DOLocalJump(jumpTo, 0.5f, 1, 1f);
        yield return new WaitForSeconds(0.65f);
    }

    public Sequence CreateNewInactivitySequence()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Append(rectTransform.DORotate(Vector3.forward * -20, 0.5f));
        sequence.Append(rectTransform.DORotate(Vector3.forward * 50, 0.3f));
        sequence.Append(rectTransform.DORotate(Vector3.zero, 0.2f));

        return sequence;
    }
}
