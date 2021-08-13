using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Threading;

public static class DialogueX
{
    static Dictionary<string, Indication> indicatorLookup => new Dictionary<string, Indication> {
        { "", Indication.None}, { "<ch>", Indication.Character},
        { "<tone>", Indication.Tone}, { "<audio>", Indication.Audio},
        { "<cond>", Indication.Condition }, { "<set>", Indication.Setter },
        { "<call>", Indication.Call }
    };

    static Dictionary<string, Indication> IndicationLookup => new Dictionary<string, Indication>
    {

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

    public static List<string> PriorityLevels => new List<string>
    {
        "low", "medium", "high"
    };

    public static event EventHandler<DialogueCommandArgs> DialogueCommand;

    static Dictionary<string, string> stringReplacementLookup => new Dictionary<string, string>
    {
        {"PLAYER_NAME", "Rat Man (PlayerBehavior):playerData.playerName" },
        { "NPC_TREE", " (NpcBehavior):dialogueStorage:currentTree:startIndex"},
        { "INV_ADJUST", "Rat Man (InventoryManager):activeInventory:"}
    };

    public static string nullTree => "NONE";


    static Dictionary<string, string> cachedGetterLookup;

    public static string[] GetStringArray(this TextAsset txt)
    {
        return txt.text.Split("\n"[0]);
    }

    public static string StrippedIndicationLine(this string feed, out int remainderIndex)
    {
        int indicationStart = feed.IndexOf('|');
        if(indicationStart >= 0)
        {
            int indicationEnd = feed.IndexOf('|', indicationStart + 1);
            if(indicationEnd > 0)
            {
                remainderIndex = indicationEnd;
                return feed.Substring(indicationStart + 1, indicationEnd - 1);
            }
        }

        remainderIndex = 0;
        return string.Empty;
    }

    static void BroadcastRun(this string strippedLine, out bool wait)
    {
        wait = false;
        string[] segments = strippedLine.Split(':');
        if (segments.CollectionIsNotNullOrEmpty())
        {
            if (segments[0].Equals("GET"))
            {
                wait = true;
            }

            if (segments[0].Equals("SET"))
            {
                DialogueCommand.BroadcastEvent(strippedLine, new DialogueCommandArgs
                {
                    obj = segments[1],
                    setting = segments[2],
                    value = segments[3]
                });
            } 
        }
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

    static void AppendIfNot(this StringBuilder builder, char c, List<char> exclude)
    {
        if (!exclude.Contains(c))
        {
            builder.Append(c);
        }
    }

    static List<char> exclude = new List<char> {'|'};
    static Stack<string> getters = new Stack<string>();
    static bool successfullyStacked;

    public static void StackValue(object stacking)
    {
        successfullyStacked = true;
        getters.Push(stacking.ToString());
    }

    public static IEnumerator ReadLine<T>(this T reader, string line, StringBuilder builder, float delay = 0.001f) where T: MonoBehaviour
    {
        if (line == null)
        {
            line = string.Empty;
        }

        if (builder == null)
        {
            builder = new StringBuilder(line.Length);
        }

        int index = 0;

        while (index < line.Length)
        {
            char c = line.ElementAt(index);

            if (c.Equals('|'))
            {
                string strippedCommand = line.StrippedIndicationLine(out index);
                bool wait;
                strippedCommand.BroadcastRun(out wait);

                if (wait)
                {
                    Debug.Log("to do: waiting for return broadcast from receiver");
                    float timer = 0;
                    while (!successfullyStacked)
                    {
                        timer += Time.fixedDeltaTime;
                        if(timer >= 0.5f)
                        {
                            break;
                        }
                        yield return null;
                    }

                    if (successfullyStacked)
                    {
                        string stacked = getters.Pop();
                        yield return reader.ReadLine(stacked, builder, delay);
                        successfullyStacked = false;
                    }

                }
            }

            if (c.Equals('<'))
            {
                int endCarrot = line.IndexOf('>', index + 1);
                if (endCarrot < index)
                {
                    endCarrot = line.Length;
                }

                builder.Append(line, index, endCarrot - index);

                index = endCarrot;
            }
            else
            {
                builder.AppendIfNot(c, exclude);

                if (c != ' ')
                {
                    yield return new WaitForSeconds(delay);
                }

                index++;
            }

        }
    }

  

    public static IEnumerator ReadLine(string line, StringBuilder builder, float delay = 0.001f)
    {
        if(line == null)
        {
            line = string.Empty;
        }

        if(builder == null)
        {
            builder = new StringBuilder(line.Length);
        }

        int index = 0;

        while(index < line.Length)
        {
            char c = line.ElementAt(index);

            if (c.Equals('|'))
            {
                string strippedCommand = line.StrippedIndicationLine(out index);
                bool wait;
                strippedCommand.BroadcastRun(out wait);

            }

            if (c.Equals('<'))
            {
                int endCarrot = line.IndexOf('>', index + 1);
                if(endCarrot < index)
                {
                    endCarrot = line.Length;
                }

                builder.Append(line, index, endCarrot - index);

                index = endCarrot;
            } else
            {
                builder.AppendIfNot(c, exclude);

                if(c != ' ')
                {
                    yield return new WaitForSeconds(delay);
                }

                index++;
            }

        }
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

public class DialogueCommandArgs: EventArgs
{
    public string obj, value, setting;

    public bool ValidateTargetObject(object o)
    {
        return obj.Equals(o.ToString());
    }
}

public interface IDialogueCommandReceiver
{
    void OnDialogueCommand(object source, DialogueCommandArgs args);

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
