using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Image))]
public class ConversationSnippet : MonoBehaviour
{
    public Image BackingImage => GetComponent<Image>();
    public TextMeshProUGUI ConversationDisplayTMPro { get; private set; }

    private RectTransform thisRect;

    private ConversationUi conversationUI;

    private void OnEnable()
    {
        AssignOrCreateTMPro();
        thisRect = GetComponent<RectTransform>();
    }

    void AssignOrCreateTMPro()
    {
        try
        {
            ConversationDisplayTMPro = GetComponentInChildren<TextMeshProUGUI>();
        } catch (NullReferenceException)
        {
            GameObject tmpro = new GameObject("ConversationTMP", typeof(TextMeshProUGUI));
            tmpro.transform.SetParent(transform);
            ConversationDisplayTMPro = tmpro.GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetConversationUi(ConversationUi convoUi)
    {
        conversationUI = convoUi;
        SubscribeToConversationUiEvents();
    }

    void SubscribeToConversationUiEvents()
    {
        if(conversationUI != null)
        {
            conversationUI.SpawningNewConversationSnippet += OnSnippetSpawnMoveConversationBubble;
        }
    }

    void UnSubscribeToConversationUiEvents()
    {
        if(conversationUI != null)
        {
            conversationUI.SpawningNewConversationSnippet -= OnSnippetSpawnMoveConversationBubble;
        }
    }
    public RectTransform GetRectTransform() => thisRect;

    public void SetPositionAndScale(Vector2 pos, Vector2 scale)
    {
        thisRect.localPosition = pos;
        thisRect.localScale = scale;
    }

    public void OnSnippetSpawnMoveConversationBubble(object source, EventArgs args)
    {
        Vector2 setPos = thisRect.localPosition + Vector3.one * 100;
        Vector2 setScale = thisRect.localScale - Vector3.one * 0.2f;
        SetPositionAndScale(setPos, setScale);
    }

    private void OnDestroy()
    {
        UnSubscribeToConversationUiEvents();
    }

}
