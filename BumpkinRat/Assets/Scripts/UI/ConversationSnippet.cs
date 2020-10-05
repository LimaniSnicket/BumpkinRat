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

    public bool isResponse;

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

        float scaleFactor = thisRect.localScale.x / 1;

        Vector2 setPos = thisRect.localPosition + Vector3.one * 100 * scaleFactor;
        Vector2 setScale = thisRect.localScale - Vector3.one * 0.2f;

        if(setScale.x <= 0)
        {
            Destroy(gameObject); //destroy to avoid negative scaling!
        }

        SetPositionAndScale(setPos, setScale);
    }

    public void ApplyConversationAesthetic(ConversationAesthetic aesthetic)
    {
        BackingImage.color = aesthetic.GetBubbleColor(isResponse);
        ConversationDisplayTMPro.color = aesthetic.GetTextColor(isResponse);
    }

    private void OnDestroy()
    {
        UnSubscribeToConversationUiEvents();
    }

}

[Serializable]
public struct ConversationAesthetic
{
    public Color promptBubbleColor, repsonseBubbleColor;
    public Color promptTextColor, responseTextColor;

    public Color GetBubbleColor(bool response)
    {
        return response ? repsonseBubbleColor : promptBubbleColor;
    }

    public Color GetTextColor(bool response)
    {
        return response ? responseTextColor : promptTextColor;
    }
    public static ConversationAesthetic SpookyConversationAesthetic
    {
        get
        {
            return new ConversationAesthetic
            {
                promptBubbleColor = ColorX.Orange,
                repsonseBubbleColor = Color.black,
                promptTextColor = Color.black,
                responseTextColor = ColorX.Orange
            };
        }
    }
}
