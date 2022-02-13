using System;
using UnityEngine;

[Serializable]
public class OverworldDialogue : IDialogue
{
    public int dialogueId;
    public int DialogueTypeId => 0;

    [SerializeField]
    private ResponseLayer[] npcDialogue;

    [SerializeField] private string[] npcLines;

    [SerializeField] private string[] throwaways;

    public bool IsValid()
    {
        return npcDialogue.CollectionIsNotNullOrEmpty();
    }
}
