using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class ConversationSnippet : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Sprite leftBubble, rightBubble, centerBubble;

    private static int childIndex = 2;

    private readonly static float focusedScaleFactor = 0.6f;

    private static EventHandler DestroySnippet;

    private static EventHandler<ConversationSnippetEventArgs> DestroySpecifiedSnippets;

    private ConversationUi conversationUI;

    private BubbleDisplay bubbleElements;

    private ConversationBubbleOrientation currentOrientation;

    private StringBuilder builder;

    private bool isResponse;

    private bool newSnip = true;

    private Vector2 originalPosition;

    private float dragWeight = 10;
    public bool Typing { get; private set; } = false;

    public bool IsBeingDragged { get; private set; }

    public int ChildIndex => childIndex;

    private void OnEnable()
    {
        ConversationBubbleOrientationManager.InitializeSprites(leftBubble, centerBubble, rightBubble);

        bubbleElements = new BubbleDisplay(this.gameObject, isResponse);

        builder = new StringBuilder();

        currentOrientation = ConversationBubbleOrientation.Center;
    }

    private void Start()
    {
        DestroySnippet += OnDestroySnippet;
        DestroySpecifiedSnippets += OnDestroySpecifiedSnippets;
    }

    private void Update()
    {
        bubbleElements.SetDisplayString(builder.ToString());
    }

    public Sequence GetInactivitySequence()
    {
        return bubbleElements.CreateNewInactivitySequence();
    }

    public void SetConversationUi(ConversationUi convoUi)
    {
        conversationUI = convoUi;
        SubscribeToConversationUiEvents();
    }

    public void SetIsResponse(bool isResponse)
    {
        this.isResponse = isResponse;
    }

    public void InitializeSnippetFromBubbleDisplay(IBubbleDisplay bubble)
    {
        builder.Append(bubble.DisplayMessage);
        bubbleElements.Copy(bubble.BubbleElements);
    }

    public void SetSnippetInactive()
    {
        bubbleElements.SetToInactiveState(0.45f, 0.5f);
    }

    public void MoveToFocusedPosition(Vector2 position, float time)
    {
        ShrinkToFocusedScale();
        bubbleElements.MoveRectTransform(position, time);
        MoveToFocusedPosition(time);
    }

    public void MoveToFocusedPosition(float time)
    {
        StartCoroutine(OrientSnippetToFocusedPosition(time));
    }

    public void InitializeSnippetTransform(Vector2 pos, bool focused)
    {
        float scale = 1;

        if (focused)
        {
            currentOrientation = ConversationBubbleOrientation.Right;
            scale = focusedScaleFactor;
        }
        else
        {
            bool useLeftSprite = pos.x >= 0;
            currentOrientation = useLeftSprite ? ConversationBubbleOrientation.Left : ConversationBubbleOrientation.Right;
        }

        SetPositionAndScale(pos, scale);

        bubbleElements.SetFromOrientation(currentOrientation);
    }

    public void ApplyLocalJumpAndJitter(Vector3 jump)
    {
        StartCoroutine(bubbleElements.DoJiggleJump(jump));
    }

    public void ApplyConversationAesthetic(ConversationAesthetic aesthetic)
    {
        bubbleElements.ApplyConversationAesthetic(aesthetic, isResponse, false);
    }

    public void ReadLine(string line, float delay)
    {
        StartCoroutine(TypeOut(line, delay));
    }

    private void SubscribeToConversationUiEvents()
    {
        if (conversationUI != null)
        {
            conversationUI.SpawningNewConversationSnippet += OnSnippetSpawnMoveConversationBubble;
        }
    }

    private void UnSubscribeToConversationUiEvents()
    {
        if (conversationUI != null)
        {
            conversationUI.SpawningNewConversationSnippet -= OnSnippetSpawnMoveConversationBubble;
        }
    }

    private IEnumerator OrientSnippetToFocusedPosition(float time)
    {
        var orientations = isResponse
            ? ConversationBubbleOrientationManager.MoveToRight(currentOrientation.BubbleOrientation) 
            : ConversationBubbleOrientationManager.MoveToLeft(currentOrientation.BubbleOrientation);

        ConversationBubbleOrientation newOrientation = currentOrientation;

        float timeInterval = time / (orientations.Length + 1);

        foreach (var orientation in orientations)
        {
            yield return new WaitForSeconds(timeInterval);

            newOrientation = orientation;

            bubbleElements.SetFromOrientation(newOrientation);
        }

        currentOrientation = newOrientation;        
    }

    private void SetPositionAndScale(Vector2 pos, float scale)
    {
        bubbleElements.SetLocalBubblePosition(pos, isResponse);

        originalPosition = bubbleElements.LocalPosition;

        bubbleElements.ScaleRectTransform(Vector3.one * scale, 0.7f);
    }

    public void UpdateChildIndex()
    {
        transform.SetSiblingIndex(childIndex);
        childIndex++;
    }

    public void SetDragWeight(float distraction)
    {
        double weight = 2 * (Math.Pow(8, distraction));
        dragWeight = (float)weight;
    }

    public void ShrinkToFocusedScale(float toScale = 0.6f, float time = 0.3f)
    {
        bubbleElements.ScaleRectTransform(Vector3.one * toScale, time);
    }

    public void GrowToFullScale(float time)
    {
        bubbleElements.ScaleRectTransform(Vector3.one, time);
    }

    private void OnSnippetSpawnMoveConversationBubble(object source, ConversationSnippetEventArgs args)
    {
        if (!newSnip)
        {
            int factor = isResponse ? -1 : 1;

            Vector3 jump = bubbleElements.LocalPosition + (Vector3.up * 200) - 100 * factor * Vector3.right;
            Vector3 rotation = 25f * -factor * Vector3.forward;

            StartCoroutine(ShrinkAndDestroy(jump, rotation, isResponse));
        }

        newSnip = false;
    }

    private IEnumerator TypeOut(string line, float delay)
    {
        Typing = true;
        yield return this.ReadLine(line, builder, delay);
        Typing = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isResponse && eventData.hovered.Contains(gameObject))
        {
            IsBeingDragged = true;
            transform.DOMove(Input.mousePosition, Time.fixedDeltaTime * dragWeight);
        } else
        {
            IsBeingDragged = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
    }

    public static void DestroyAllSnippets(object source)
    {
        DestroySnippet.BroadcastEvent(source);
    }

    public static void DestroyAllCustomerResponseSnippets(object source)
    {
        ConversationSnippetEventArgs args = new ConversationSnippetEventArgs
        {
            DestroyCustomerSnippets = true,
            DestroyResponseSnippets = false
        };

        DestroySpecifiedSnippets.BroadcastEvent(source, args);
    }

    void OnDestroySnippet(object source, EventArgs args)
    {
        StartCoroutine(ShrinkAndDestroy());
    }

    void OnDestroySpecifiedSnippets(object source, ConversationSnippetEventArgs args)
    {
        bool checkDestroy = isResponse ? args.DestroyResponseSnippets : args.DestroyCustomerSnippets;
        if (checkDestroy)
        {
            IEnumerator destroyRoutine = ShrinkAndDestroy(); 
            StartCoroutine(destroyRoutine);
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        Vector3 jump = bubbleElements.LocalPosition * 1.6f;
        Vector3 rotate = Vector3.forward * -30;

        yield return StartCoroutine(ShrinkAndDestroy(jump, rotate));
    }

    private IEnumerator ShrinkAndDestroy(Vector3 localJump, Vector3 localRotate, bool response = false)
    {
        yield return StartCoroutine(bubbleElements.ShrinkAndDestroy(localJump, localRotate, response));
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnSubscribeToConversationUiEvents();
        DestroySnippet -= OnDestroySnippet;
        DestroySpecifiedSnippets -= OnDestroySpecifiedSnippets;
        childIndex--;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsBeingDragged = false;
    }
}

public struct FocusedPortrait
{
    public Image backing, portrait;

    public bool Active { get; private set; }

    public RectTransform RectTransform { get; private set; }
    public FocusedPortrait(GameObject obj)
    {
        backing = obj.GetOrAddComponent<Image>();
        portrait = backing.GetComponentInChildren<Image>();
        RectTransform = backing.GetComponent<RectTransform>();
        Active = backing.gameObject.activeSelf;
    }

    public void SetPortrait(Sprite sprite)
    {
        portrait.sprite = sprite;
    }

    public void SetActive(bool active)
    {
        backing.gameObject.SetActive(active);
        Active = backing.gameObject.activeSelf;
    }
}

public class ConversationSnippetEventArgs : EventArgs
{
    public bool SpawningResponse { get; set; }

    public bool DestroyCustomerSnippets { get; set; }

    public bool DestroyResponseSnippets { get; set; }
}
