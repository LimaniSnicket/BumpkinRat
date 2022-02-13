using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class NpcDialogue : IDialogue
{
    public int levelId;

    public int DialogueTypeId => 1;

    [SerializeField]
    private ResponseLayer[] npcDialogue;

    public bool IsValid()
    {
        return npcDialogue.CollectionIsNotNullOrEmpty();
    }
}
