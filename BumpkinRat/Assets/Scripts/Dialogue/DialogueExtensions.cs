using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueX
{
    static Dictionary<string, Indication> indicatorLookup => new Dictionary<string, Indication> {
        { "", Indication.None}, { "<ch>", Indication.Character},
        { "<tone>", Indication.Tone}, { "<audio>", Indication.Audio},
        { "<cond>", Indication.Condition }, { "<set>", Indication.Setter },
        { "<call>", Indication.Call }
    };
    static List<Indication> broadcastable => new List<Indication> {
        Indication.Audio,
        Indication.Setter,
        Indication.Tone,
        Indication.Call,
        Indication.Condition
    };
    static List<Indication> consumable => new List<Indication> {
        
    };

    static Dictionary<string, string> stringReplacementLookup => new Dictionary<string, string>
    {
        {"PLAYER_NAME", "Rat Man (PlayerBehavior):playerData.playerName" },
        { "NPC_TREE", " (NpcBehavior):dialogueStorage:currentTree:startIndex"},
        { "INV_ADJUST", "Rat Man (InventoryManager):activeInventory:"}
    };

    public static string nullTree => "NONE";

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

    public static string[] FormatMacros(this string line, char seperator, char joiner)
    {
        int s = line.IndexOf(joiner) + 1;
        if (s <= 0) { return line.Split(seperator); }

        StringBuilder build = new StringBuilder(line);
        char[] indexOfAny = { seperator, joiner };

        int e = (line.IndexOfAny(indexOfAny, s)) - s;
        string j = line.Substring(s, e);
        if (stringReplacementLookup.ContainsKey(j))
        {
            build.Replace(j, stringReplacementLookup[j]);
        }
        build.Remove(line.IndexOf(joiner), 1);
        Debug.Log(build.ToString());
        return build.ToString().FormatMacros(seperator, joiner);
    }

    public static bool BroadcastableIndicator(this Indication ind)
    {
        return broadcastable.Contains(ind);
    }

    public static bool ConsumableIndicator(this Indication ind)
    {
        return consumable.Contains(ind);
    }

    public static string GetIndicator(this string feed, bool inclusive = false)
    {
        string extracted = feed.SubstringBetweenChars('<', '>', inclusive);
        return extracted;
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

    public static bool CheckForIndication(this string line, Indication i)
    {
        return line.IndicationType().Equals(i);
    }
}

public enum Indication
{
    None, Character, Tone, Audio, Condition, Setter, Call
}

public enum NodeType
{
    Default, Option, Branch, Still, Bark
}

public class IndicatorArgs: EventArgs
{
    public Indication indicatorType;
    public string infoToParse;
    public char seperator = ':';
    public char joiner = '+';

    public string[] seperatedInfo
    {
        get
        {
            if (infoToParse.IndexOf(seperator) < 0) { return new string[] {infoToParse }; }
            return infoToParse.FormatMacros(seperator, joiner);
        }
    }

    public bool TargetObject(object o)
    {
        Debug.Log(o.ToString());
        if (!seperatedInfo.ValidArray()) { return false; }
        return o.ToString() == seperatedInfo[0];
    }
}
