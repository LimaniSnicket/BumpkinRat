﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

public class DialogueRunner : MonoBehaviour
{
    private static DialogueRunner dr;
    public Dialogue activeDialogue;

    public GameObject dialogueMenuObject;
    static DialogueMenu dialogueMenu;

    string displayLine;
    public static bool validCondition { get; set; }
    public static int validationTally { get; private set; }

    public static Dictionary<string, object> validation_lookup;


    public static event EventHandler<IndicatorArgs> DialogueEventIndicated;

    private void Awake()
    {
        if(dr == null) { dr = this; } else { Destroy(gameObject); }
        if (dialogueMenuObject != null) { dialogueMenu = new DialogueMenu(dialogueMenuObject); }
        if (validation_lookup == null)
        {
            validation_lookup = FindObjectsOfType<GameObject>().Where(g => g is IValidateable).
                ToDictionary(p => ((object)p).ToString(), p => (object)p);
        }

        NpcBehavior.NpcDialogueTriggered += OnNpcDialogueTriggered;
        
    }

    private void Update()
    {
        if (reading)
        {
            dialogueMenu.UpdateDialogueDisplay(displayLine);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            string toSlice = "<set>PLAYER_NAME:Billy</set> Hey Billy.";
            (string, string) sliced = toSlice.SliceIndicator();
            //sliced = ("<set>Rat Man (PlayerBehavior):playerData:playerName:Billy</set>", "Hey Billy.")

            (string, Indication) info = sliced.Item1.GetIndicationInfo();
            //info = ("Rat Man (PlayerBehavior):playerData:playerName:Billy", Indication.Setter)

            BroadcastDialogueIndicatorEvent(info);

            string ch = "Npc+NPC_TREE+PLAYER_NAME:2";
            string[] charr = ch.FormatMacros(':', '+');
            Debug.Log(charr.Length);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DialogueNode[] nodes = { new DialogueNode(new string[] {
                "<set>Rat Man (PlayerBehavior):playerData.playerName:Billy</set> Hey Billly.",
                "<cond>Rat Man (PlayerBehavior):playerData.playerName:Billy</cond>Conditional Statement" }, 1),
                new DialogueNode(new string[] { "<b>Entering dialogue node 2.</b>",
                    "This one has an option pointer!" }, 2, new NodeSpecs{ waitForPlayerInput = true}),
                new DialogueNode(new string[]{ "Final Node."})};
            DialogueTree testTree = new DialogueTree(nodes);
            dialogueMenu.LoadMenu();
            StartCoroutine(RunTree(testTree, testTree.GetNode(0)));
        }
    }

    void OnNpcDialogueTriggered(object source, DialogueTriggerEventArgs args)
    {
        if (dialogueMenu != null)
        {
            dialogueMenu.LoadMenu();
            StartCoroutine(RunTree(args.triggeredTree, args.triggeredTree.startNode));
        }
    }

    bool writingLine;
    public static bool reading { get; private set; }

    public int DialogueChoiceInput (){
        if (Input.GetKey(KeyCode.Alpha1)) { return 0; }
        if (Input.GetKey(KeyCode.Alpha2)) { return 1; }
        if (Input.GetKey(KeyCode.Alpha3)) { return 2; }
        return -1;
    }

    IEnumerator RunTree(DialogueTree t, DialogueNode n)
    {
        reading = true;
        yield return StartCoroutine(RunNode(n));
      
        if (n.specs.waitForPlayerInput) {
            int choice = -1;
            while (choice < 0)
            {
                choice = DialogueChoiceInput();
                yield return new WaitForEndOfFrame();
            }

            n.ChangePointer(choice);
        }

        if(n.pointer > -1)
        {
            if (t.ValidNode(n.pointer))
            {
                StartCoroutine(RunTree(t, t.GetNode(n.pointer)));
            }
        } else
        {
            Debug.Log("Exit Dialogue Tree");
            dialogueMenu.CloseMenu();
            reading = false;
        }
    }

    IEnumerator RunNode(DialogueNode n)
    {
        int atLine = 0; 
        bool waitForLine = false;
        while (atLine < n.numberOfLines)
        {
            if (!waitForLine)
            {
                waitForLine = true;
                StartCoroutine(RunLine(n.lines[atLine]));
            }
            if (InputX.interactionButton)
            {
                if (!writingLine)
                {
                    atLine++;
                    waitForLine = false;
                }
                else
                {
                    StopCoroutine("RunLine");
                    displayLine = n.lines[atLine];
                    writingLine = false;
                }
            }
            yield return null;
        }
    }

    IEnumerator RunLine(string line)
    {
        displayLine = "";
        writingLine = true;
        string formatter = "";
        (string, string) indications = ("", "");
        string l = line;
        while (l.Length > 0)
        {
            char c = l.ElementAt(0);
            if (c == '<')
            {
                if (l.isIndicator())
                {
                    indications = l.SliceIndicator();
                    (string, Indication) stripped = indications.Item1.GetIndicationInfo();
                    if (stripped.Item2.BroadcastableIndicator())
                    {
                        BroadcastDialogueIndicatorEvent(stripped);
                    }
                    if (stripped.Item2.Equals(Indication.Condition))
                    {
                        Debug.Log("Validating Condition");
                        string[] parse = stripped.Item1.FormatMacros(':', '+');
                        
                    }

                    stripped.DebugTuple();

                    l = indications.Item2;
                }
                else
                {
                    formatter = l.GetIndicator(true);
                    l = l.Remove(0, formatter.Length);
                    displayLine += formatter;
                }
            } else
            {
                displayLine += c;
                l = l.TrimStart(c);
            }
            if (c != ' ')
            {
                yield return new WaitForSeconds(0.01f);
            }
        }
        writingLine = false;
    }

    void BroadcastDialogueIndicatorEvent(string slicedIndicator)
    {
        (string, Indication) relevantInfo = slicedIndicator.GetIndicationInfo();
        relevantInfo.DebugTuple();
        if (DialogueEventIndicated != null)
        {
            DialogueEventIndicated(this, new IndicatorArgs { indicatorType = relevantInfo.Item2, InfoToParse = relevantInfo.Item1 }) ;
        }
    }

    void BroadcastDialogueIndicatorEvent((string, Indication) tuple)
    {
        if (DialogueEventIndicated != null)
        {
            DialogueEventIndicated(this, new IndicatorArgs { indicatorType = tuple.Item2,
                InfoToParse = tuple.Item1 });
        }
    }

    public void GenerateDialogue(TextAsset txt)
    {
        dr.activeDialogue = new Dialogue();
    }

    public static void RegisterObjectsForEvaluation(Action<object, IndicatorArgs> callback)
    {

    }

    private void OnDisable()
    {
        NpcBehavior.NpcDialogueTriggered -= OnNpcDialogueTriggered;
    }
}

public class DialogueTriggerEventArgs : EventArgs
{
    public string sampleDialogue;
    public DialogueTree triggeredTree;
    public NpcBehavior activatedNPC;
}

public interface IValidateable
{
    bool Validate(string thisObj, string[] compare);
}
