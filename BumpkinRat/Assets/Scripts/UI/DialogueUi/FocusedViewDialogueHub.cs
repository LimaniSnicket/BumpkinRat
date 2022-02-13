using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class FocusedViewDialogueHub : MonoBehaviour, IDropHandler
{
    private FocusedPortrait focusedPortrait;

    private RectTransform dragResponseToRect;

    private const string FocusedPortraitName = "FocusedPortrait";
    private const string DragToAreaName = "DragContainer";

    public Vector2 focusedConversationSnippetSpawnPoint;

    private Vector2 viewPosition;

    private readonly Vector2 offsetPosBy = new Vector2(600, 0);

    private ConversationSnippet mostRecentPlayerResponse;

    private RectTransform responseRectTransform;

    private RectTransform rectTransform;

    [SerializeField]
    private bool responseOverlappingView;

    public bool ResponseOverlapsFocusedView
    {
        get => responseOverlappingView;
        private set
        {
            if (value != responseOverlappingView)
            {
                Debug.Log("Change in overlap status. New Value: " + value);
            }
            responseOverlappingView = value;
        }
    }
    public bool FocusedHubActive { get; private set; }

    private void Start()
    {
        this.InitializeFocusedPortrait();
        this.SetDragContainerFromChildren();
       this. SetFocusedHubActive(false);

        rectTransform = GetComponent<RectTransform>();
        viewPosition = rectTransform.localPosition;

        rectTransform.localPosition = viewPosition - offsetPosBy;
    }
    private void Update()
    {
        if(responseRectTransform != null)
        {
            ResponseOverlapsFocusedView = IsOverlappingFocusedHub(responseRectTransform);
        }
    }

    private void InitializeFocusedPortrait()
    {
        GameObject portraitObj = transform.Find(FocusedPortraitName).gameObject;
        focusedPortrait = new FocusedPortrait(portraitObj);
    }

    private void SetDragContainerFromChildren()
    {
        dragResponseToRect = transform.Find(DragToAreaName).GetComponent<RectTransform>();
    }

    public void SetFocusedHubConversationActiveWithSnippet(ConversationSnippet focusedSnippet)
    {
        StartCoroutine(MoveFocusedDialogueHubIntoView());
        focusedSnippet.MoveToFocusedPosition(focusedConversationSnippetSpawnPoint, 1f);
    }

    public bool MostRecentResponseIsOverlappingDragToRect()
    {
        if (mostRecentPlayerResponse == null)
        {
            return false;
        }

        return this.IsOverlappingDragResponseTo(mostRecentPlayerResponse.GetComponent<RectTransform>());
    }

    public IEnumerator ChangeAppearanceWhenOverlapping()
    {
        yield return new WaitUntil(() => MostRecentResponseIsOverlappingDragToRect());
        mostRecentPlayerResponse.MoveToFocusedPosition(0.5f);
    }

    private IEnumerator MoveFocusedDialogueHubIntoView()
    {
        SetFocusedHubActive(true);

        rectTransform.DOLocalMove(viewPosition, 0.75f);

        yield return new WaitForSeconds(0.4f);

        rectTransform.DOScaleX(1.5f, 0.35f);

        yield return new WaitForSeconds(0.35f);

        rectTransform.DOScaleX(1, 0.1f);
    }

    public void SetMostRecentPlayerResponse(ConversationSnippet responseSnippet)
    {
        mostRecentPlayerResponse = responseSnippet;
        responseRectTransform = responseSnippet == null ? null : mostRecentPlayerResponse.GetComponent<RectTransform>();
    }

    public void ShrinkMostReceptPlayerResponse()
    {
        if(mostRecentPlayerResponse != null)
        {
            mostRecentPlayerResponse.ShrinkToFocusedScale();
        }
    }

    private void SetFocusedHubActive(bool setActive)
    {
        focusedPortrait.SetActive(setActive);
        dragResponseToRect.gameObject.SetActive(setActive);
        FocusedHubActive = setActive;
    }

    private bool IsOverlappingFocusedHub(RectTransform draggingRect)
    {
        return rectTransform.OverlappingLocalSpace(draggingRect);
    }

    private bool IsOverlappingDragResponseTo(RectTransform draggingRect)
    {
        return dragResponseToRect.OverlappingLocalSpace(draggingRect);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.Equals(mostRecentPlayerResponse.gameObject))
        {
            mostRecentPlayerResponse.transform.SetParent(transform);
        }
    }
}

public class ResponseTimeTracker
{
    private ConversationSnippet lastActiveSnippet;

    private float timeTracker;

    private readonly float inactivityThreshold;

    private bool isInactive;

    public bool IsInactive => isInactive;

    private Coroutine runningInactivityCoroutine;

    public ResponseTimeTracker(float inactivityThreshold)
    {
        this.inactivityThreshold = inactivityThreshold;
    }
    public void StartTrackingSnippet(ConversationSnippet snippetToTrack)
    {
        lastActiveSnippet = snippetToTrack;
        timeTracker = 0;
    }

    public void StopTracking()
    {
        if (runningInactivityCoroutine != null)
        {
            lastActiveSnippet.StopCoroutine(runningInactivityCoroutine);
            runningInactivityCoroutine = null;
        }

        lastActiveSnippet = null;
        isInactive = false;
        timeTracker = 0;
    }

    public void Tick(float timescale)
    {
        if(lastActiveSnippet != null)
        {
            timeTracker += timescale;

            if (!isInactive && timeTracker > inactivityThreshold)
            {
                isInactive = true;
                if (runningInactivityCoroutine == null)
                {
                    runningInactivityCoroutine = lastActiveSnippet.StartCoroutine(InactiveBehavior());
                }
            }
        }
    }

    private IEnumerator InactiveBehavior()
    {
        yield return new WaitUntil(() => isInactive);

        lastActiveSnippet.SetSnippetInactive();

        yield return new WaitForSeconds(3);

        while (isInactive)
        {
            Sequence seq = lastActiveSnippet.GetInactivitySequence();

            seq.Play();

            yield return seq.WaitForCompletion();

            yield return new WaitForSeconds(2f + Random.Range(0, 10));
        }
    }
}
