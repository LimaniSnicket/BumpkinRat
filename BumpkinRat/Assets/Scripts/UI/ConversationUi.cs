using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ConversationUi : MonoBehaviour
{
    public GameObject conversationSnippetPrefab;
    public GameObject uiContainer, responseContainer;

    public Vector2 conversationSnippetSpawnPoint;

    Stack<string> storedMessages;

    private bool responding;
    private bool responseEnabled;

    public bool CanRespond => responding && responseEnabled;
    public string joined;

    StringBuilder stringBuilder;
    LoremIpsum loremIpsum;

    public event EventHandler SpawningNewConversationSnippet;

    public ConversationAesthetic currentConversationAesthetic;
    ConversationResponseDisplay[] conversationResponses;

    public CustomerDialogueTracker conversationTracker;

    Dictionary<KeyCode, (string, float)> keyToResponseLevel = new Dictionary<KeyCode, (string, float)>
    {
        { KeyCode.A, ("low", 1) },
        { KeyCode.S, ("medium", 1.5f) },
        { KeyCode.D, ("high", 2) }
    };

    private void Start()
    {
        storedMessages = new Stack<string>();
        stringBuilder = new StringBuilder();
        loremIpsum = new LoremIpsum();
        currentConversationAesthetic = ConversationAesthetic.RuralAesthetic;//SpookyConversationAesthetic;
        conversationResponses = ConversationResponseDisplay.GetResponseDisplays(responseContainer.transform.GetChildren(), 
            currentConversationAesthetic);

        CustomerDialogue activeConversation = GeneralStorePrologue.CraftingOrderTest.GetCustomerDialogue();
        conversationTracker = CustomerDialogueTracker.GetCustomerDialogueTracker(activeConversation);

        StartCoroutine(RunCustomerDialogueIntro(activeConversation));
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

        foreach(KeyValuePair<KeyCode, (string, float)> pairs in keyToResponseLevel)
        {
            if (Input.GetKeyDown(pairs.Key))
            {
                RespondMessage(pairs.Key);
            }
        }

    }

    void BroadcastMessageSpawning()
    {
        if (SpawningNewConversationSnippet != null)
        {
            SpawningNewConversationSnippet(this, new EventArgs());
        }
    }

    public void RespondMessage(KeyCode pressed)
    {
        if (!responding)
        {
            StartCoroutine(TakeResponseFromConversation(pressed, conversationTracker));
        }
    }

    IEnumerator TakeResponseFromConversation(KeyCode pressed, CustomerDialogueTracker tracker)
    {
        (string, float) level = (string.Empty, 1);
        bool valid = keyToResponseLevel.TryGetValue(pressed, out level);
        responding = true;

        Dictionary<string, string> responseMap = tracker.GetResponses().
            ToDictionary(r => r.responseLevel, r => r.displayDialogue);

        if (responseMap.ContainsKey(level.Item1) && !tracker.DialogueComplete)
        {
            string message = responseMap[level.Item1];

            ConversationResponseDisplay.SetAllInactive();

            if (message != string.Empty)
            {
                ConversationSnippet snip;
                InstantiateConversationSnippet(message, true, out snip);
                BroadcastMessageSpawning();

                while (snip.Typing)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForSeconds(0.5f);

            }

            yield return StartCoroutine(GetCustomerResponse(tracker, level.Item1, level.Item2));

        }

        responding = false;

        yield return null;
    }

    IEnumerator GetCustomerResponse(CustomerDialogueTracker tracker, string level, float distraction)
    {
        BroadcastMessageSpawning();
        ConversationSnippet snip;

        tracker.Advance();

        string message = tracker.Tracking.promptedCustomerDialogue[tracker.DialogueIndex].GetCustomerResponse(level);

        CraftingUI.Distract(distraction);

        InstantiateConversationSnippet(message, false, out snip);

        while (snip.Typing)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.2f);

        ConversationResponseDisplay.SetFromConversationResponses(tracker.GetResponses());

        if (tracker.DialogueComplete)
        {
            ConversationSnippet.DestroyAllSnippets(this);
        }
    }

    public void InstantiateConversationSnippet(string message, bool response, out ConversationSnippet snippet)
    {
        ConversationSnippet snip = GetSnippet(message, response);
        snippet = snip;
    }

    IEnumerator RunCustomerDialogueIntro(CustomerDialogue dialogue)
    {
        yield return StartCoroutine(ReadMultiLineConversationSnippet(dialogue.introLines));
        yield return new WaitForSeconds(0.2f);
        ConversationResponseDisplay.SetFromConversationResponses(dialogue.introResponses);
    }

    IEnumerator ReadMultiLineConversationSnippet(string[] lines)
    {
        if (!lines.CollectionIsNotNullOrEmpty())
        {
            yield return null;
        }

        responseEnabled = false;

        int tracker = 0;
        int amount = lines.Length;

        yield return new WaitForSeconds(0.5f);

        ConversationSnippet active = GetSnippet(lines[tracker], false);
        BroadcastMessageSpawning();

        while (tracker < amount)
        {
            if (active.Typing)
            {
                yield return new WaitForEndOfFrame();
            } else
            {
                yield return new WaitForSeconds(1.2f);
                try
                {
                    active = GetSnippet(lines[tracker + 1], false);
                    BroadcastMessageSpawning();
                    tracker++;
                }
                catch (IndexOutOfRangeException)
                {
                    break;
                }
            }
        }

        responseEnabled = true;

    }

    ConversationSnippet GetSnippet(string message, bool response)
    {
        GameObject snippet = Instantiate(conversationSnippetPrefab, transform);
        ConversationSnippet convoSnippet = snippet.GetComponent<ConversationSnippet>();
        convoSnippet.isResponse = response;
        convoSnippet.SetConversationUi(this);
        convoSnippet.ApplyConversationAesthetic(currentConversationAesthetic);
        convoSnippet.InitializeSnippet(conversationSnippetSpawnPoint);
        convoSnippet.ReadLine(message, 0.07f);
        return convoSnippet;
    }
}
