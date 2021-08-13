using System;

[Serializable]
public class CustomerDialogue : IDialogue
{
    public int dialogueId;
    public int levelId;

    public string[] introLines;
    public PlayerResponse[] introResponses;
    public DialogueLayer[] promptedCustomerDialogue;

    public DialogueResponse[] outroDialogue;
    public int DialogueTypeId => 2;
    public int IntroLineCount => IsValid() ? introLines.Length : 0;

    public DialogueResponse[] DialogueResponses => throw new NotImplementedException();

    public DialogueLayer GetCustomerResponseAtIndex(int index)
    {
        if (promptedCustomerDialogue.ValidArray())
        {
            int clampIndex = Math.Min(promptedCustomerDialogue.Length - 1, Math.Max(0, index));
            return promptedCustomerDialogue[clampIndex];
        }
        return new DialogueLayer();
    }

    public bool IsValid()
    {
        return introLines.CollectionIsNotNullOrEmpty()
        && promptedCustomerDialogue.CollectionIsNotNullOrEmpty()
        && introResponses.CollectionIsNotNullOrEmpty();
    }

    public override string ToString()
    {
        return $"[{dialogueId}-{levelId}]: {introLines[0]}...";
    }
}