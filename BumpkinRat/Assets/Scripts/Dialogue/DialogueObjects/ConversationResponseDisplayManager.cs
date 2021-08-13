using System.Collections.Generic;
using System.Linq;

public class ConversationResponseDisplayManager 
{
    private readonly Dictionary<ResponseTier, ConversationResponseDisplay> activeInConversation;

    public ConversationResponseDisplayManager()
    {
        activeInConversation = new Dictionary<ResponseTier, ConversationResponseDisplay>();
    }

    public bool IsActive(ResponseTier tier)
    {
        return activeInConversation[tier].Active;
    }

    public ConversationResponseDisplay GetDisplayForResponseTier(ResponseTier tier)
    {
        return activeInConversation[tier];
    }

    public void AddToActiveInConversation(ConversationResponseDisplay response, ResponseTier priorityIndex)
    {
        activeInConversation.AddOrReplaceKeyValue(priorityIndex, response);
    }

    public void SetActiveInConversation(PlayerResponse[] responses)
    {
        if (responses.CollectionIsNotNullOrEmpty())
        {
            foreach (var response in responses)
            {
                ResponseTier t = (ResponseTier)response.responseLevel;
                if (activeInConversation.ContainsKey(t))
                {
                    activeInConversation[t].ActivateResponseWithMessage(response.displayDialogue);
                }
            }
        }
    }

    public void SetAllInactive()
    {
        for (int i = 0; i < activeInConversation.Values.Count; i++)
        {
            activeInConversation.ElementAt(i).Value.SetActiveState(false);
        }
    }
}
