using System;
public class CustomerDialogueTracker : DialogueTracker
{
    public event EventHandler CustomerDialogueExpired;

    public static event EventHandler IntroDialogueEnded;

    public static event EventHandler OutroDialogueTriggered;

    private CustomerDialogue tracking;

    private int dialogueIndex = -1;

    private bool mainDialogueComplete;

    private bool outroTriggered;

    public bool ResponseDialogueComplete => mainDialogueComplete;

    public CustomerDialogueTracker(CustomerDialogue dialogueToTrack)
    {
        tracking = dialogueToTrack;
        dialogueIndex = -1;
    }

    public string[] GetDisplayLinesAtDialogueIndex(int quality)
    {
        var layer = tracking.GetResponseAtLayer(dialogueIndex);

        return GetResponseFromLayer(layer, quality).SplitDialogueLines();
    }

    public string[] GetPlayerDialogueChoices()
    {
        if (dialogueIndex >= 0)
        {
            return tracking.responses[dialogueIndex].playerDialogue;
        }

        return tracking.playerIntro.SplitDialogueLines();
    }

    public string[] GetOutroDialogue()
    {
        if (tracking.customerOutro.CollectionIsNotNullOrEmpty())
        {
            return GetOutroDialogueOfQuality(tracking.customerOutro, Quality);
        }

        return Array.Empty<string>();
    }
    public void TriggerOutro()
    {
        outroTriggered = true;
    }

    private string GetResponseFromLayer(ResponseLayer responseLayer, int quality)
    {
        return this.GetDialogueOfQuality(responseLayer.npcDialogue, quality);
    }

    private string[] GetOutroDialogueOfQuality(string[] outro, int quality)
    {
        return this.GetDialogueOfQuality(outro, quality).SplitDialogueLines();
    }

    public void TrackNew(CustomerDialogue toTrack)
    {
        tracking = toTrack;
        dialogueIndex = -1;
        Quality = 0;
        mainDialogueComplete = false;
        outroTriggered = false;
    }

    public void EndIntroDialogue()
    {
        if (dialogueIndex == -1)
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
        dialogueIndex++;
        mainDialogueComplete = tracking.IsResponseDialogueComplete(dialogueIndex);

        if (mainDialogueComplete)
        {
            CustomerDialogueExpired.BroadcastEvent(this);
        }
    }

    public void AdvanceDialogueWithQuality(int qualityIncrease)
    {
        if (qualityIncrease > 0)
        {
            QualityIncreaseBy(qualityIncrease);
        }

        this.AdvanceDialogue();
    }
}

