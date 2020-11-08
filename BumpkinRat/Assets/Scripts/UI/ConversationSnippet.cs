using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

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
    Vector2 defaultResponseTextPos = new Vector2(-55,7); 

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
        thisRect.DOScale(Vector3.one, 0.7f);


        BackingImage.sprite = !isResponse ? onLeftBubble : onRightBubble;

        if (isResponse)
        {

            ConversationDisplayTMPro.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -17);
            ConversationDisplayTMPro.GetComponent<RectTransform>().localPosition = defaultResponseTextPos;

        }
 
    }

    public void SetPositionAndScale(Vector2 pos, Vector2 scale)
    {
        thisRect.localPosition = pos;
        thisRect.localScale = scale;
    }

    public void MoveToNewPositionAndScale(Vector2 pos, Vector2 scale)
    {
        thisRect.DOLocalMove(pos, 1);
        thisRect.DOScale(scale, 0.5f);
    }
    
    bool newSnip = true;

    public void OnSnippetSpawnMoveConversationBubble(object source, EventArgs args)
    {
        if (!newSnip)
        {

            float scaleFactor = thisRect.localScale.x * 1.2f;

            Vector2 setPos = thisRect.localPosition + ((Vector3.right * 100) + (Vector3.up * 200)) * scaleFactor;
            Vector2 setScale = thisRect.localScale - Vector3.one * 0.4f;

            if (setScale.x <= 0)
            {
                Destroy(gameObject); //destroy to avoid negative scaling!
            }

            MoveToNewPositionAndScale(setPos, setScale);
        }

        newSnip = false;
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

        if (ActiveInConversation == null)
        {
            ActiveInConversation = new Dictionary<string, ConversationResponseDisplay>();
        }
    }

    public static ConversationResponseDisplay[] GetResponseDisplays(GameObject[] uiElements, ConversationAesthetic aesthetics)
    {
        ConversationResponseDisplay[] arr = new ConversationResponseDisplay[3];

        for (int i = 0; i < arr.Length; i++)
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
            for (int i = 0; i < priorities.Length; i++)
            {
                string level = priorities[i];

                try
                {
                    ActiveInConversation[level].SetDisplayString(mapResponses[level]);
                    ActiveInConversation[level].SetActiveState(true);

                }
                catch (KeyNotFoundException)
                {

                }
            }
        }
    }

    public static void SetAllInactive()
    {
        for (int i = 0; i < ActiveInConversation.Values.Count; i++)
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
        activeColor = aesthetic.GetBubbleColor(true);
        inactiveColor = new Color(activeColor.r, activeColor.g, activeColor.b, 0.5f);

        backing.color = inactiveColor;
        textMesh.color = aesthetic.GetTextColor(true);
    }

    void SetActiveState(bool active)
    {
        if (active)
        {
            backing.color = activeColor;
            rectTransform.DOScale(Vector3.one, 0.5f);
            //rectTransform.localScale = Vector3.one;

        }
        else
        {
            backing.color = inactiveColor;
            rectTransform.DOScale(Vector3.one * 0.5f, 0.2f);
            //rectTransform.localScale = Vector3.one * 0.5f;
            SetDisplayString(". . .");
        }

        Active = active;
    }

    public override string ToString()
    {
        return responseMessageDisplay;
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

    public static ConversationAesthetic RuralAesthetic
    {
        get
        {
            return new ConversationAesthetic
            {
                promptBubbleColor = ColorX.Auburn,
                repsonseBubbleColor = new Color(0.3f, 0.3f, 0.3f, 1),
                promptTextColor = Color.white,
                responseTextColor = Color.white
            };
        }
    }
}
