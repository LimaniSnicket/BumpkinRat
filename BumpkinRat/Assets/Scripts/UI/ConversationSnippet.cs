using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Runtime.CompilerServices;
using System.Text;
using System.Collections;

[RequireComponent(typeof(Image))]
public class ConversationSnippet : MonoBehaviour
{
    public Sprite onLeftBubble, onRightBubble;

    public Image BackingImage => GetComponent<Image>();
    public TextMeshProUGUI ConversationDisplayTMPro { get; private set; }

    private RectTransform thisRect;

    private ConversationUi conversationUI;

    StringBuilder builder;

    public bool isResponse;

    Vector2 originalPosition;

    static EventHandler DestroySnippet;

    public bool Typing { get; private set; } = false;

    private void OnEnable()
    {
        AssignOrCreateTMPro();
        thisRect = GetComponent<RectTransform>();
        builder = new StringBuilder();
    }

    private void Start()
    {
        DestroySnippet += OnDestroySnippet;
    }

    private void Update()
    {
        ConversationDisplayTMPro.text = builder.ToString();   
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

    public void InitializeSnippet(Vector2 pos)
    {
        if (isResponse)
        {
            thisRect.localPosition = new Vector2(-pos.x, pos.y);

        } else
        {
            thisRect.localPosition = pos;
        }

        originalPosition = thisRect.localPosition;


        BackingImage.sprite = !isResponse ? onLeftBubble : onRightBubble;

        if (isResponse)
        {
            ConversationDisplayTMPro.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -17);
        }
    }

    public void SetPositionAndScale(Vector2 pos, Vector2 scale)
    {
        thisRect.localPosition = pos;
        thisRect.localScale = scale;
    }

    public void OnSnippetSpawnMoveConversationBubble(object source, EventArgs args)
    {

        float scaleFactor = thisRect.localScale.x * 1.1f;

        Vector2 setPos = thisRect.localPosition + ((Vector3.right * 100) + (Vector3.up * 200)) * scaleFactor;
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

    public void SetResponseFromCustomerDialogueIntro(CustomerDialogue dialogue, int index)
    {
        if (dialogue.isValid)
        {
            if (isResponse)
            {
                ConversationDisplayTMPro.text = dialogue.introResponses[index].displayDialogue;
            }

        } else
        {
            Destroy(gameObject);
        }
    }

    public void ReadLine(string line, float delay)
    {
        StartCoroutine(ReadLineAndSetTyping(line, delay));
    }

    IEnumerator ReadLineAndSetTyping(string line, float delay)
    {
        Typing = true;
        yield return StartCoroutine(DialogueX.ReadLine(line, builder, delay));
        Typing = false;
    }


    public static void DestroyAllSnippets(object source)
    {
        DestroySnippet.BroadcastEvent(source);
    }

    void OnDestroySnippet(object source, EventArgs args)
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        UnSubscribeToConversationUiEvents();
        DestroySnippet -= OnDestroySnippet;
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

    public static ConversationAesthetic BasicConversationAesthetic
    {
        get
        {
            return new ConversationAesthetic
            {
                promptBubbleColor = Color.white,
                repsonseBubbleColor = Color.blue,
                promptTextColor = Color.black,
                responseTextColor = Color.white
            };
        }
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
