public class DialogueTrackerFactory 
{
    public CustomerDialogueTracker CreateCustomerDialogueTracker(CustomerDialogue tracking)
    {
        return new CustomerDialogueTracker(tracking);
    }
}
