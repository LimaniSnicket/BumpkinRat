using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        { KeyCode.Alpha1, ("low", 1) },
        { KeyCode.Alpha2, ("medium", 1.5f) },
        { KeyCode.Alpha3, ("high", 2) }
    };

    private void Start()
    {
        storedMessages = new Stack<string>();
        stringBuilder = new StringBuilder();
        loremIpsum = new LoremIpsum();
        currentConversationAesthetic = ConversationAesthetic.SpookyConversationAesthetic;
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
            StartCoroutine(TakeResponseFromConversationIntro(pressed, conversationTracker));
        }
    }

    IEnumerator TakeResponseFromConversationIntro(KeyCode pressed, CustomerDialogueTracker tracker)
    {
        (string, float) level = (string.Empty, 1);
        bool valid = keyToResponseLevel.TryGetValue(pressed, out level);

        responding = true;

        Dictionary<string, string> responseMap = tracker.Tracking.introResponses.ToDictionary(r => r.responseLevel, r => r.displayDialogue);

        if (responseMap.ContainsKey(level.Item1) && !tracker.DialogueComplete)
        {
            string message = responseMap[level.Item1];

            ConversationResponseDisplay.SetAllInactive();

            if (message != string.Empty)
            {
                ConversationSnippet snip;
                InstantiateConversationSnippet(message, true, out snip);

                while (snip.Typing)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield return new WaitForSeconds(0.25f);

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

        string message = tracker.Tracking.promptedCustomerDialogue[tracker.DialogueIndex].GetCustomerResponse(level);

        CraftingUI.Distract(distraction);

        InstantiateConversationSnippet(message, false, out snip);

        while (snip.Typing)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.2f);

        ConversationResponseDisplay.SetFromConversationResponses(tracker.GetResponses());
        tracker.Advance();

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

        Debug.Log($"Reading multiple lines: {amount}");

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

[Serializable]
public struct ConversationResponseDisplay
{
    static string[] priorities = { "low", "medium", "high" };
    private int priority;

    public int Priority => priority;

    private string responseMessageDisplay;

    private Image backing;
    private TextMeshProUGUI textMesh;
    private RectTransform rectTransform;

    private Color activeColor, inactiveColor;

    public bool Active { get; private set; }

    public static Dictionary<string, ConversationResponseDisplay> ActiveInConversation { get; private set; }

    ConversationResponseDisplay(int pri, GameObject prefab)
    {
        priority = SetPriority(pri);
        responseMessageDisplay = string.Empty;

        backing = prefab.GetOrAddComponent<Image>();
        textMesh = backing.GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = backing.GetComponent<RectTransform>();
        activeColor = Color.white;
        inactiveColor = Color.clear;
        Active = false;

        if(ActiveInConversation == null)
        {
            ActiveInConversation = new Dictionary<string, ConversationResponseDisplay>();
        }
    }

    public static ConversationResponseDisplay[] GetResponseDisplays(GameObject[] uiElements, ConversationAesthetic aesthetics)
    {
        ConversationResponseDisplay[] arr = new ConversationResponseDisplay[3];

        for(int i = 0; i< arr.Length; i++)
        {
            arr[i] = new ConversationResponseDisplay(i, uiElements[i]);
            arr[i].SetDisplayString(". . .");
            arr[i].ApplyConversationAesthetic(aesthetics);

            ActiveInConversation.AddOrReplaceKeyValue(priorities[i], arr[i]);

            arr[i].SetActiveState(false);
        }

        return arr;
    }

    public static void SetFromConversationResponses(PlayerResponse[] responses)
    {
        if (responses.CollectionIsNotNullOrEmpty())
        {
            Dictionary<string, string> mapResponses = responses.ToDictionary(r => r.responseLevel.ToLower(), r => r.displayDialogue);
            for(int i = 0; i < priorities.Length; i++)
            {
                string level = priorities[i];

                try
                {
                    ActiveInConversation[level].SetDisplayString(mapResponses[level]);
                    ActiveInConversation[level].SetActiveState(true);

                } catch (KeyNotFoundException)
                {

                }
            }
        }
    }

    public static void SetAllInactive()
    {
        for(int i = 0; i < ActiveInConversation.Values.Count; i++)
        {
            ActiveInConversation.ElementAt(i).Value.SetActiveState(false);
        }
    }

    public string GetResponseLevel()
    {
        return priorities[priority];
    }

    public void SetDisplayString(string message)
    {
        responseMessageDisplay = message;
        textMesh.text = responseMessageDisplay;
    }

    static int SetPriority(int i)
    {
        return Mathf.Clamp(i, 0, 2);
    }

    public void ApplyConversationAesthetic(ConversationAesthetic aesthetic)
    {
        activeColor = aesthetic.GetBubbleColor(false);
        inactiveColor = new Color(activeColor.r, activeColor.g, activeColor.b, 0.5f);

        backing.color = inactiveColor;
        textMesh.color = aesthetic.GetTextColor(false);
    }

    void SetActiveState(bool active)
    {
        if (active)
        {
            backing.color = activeColor;
            rectTransform.localScale = Vector3.one;

        } else
        {
            backing.color = inactiveColor;
            rectTransform.localScale = Vector3.one * 0.5f;
            SetDisplayString(". . .");
        }

        Active = active;
    }

    public override string ToString()
    {
        return responseMessageDisplay;
    }

}
