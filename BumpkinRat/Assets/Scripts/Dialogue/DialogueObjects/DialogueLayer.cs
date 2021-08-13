using System;

[Serializable]
public class DialogueLayer 
{
    public DialogueResponse npcBaseResponse;

    public DialogueResponse[] additionalNpcResponses;

    public PlayerResponse[] playerResponses;
}
