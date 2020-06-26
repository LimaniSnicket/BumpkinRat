using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueExtensions 
{
    static Dictionary<string, Indication> indicatorLookup => new Dictionary<string, Indication> {
        { "", Indication.None}, { "<ch>", Indication.Character},
        { "<tone>", Indication.Tone}, { "<audio>", Indication.Audio} };


    public static string[] GetStringArray(this TextAsset txt)
    {
        return txt.text.Split("\n"[0]);
    }

    static Indication IndicationType(this string feed)
    {
        int s = feed.IndexOf('<');
        if (s < 0) { return Indication.None; }
        int e = feed.IndexOf('>');
        if (e < 0) { return Indication.None; }

        string f = feed.Substring(0, e + 1); 
        if (indicatorLookup.ContainsKey(f)) { return indicatorLookup[f]; }
        return Indication.None;
    }

    public static bool isIndicator(this string feed)
    {
        return feed.IndicationType() != Indication.None;
    }

    //<text>info</text>blah blah blah returns (<text>info</text>, blah blah blah)
    public static (string, string) SliceIndicator(this string feed)
    {
        int s = feed.IndexOf('>');
        if (s < 0) { return ("", feed); }
        int s2 = feed.IndexOf('>', s + 1);
        if (s2 < 0) { return ("", feed); }
        string ind = feed.Substring(0, s2 + 1);
        string line = feed.Substring(s2 + 2);
        return (ind, line);
    }

    //<text>info</text> returns (info, Indication.Text)
    public static (string, Indication) GetIndicationInfo(this string feed)
    {
        Indication i = feed.IndicationType();
        int s = feed.IndexOf('>') + 1;
        int e = (feed.IndexOf('/') - 1) - s;
        string inf = feed.Substring(s, e);
        return (inf, i);
    }

    public static string GetIndicator(this string feed, bool inclusive = false)
    {
        string extracted = feed.SubstringBetweenChars('<', '>', inclusive);
        return extracted;
    }

    public static string GetIndicatorEnd(this string feed, int start, string indicator)
    {
        if (feed.Length < start || start + indicator.Length > feed.Length) { return feed; }
        int cIndex = feed.IndexOf('<', start);
        if (cIndex < 0) { return feed; }
        string end = feed.Substring(start, indicator.Length + 1);
        return MatchingIndicators(indicator, end) ? end : feed;
    }

    static bool MatchingIndicators(string begin, string end)
    {
        string b = begin.Insert(1, "/");
        return string.Compare(b, end) == 0;
    }

    static bool ValidIndicator(this string feed) => indicatorLookup.ContainsKey(feed);

    static string SubstringBetweenChars(this string feed, char start, char end, bool inclusive)
    {
        int s = feed.IndexOf(start);
        if(s < 0) { Debug.Log("Line Feed does not contain " + start);  return feed; }
        int e = feed.IndexOf(end);
        return inclusive ? feed.Substring(s, e + 1) : feed.Substring(s + 1, e);
    }

    public static string StripIndicator(this string s, bool inclusive)
    {    
        int indexOfEndPointer = s.IndexOf('>');
        if (indexOfEndPointer < 0) { return s; } //return the base string if it doesn't have and end pointer
        int sub = inclusive ? indexOfEndPointer : indexOfEndPointer + 1;
        return sub > s.Length ? "" : s.Substring(sub);

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

public class IndicatorArgs: EventArgs
{
    public Indication indicatorType;
    public string infoToParse;
    public char seperator = '|';

    public string[] seperatedInfo
    {
        get
        {
            if (infoToParse.IndexOf(seperator) < 0) { return new string[] { }; }
            return infoToParse.Split(seperator);
        }
    }
}
