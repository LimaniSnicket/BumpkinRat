using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcDialogue : IDialogue
{
    public int levelId;

    public int DialogueTypeId => 1;

    public DialogueResponse[] DialogueResponses => npcDialogue ?? Array.Empty<DialogueResponse>();

    [SerializeField]
    private DialogueResponse[] npcDialogue;

    public bool IsValid()
    {
        return npcDialogue.CollectionIsNotNullOrEmpty();
    }
}
