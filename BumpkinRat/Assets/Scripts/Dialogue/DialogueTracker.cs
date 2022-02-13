using System.Collections.Generic;
using System.Linq;
public class DialogueTracker
{
    private static List<string> CompletedDialogue { get; set; } = new List<string>();

    public int Quality { get; protected set; } = 0;

    public static void CacheCompletedDialogue(string entry)
    {
        CompletedDialogue.Add(entry);
    }

    protected static bool CheckCache(string check)
    {
        return CompletedDialogue.CollectionIsNotNullOrEmpty() && CompletedDialogue.Contains(check);
    }

    protected void QualityIncreaseBy(int add)
    {
        Quality += add;
    }

    protected string GetDialogueOfQuality(string[] arr, int quality)
    {
        foreach (var response in arr)
        {
            if (response.StartsWith(quality.ToString()))
            {
                return response;
            }
        }

        return arr[0];
    }
}

public static class DialogueUtility
{
    private const char  QualitySplitter = ':';

    private const char LineSplitter = '_';

    public static string[] SplitDialogueLines(this string line)
    {
        return line.Split(LineSplitter);
    }

    public static (string, ResponseTier) LineWithQuality(this string line)
    {
        if (!line.Contains(QualitySplitter))
        {
            return (line, 0);
        }

        var split = line.Split(QualitySplitter);
        var quality = GetResponseTier(split[0]);

        return (split[1], quality);
    }

    private static ResponseTier GetResponseTier(string str)
    {
        if (str.Contains("1"))
        {
            return ResponseTier.MID;
        }

        if (str.Contains("2"))
        {
            return ResponseTier.BEST;
        }

        return ResponseTier.LOW;
    }
}
