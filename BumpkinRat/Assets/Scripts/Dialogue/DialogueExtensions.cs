using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueExtensions 
{
    static Dictionary<string, Indication> indicatorLookup => new Dictionary<string, Indication> { { "", Indication.None}, { "ch", Indication.Character}, { "tone", Indication.Tone}, { "audio", Indication.Audio} };


    public static string[] GetStringArray(this TextAsset txt)
    {
        return txt.text.Split("\n"[0]);
    }

    //<blah>string</blah>
    static Indication GetIndication(this string feed)
    {
        string i = feed.GetIndicator();
        if (!i.ValidIndicator()) { return Indication.None; }
        return indicatorLookup[i];
    }

    static string GetIndicator(this string feed)
    {
        string extracted = feed.SubstringBetweenChars('<', '>');
        return extracted;
    }

    static bool ValidIndicator(this string feed) => indicatorLookup.ContainsKey(feed);

    static string SubstringBetweenChars(this string feed, char start, char end)
    {
        int s = feed.IndexOf(start);
        if(s < 0) { Debug.Log("Line Feed does not contain " + start);  return feed; }
        int e = feed.IndexOf(end);
        return feed.Substring(s + 1, e - 1);
    }
}

public enum Indication
{
    None, Character, Tone, Audio
}

public enum NodeType
{
    Default, Option, Branch, Still, Bark
}
