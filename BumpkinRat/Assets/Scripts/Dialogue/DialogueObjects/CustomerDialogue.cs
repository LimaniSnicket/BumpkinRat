using System;

[Serializable]
public class CustomerDialogue : IDialogue
{
    public int dialogueId;

    public string customerIntro;

    public string playerIntro;

    public ResponseLayer[] responses;

    public string[] customerOutro;
    public int DialogueTypeId => 2;

    public ResponseLayer GetResponseAtLayer(int index)
    {
        return responses[index];
    }

    public bool IsResponseDialogueComplete(int dialogueIndex)
    {
        return dialogueIndex >= responses.Length;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(customerIntro) && !string.IsNullOrEmpty(playerIntro) && (responses != null);
    }

    public override string ToString()
    {
        return $"[{dialogueId}]: {customerIntro}...";
    }
}

[Serializable]
public struct ResponseLayer
{
    public string[] npcDialogue;
    public string[] playerDialogue;
}