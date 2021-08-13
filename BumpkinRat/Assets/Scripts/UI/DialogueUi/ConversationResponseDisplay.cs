using System;
using UnityEngine;

[Serializable]
public class ConversationResponseDisplay : IBubbleDisplay
{
    private string responseMessageDisplay;

    private readonly BubbleDisplay bubbleElements;

    private const float InactiveScaleFactor = 0.5f;

    private const float InactiveTweenSpeed = 0.2f;

    private const float ActiveScaleFactor = 0.85f;

    private const float ActiveTweenSpeed = 0.5f;

    public BubbleDisplay BubbleElements => bubbleElements;

    public string DisplayMessage => responseMessageDisplay;

    public bool Active { get; private set; }

    public GameObject gameObject { get; private set; }

    public ConversationResponseDisplay(GameObject prefab)
    {
        responseMessageDisplay = string.Empty;

        gameObject = prefab;
        bubbleElements = new BubbleDisplay(prefab);
        Active = false;
    }

    public void InitializeConversationResponseDisplayBubble(ConversationAesthetic aesthetic)
    {
        bubbleElements.SetDisplayToEllipses();
        bubbleElements.ApplyConversationAesthetic(aesthetic);
    }

    public void ActivateResponseWithMessage(string message)
    {
        responseMessageDisplay = message;
        bubbleElements.SetDisplayString(message);
        SetActiveState(true);
    }

    public void SetActiveState(bool active)
    {
        if (!active)
        {
            bubbleElements.SetToInactiveState(InactiveScaleFactor, InactiveTweenSpeed);
            bubbleElements.SetDisplayString(". . .");
        }
        else 
        {
            bubbleElements.SetToActiveState(ActiveScaleFactor, ActiveTweenSpeed);
        }

        Active = active;
    }

}

public enum ResponseTier
{
    LOW = 0, 
    MID = 1,
    BEST = 2
}

