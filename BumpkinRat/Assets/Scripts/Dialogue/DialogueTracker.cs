using System;
using System.Collections.Generic;
using System.Linq;
public class DialogueTracker
{
    static List<string> CompletedDialogue { get; set; } = new List<string>();

    public int QualityIndex { get; protected set; } = 0;

    public static void CacheCompletedDialogue(string entry)
    {
        CompletedDialogue.Add(entry);
    }

    protected static bool CheckCache(string check)
    {
        return CompletedDialogue.CollectionIsNotNullOrEmpty() && CompletedDialogue.Contains(check);
    }

    public void IncreaseQualityIndex(int add)
    {
        QualityIndex += add;
    }

    internal DialogueResponse GetDialogueResponseOfQuality(DialogueResponse[] responses, int quality)
    {
        var qualityResponses = responses.Where(r => r.qualityThreshold <= quality);

        if (qualityResponses.CollectionIsNotNullOrEmpty())
        {
            return qualityResponses.OrderByDescending(r => r.qualityThreshold).First();
        }

        throw new ArgumentOutOfRangeException($"No Response of quality value {quality} found.");
    }
}
