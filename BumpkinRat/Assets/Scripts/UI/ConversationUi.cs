using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ConversationUi : MonoBehaviour
{
    public GameObject conversationSnippetPrefab;
    public Vector2 conversationSnippetSpawnPoint;

    Stack<string> storedMessages;

    private bool responding;

    public bool CanRespond => responding;
    public string joined;

    StringBuilder stringBuilder;
    LoremIpsum loremIpsum;

    public TextMeshProUGUI displayConversationMessages;

    public event EventHandler SpawningNewConversationSnippet;

    public ConversationAesthetic currentConversationAesthetic;

    private void Start()
    {
        storedMessages = new Stack<string>();
        stringBuilder = new StringBuilder();
        loremIpsum = new LoremIpsum();
        currentConversationAesthetic = ConversationAesthetic.SpookyConversationAesthetic;
    }

    private void Update()
    {
        UpdateConversationDisplay();

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RespondMessage(KeyCode.Alpha1);
        }  

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            RespondMessage(KeyCode.Alpha2);
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
            StartCoroutine(TakeResponse(pressed));
        }
    }

    IEnumerator TakeResponse(KeyCode pressed)
    {
        responding = true;

        BroadcastMessageSpawning();

        string response = $"{$"Responding with {pressed}", 0:F3}";
        storedMessages.Push(response);

        InstantiateConversationSnippet(storedMessages.Peek(), true);
        yield return new WaitForSeconds(1);


        stringBuilder.AppendLine(storedMessages.Peek());
        joined = stringBuilder.ToString();

        yield return new WaitForSeconds(1);
        BroadcastMessageSpawning();

        SetConversationNodeToRespondTo();
        responding = false;
    }

    void SetConversationNodeToRespondTo()
    {
        string lorem = loremIpsum.GetLoremIpsum();
        storedMessages.Push(lorem + ".");
        stringBuilder.AppendLine(storedMessages.Peek());
        joined = stringBuilder.ToString();

        InstantiateConversationSnippet(storedMessages.Peek(), false);
    }

    void UpdateConversationDisplay()
    {
        if(displayConversationMessages != null)
        {
            displayConversationMessages.text = joined;
        }
    }

    public void InstantiateConversationSnippet(string message, bool response)
    {
        GameObject snippet = Instantiate(conversationSnippetPrefab, transform);
        ConversationSnippet convoSnippet = snippet.GetComponent<ConversationSnippet>();
        convoSnippet.isResponse = response;
        convoSnippet.SetConversationUi(this);
        convoSnippet.ApplyConversationAesthetic(currentConversationAesthetic);
        convoSnippet.SetPositionAndScale(conversationSnippetSpawnPoint, Vector2.one);
        convoSnippet.ConversationDisplayTMPro.text = message;
    }
}
