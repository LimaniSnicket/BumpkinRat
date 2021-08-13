using System;
using UnityEngine;

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