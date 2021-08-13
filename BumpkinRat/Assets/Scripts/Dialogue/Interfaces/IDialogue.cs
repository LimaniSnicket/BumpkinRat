
public interface IDialogue 
{
    int DialogueTypeId { get; }

    DialogueResponse[] DialogueResponses { get; }

    bool IsValid();
}
