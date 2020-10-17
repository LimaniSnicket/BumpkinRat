﻿using System;
using System.Collections;
using System.Collections.Generic;
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

    public bool CanRespond => responding;
    public string joined;

    StringBuilder stringBuilder;
    LoremIpsum loremIpsum;

    public event EventHandler SpawningNewConversationSnippet;

    public ConversationAesthetic currentConversationAesthetic;
    ConversationResponseDisplay[] conversationResponses;

    KeyCode[] inputKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3 };

    private void Start()
    {
        storedMessages = new Stack<string>();
        stringBuilder = new StringBuilder();
        loremIpsum = new LoremIpsum();
        currentConversationAesthetic = ConversationAesthetic.SpookyConversationAesthetic;
        conversationResponses = ConversationResponseDisplay.GetResponseDisplays(responseContainer.transform.GetChildren(), currentConversationAesthetic);
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
        UpdateConversationDisplay();


        for(int i = 0; i< inputKeys.Length; i++)
        {
            if (Input.GetKeyDown(inputKeys[i]))
            {
                RespondMessage(inputKeys[i]);
                Debug.Log(conversationResponses[i].ToString());
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
        yield return new WaitForSeconds(0.25f);


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

[Serializable]
public struct ConversationResponseDisplay
{
    //set to low, medium, or high
    static string[] priorities = {"Low", "Medium", "High" };
    private int priority;

    public int Priority => priority;

    private string responseMessageDisplay;

    private Image backing;
    private TextMeshProUGUI textMesh;

    ConversationResponseDisplay(int pri, GameObject prefab)
    {
        priority = pri;
        responseMessageDisplay = string.Empty;

        backing = prefab.GetOrAddComponent<Image>();
        textMesh = backing.GetComponentInChildren<TextMeshProUGUI>();
    }

    public static ConversationResponseDisplay[] GetResponseDisplays(GameObject[] uiElements, ConversationAesthetic aesthetics)
    {
        ConversationResponseDisplay[] arr = new ConversationResponseDisplay[3];

        for(int i = 0; i< arr.Length; i++)
        {
            arr[i] = new ConversationResponseDisplay(i, uiElements[i]);
            arr[i].SetDisplayString($"{priorities[i]} Level Response!");
            arr[i].ApplyConversationAesthetic(aesthetics);
        }

        return arr;
    }

    public void SetDisplayString(string message)
    {
        responseMessageDisplay = message;
        textMesh.text = responseMessageDisplay;
    }

    public void SetPriority(int i)
    {
        priority = Mathf.Clamp(i, 0, 2);
    }

    public void ApplyConversationAesthetic(ConversationAesthetic aesthetic)
    {
        backing.color = aesthetic.GetBubbleColor(false);
        textMesh.color = aesthetic.GetTextColor(false);
    }

    public override string ToString()
    {
        return responseMessageDisplay;
    }

}
