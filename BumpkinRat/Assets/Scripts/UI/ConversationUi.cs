using System;
using System.Collections;
using UnityEngine;

public class ConversationUi : MonoBehaviour
{
    public GameObject conversationSnippetPrefab;
    public GameObject uiContainer, responseContainer, focusedPortraitObject;

    public Vector2 conversationSnippetSpawnPoint;
    public Vector2 responseSpawnPoint;
    public float InactivityThreshold { get; set; } = 10f;

    private bool responding;
    private bool responseEnabled;
    private bool conversationUiBusy;

    public static int numberOfSnippets;

    public event EventHandler<ConversationSnippetEventArgs> SpawningNewConversationSnippet;

    public ConversationAesthetic currentConversationAesthetic;

    private ConversationSnippet hangingNpcResponse;

    private CustomerDialogueTracker conversationTracker;

    private ConversationResponseDisplayManager conversationResponseManager;

    private CharacterMenu characterMenu;

    private bool playerResponseDraggedSuccess;

    private DialogueTrackerFactory trackerFactory;
    private ConversationUiElementFactory snippetFactory;
    private FocusedViewDialogueHub focusedViewDialogueHub;
    private ResponseTimeTracker responseTimeTracker;

    private KeyCodeToResponseMap keyMap;

    private void Awake()
    {
        trackerFactory = new DialogueTrackerFactory();
        conversationResponseManager = new ConversationResponseDisplayManager();
        snippetFactory = new ConversationUiElementFactory(this, conversationResponseManager);
        characterMenu = GetComponentInChildren<CharacterMenu>();
        characterMenu.gameObject.SetActive(false);

        keyMap = new KeyCodeToResponseMap(KeyCode.A, KeyCode.S, KeyCode.D);
    }

    private void Start()
    {
        currentConversationAesthetic = ConversationAesthetic.SpookyConversationAesthetic;

        var conversationResponses = snippetFactory.GetResponseDisplays(responseContainer.transform.GetChildren(), currentConversationAesthetic);

        focusedViewDialogueHub = GetComponentInChildren<FocusedViewDialogueHub>();

        responseTimeTracker = new ResponseTimeTracker(InactivityThreshold);
    }

    private void OnEnable()
    {
        uiContainer.SetActive(true);
    }

    private void OnDisable()
    {
        uiContainer.SetActive(false);
    }

    private void Update()
    {
        if (!responseEnabled)
        {
            return;
        }

        responseTimeTracker.Tick(Time.deltaTime);

        foreach(KeyCode key in keyMap.KeyCodes)
        {
            if (Input.GetKeyDown(key))
            {
                TriggerConversationResponse(key);
            }
        }

        if(!playerResponseDraggedSuccess)
        {
            playerResponseDraggedSuccess = focusedViewDialogueHub.ResponseOverlapsFocusedView && Input.GetMouseButtonUp(0); 
        }

    }

    public void InvokeOutroDialogue(float delayTime)
    {
        this.Invoke("OnRecipeCompleteRunOutro", delayTime);
    }

    public void SetActiveConversation(CustomerOrder activeOrder)
    {
        CustomerDialogue active = activeOrder.GetCustomerDialogue();
        conversationTracker = trackerFactory.CreateCustomerDialogueTracker(active);
        this.characterMenu.SetActiveCharacter(activeOrder.CustomerData);
        StartCoroutine(RunCustomerDialogueIntro(active));
    }

    private void OnRecipeCompleteRunOutro()
    {
        ConversationSnippet.DestroyAllSnippets(this);

        var outro = conversationTracker.GetOutroDialogue();

        if (!outro.ValidArray())
        {
            StartCoroutine(RunCustomerDialogueOutro(outro));
        }
    }

    private IEnumerator TakeResponseFromConversation(KeyCode pressed)
    {
        responding = true;

        float distraction = keyMap.CalculateDistractionAmount(pressed);

        ResponseTier tier = keyMap.GetResponseTierForKey(pressed);

        int levelIndex = (int)tier;

        if (conversationResponseManager.IsActive(tier) && !this.conversationTracker.ResponseDialogueComplete)
        {

            DestroyHangingResponse();

            var responseTaken = conversationResponseManager.GetDisplayForResponseTier(tier);
            string message = responseTaken.DisplayMessage;

            if (message != string.Empty)
            {
                yield return StartCoroutine(CreateConversationSnippetForResponseTaken(responseTaken, distraction));
            }

            StartCoroutine(focusedViewDialogueHub.ChangeAppearanceWhenOverlapping());

            while (!playerResponseDraggedSuccess)
            {
                Debug.Log("Waiting for response to get dragged!");

                // run some other routines for animations

                yield return new WaitForEndOfFrame();
            }

            focusedViewDialogueHub.ShrinkMostReceptPlayerResponse();

            BroadcastResponseSnippetSpawn();

            yield return new WaitForSeconds(0.15f);
            
            yield return StartCoroutine(ShowCustomerResponse(levelIndex, distraction));
        }

        responding = false;
    }

    private IEnumerator CreateConversationSnippetForResponseTaken(ConversationResponseDisplay responseTaken, float distraction)
    {
        conversationResponseManager.SetAllInactive();

        ConversationSnippet snippet = snippetFactory.CreatePlayerSnippetFromResponseDisplay(responseTaken);

        yield return new WaitWhile(() => !CraftingManager.FocusedOnCrafting && CraftingManager.CraftingUiBusy);

        yield return new WaitForSeconds(0.05f);

        snippet.GrowToFullScale(0.1f);

        snippet.ApplyLocalJumpAndJitter(Vector3.up);

        snippet.SetDragWeight(distraction);

        focusedViewDialogueHub.SetMostRecentPlayerResponse(snippet);

        yield return new WaitWhile(() => snippet.Typing);

        yield return new WaitForSeconds(0.15f);
    }

    private IEnumerator ShowCustomerResponse(int level, float distraction)
    {
        this.conversationTracker.AdvanceDialogueWithQuality(level);

        CraftingManager.IncreaseDistraction(distraction);

        int quality = this.conversationTracker.Quality;

        yield return new WaitForSeconds(0.15f);

        playerResponseDraggedSuccess = false;

        focusedViewDialogueHub.SetMostRecentPlayerResponse(null);

        yield return new WaitForSeconds(0.25f);

        string[] getLines = this.conversationTracker.GetDisplayLinesAtDialogueIndex(quality);

        yield return StartCoroutine(ReadMultiLineConversationSnippet(getLines));

        yield return new WaitWhile(() => conversationUiBusy);

        yield return new WaitForSeconds(0.15f);

        conversationResponseManager.SetActiveInConversation(this.conversationTracker.GetPlayerDialogueChoices());

        if (this.conversationTracker.ResponseDialogueComplete)
        {
            ConversationSnippet.DestroyAllSnippets(this);
        } 
    }

    private IEnumerator RunCustomerDialogueIntro(CustomerDialogue dialogue)
    {
        var introLines = dialogue.customerIntro.SplitDialogueLines();

        yield return StartCoroutine(ReadMultiLineConversationSnippet(introLines));
        yield return new WaitForSeconds(0.5f);

        //trigger crafting mode here!
        conversationTracker.EndIntroDialogue();

        yield return new WaitForSeconds(0.15f);
        focusedViewDialogueHub.SetFocusedHubConversationActiveWithSnippet(hangingNpcResponse);

        yield return new WaitWhile(() => !CraftingManager.FocusedOnCrafting && CraftingManager.CraftingUiBusy);

        conversationResponseManager.SetActiveInConversation(dialogue.playerIntro.SplitDialogueLines());
    }

    private IEnumerator RunCustomerDialogueOutro(string[] outro)
    {
        conversationTracker.TriggerOutro();

        yield return StartCoroutine(ReadMultiLineConversationSnippet(outro));
        yield return new WaitForSeconds(0.15f);

        conversationTracker.AdvanceDialogue();
        conversationTracker.EndOutroDialogue();
    }

    private IEnumerator ReadMultiLineConversationSnippet(string[] lines)
    {
        if (!lines.CollectionIsNotNullOrEmpty())
        {
            yield return null;
        }

        responseEnabled = false;
        conversationUiBusy = true;

        int tracker = 0;
        int amount = lines.Length;

        yield return new WaitForSeconds(0.5f);

        Vector2 spawnPoint = CraftingManager.FocusedOnCrafting 
            ? focusedViewDialogueHub.focusedConversationSnippetSpawnPoint 
            : conversationSnippetSpawnPoint;

        if (tracker < amount)
        {
            string snipLine = lines[tracker];

            ConversationSnippet active = snippetFactory.CreateNpcSnippet(snipLine, spawnPoint);

            BroadcastCustomerSnippetSpawn();

            while (tracker < amount)
            {
                yield return new WaitWhile(() => active.Typing);

                yield return new WaitForSeconds(0.15f);
                try
                {
                    active = snippetFactory.CreateNpcSnippet(lines[tracker + 1], spawnPoint);

                    BroadcastCustomerSnippetSpawn();

                    hangingNpcResponse = active;

                    responseTimeTracker.StartTrackingSnippet(hangingNpcResponse);

                    tracker++;
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
        }

        responseEnabled = true;
        conversationUiBusy = false;
    }

    private void BroadcastResponseSnippetSpawn()
    {
        ConversationSnippetEventArgs args = new ConversationSnippetEventArgs
        {
            SpawningResponse = true
        };

        SpawningNewConversationSnippet.BroadcastEvent(this, args);
    }

    private void BroadcastCustomerSnippetSpawn()
    {
        ConversationSnippetEventArgs args = new ConversationSnippetEventArgs
        {
            SpawningResponse = false
        };

        SpawningNewConversationSnippet.BroadcastEvent(this, args);
    }

    private void TriggerConversationResponse(KeyCode pressed)
    {
        if (!responding)
        {
            StartCoroutine(TakeResponseFromConversation(pressed));
        }
    }

    private void DestroyHangingResponse()
    {
        if (hangingNpcResponse != null)
        {
            responseTimeTracker.StopTracking();
            ConversationSnippet.DestroyAllCustomerResponseSnippets(this);
            hangingNpcResponse = null;
        }
    }
}
