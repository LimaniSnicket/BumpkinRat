using System;
public class CustomerDialogueTracker : DialogueTracker
{
    public CustomerDialogue Tracking { get; private set; }
    public int DialogueIndex { get; private set; } = -1;

    bool outroTriggered;

    public bool MainDialogueComplete { get; private set; }

    public event EventHandler CustomerDialogueExpired;

    public static event EventHandler IntroDialogueEnded;

    public static event EventHandler OutroDialogueTriggered;

    public CustomerDialogueTracker(CustomerDialogue dialogueToTrack)
    {
        Tracking = dialogueToTrack;
        DialogueIndex = -1;
    }
    public string[] GetDisplayLinesAtDialogueIndex(int quality)
    {
        DialogueLayer response = Tracking.GetCustomerResponseAtIndex(DialogueIndex);
        return GetDialogueResponseOfQuality(response, quality).displayDialogue;
    }

    public PlayerResponse[] GetResponses()
    {
        try
        {

            return DialogueIndex >= 0 
                ? Tracking.promptedCustomerDialogue[DialogueIndex].playerResponses 
                : Tracking.introResponses;

        }
        catch (IndexOutOfRangeException)
        {
            return Array.Empty<PlayerResponse>();
        }
    }

    public DialogueResponse GetOutroDialogue()
    {
        if (Tracking.outroDialogue.CollectionIsNotNullOrEmpty())
        {
            return GetDialogueResponseOfQuality(Tracking.outroDialogue, QualityIndex);
        }

        return DialogueResponse.Null;
    }
    public void TriggerOutro()
    {
        outroTriggered = true;
    }

    DialogueResponse GetDialogueResponseOfQuality(DialogueLayer response, int quality)
    {
        if (response.additionalNpcResponses.CollectionIsNotNullOrEmpty())
        {
            if (quality >= response.additionalNpcResponses[0].qualityThreshold)
            {
                return GetDialogueResponseOfQuality(response.additionalNpcResponses, quality);
            }
        }

        return response.npcBaseResponse;
    }

    public void Reset(CustomerDialogue newTracking)
    {
        Tracking = newTracking;
        DialogueIndex = -1;
        QualityIndex = 0;
        MainDialogueComplete = false;
        outroTriggered = false;
    }

    public void EndIntroDialogue()
    {
        if (DialogueIndex == -1)
        {
            IntroDialogueEnded.BroadcastEvent(this);
        }
    }

    public void EndOutroDialogue()
    {
        if (outroTriggered)
        {
            OutroDialogueTriggered.BroadcastEvent(this);
        }
    }

    public void AdvanceDialogue()
    {
        DialogueIndex++;
        MainDialogueComplete = Tracking.promptedCustomerDialogue.Length <= DialogueIndex;

        if (MainDialogueComplete)
        {
            CustomerDialogueExpired.BroadcastEvent(this);
        }
    }

    public void AdvanceDialogueWithQuality(int qualityIncrease)
    {
        if (qualityIncrease > 0)
        {
            IncreaseQualityIndex(qualityIncrease);
        }

        AdvanceDialogue();
    }
}

