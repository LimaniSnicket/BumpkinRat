using System.Collections.Generic;
using UnityEngine;

public class OverworldDialogueUI : MonoBehaviour, IComparer<OverworldNpc>
{
    private static OverworldDialogueUI overworldDialogueUi;

    public static OverworldNpc activeOverworldNpc;

    public GameObject dialogueSnippetPrefab;

    static List<OverworldNpc> withinRange;

    static OverworldDialogueSnippet activeSnippet;

    static Vector2 defaultStartPosition => new Vector2(-1082, 350);
    static Vector2 inViewPosition => new Vector2(-853, 350);

    public static bool HasActiveOverworldNpc => activeOverworldNpc != null;

    private void Awake()
    {
        if (overworldDialogueUi == null)
        {
            overworldDialogueUi = this;
        }
        else
        {
            Destroy(this);
        }

        OverworldNpc.OverworldNpcEvent += OnOverworldNpcEvent;
    }

    private void Start()
    {
        withinRange = new List<OverworldNpc>();
        InitializeDialogueSnippet();
    }

    private void OnOverworldNpcEvent(object sender, EvaluateEventArgs<(bool, OverworldNpc)> e)
    {
        HandleNpcRanges(e.Evaluate.Item2, e.Evaluate.Item1);
    }

    public static void HandleNpcRanges(OverworldNpc npc, bool add)
    {
        withinRange.HandleInstanceObjectInList(npc, add);
        if (withinRange.CollectionIsNotNullOrEmpty())
        {
            withinRange.Sort(overworldDialogueUi.Compare);
            OverworldNpc previous = withinRange[0];

            if (!IsActive(previous))
            {
                SetActiveNpc(previous);
                SetSnippetText(previous);
                MoveSnippetIntoPosition(true);
            }

        }
        else
        {
            SetActiveNpc();
            MoveSnippetIntoPosition(false);
        }
    }

    static void InitializeDialogueSnippet()
    {
        if(activeSnippet != null)
        {
            Destroy(activeSnippet.gameObject);
        }

        activeSnippet = GetDialogueSnippet();
        activeSnippet.SetLocalPosition(defaultStartPosition);
    }

    static void SetSnippetText(OverworldNpc npc)
    {
        if(activeSnippet != null)
        {
            activeSnippet.SetMessage(npc.GetDialogueToDisplay());
        }
    }

    internal static void UpdateSnippetText()
    {
        if(activeOverworldNpc != null && activeSnippet != null)
        {
            activeOverworldNpc.UpdateOverworldDialogueTrackerForNpc();
            SetSnippetText(activeOverworldNpc);
        }
    }

    static void MoveSnippetIntoPosition(bool inView)
    {
        if(activeSnippet == null)
        {
            return;
        }

        if (inView)
        {
            activeSnippet.MoveTo(inViewPosition, 0.25f);
            activeSnippet.SetOriginalPosition(inViewPosition);
        } else
        {
            activeSnippet.MoveTo(defaultStartPosition, 0.15f);
            //activeSnippet.ForceCollapse();
        }

        activeSnippet.MarkInView(inView);
        

    }

    static OverworldDialogueSnippet GetDialogueSnippet()
    {
        GameObject obj = Instantiate(overworldDialogueUi.dialogueSnippetPrefab, overworldDialogueUi.transform);
        return obj.GetComponent<OverworldDialogueSnippet>();
    }

     static bool IsActive(OverworldNpc npc)
    {
        if (activeOverworldNpc == null)
        {
            return false;
        }

        return npc.Equals(activeOverworldNpc);
    }

     static void SetActiveNpc(OverworldNpc npc = null)
    {
        if(activeOverworldNpc == null || npc == null)
        {
            activeOverworldNpc = npc;
        }
    }

    private void OnDestroy()
    {
        OverworldNpc.OverworldNpcEvent -= OnOverworldNpcEvent;

    }

    public int Compare(OverworldNpc x, OverworldNpc y)
    {
        return OverworldNpc.CloserNpc(x, y);
    }
}
