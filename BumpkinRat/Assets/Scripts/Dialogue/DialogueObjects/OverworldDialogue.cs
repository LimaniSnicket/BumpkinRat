using System;
using UnityEngine;

[Serializable]
public class OverworldDialogue : IDialogue
{
    public int dialogueId;
    public int DialogueTypeId => 0;

    public DialogueResponse[] DialogueResponses => npcDialogue ?? Array.Empty<DialogueResponse>();

    [SerializeField]
    private DialogueResponse[] npcDialogue;

    public bool IsValid()
    {
        return npcDialogue.CollectionIsNotNullOrEmpty();
    }
}
