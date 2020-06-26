using System;
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
    public TextMeshPro textmeshDisplay;
    public Dialogue activeDialogue;

    string displayLine;

    public static event EventHandler<IndicatorArgs> DialogueEventIndicated;

    private void OnEnable()
    {
        if(dr == null) { dr = this; } else { Destroy(gameObject); }
        NpcBehavior.NpcDialogueTriggered += OnNpcDialogueTriggered;
        
    }

    private void Update()
    {
        textmeshDisplay.text = reading ? displayLine : "";

        if (Input.GetKeyDown(KeyCode.L))
        {
            string toSlice = "<tone>Tone|Pointer</tone> blah blah blah";
            (string, string) sliced = toSlice.SliceIndicator();
            (string, Indication) info = sliced.Item1.GetIndicationInfo();
            info.DebugTuple();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            DialogueNode[] nodes = { new DialogueNode(new string[] { "<tone>Tone|Pointer</tone>fuck line 1", "fuck Line 2" }, 1),
                new DialogueNode(new string[] { "<b>Entering dialogue node 2.</b>", "This one has an option pointer!" }, 2, new NodeSpecs{ waitForPlayerInput = true}),
                new DialogueNode(new string[]{ "Final Node."})};
            DialogueTree testTree = new DialogueTree(nodes);
            StartCoroutine(RunTree(testTree, testTree.GetNode(0)));
        }
    }

    void OnNpcDialogueTriggered(object source, DialogueTriggerEventArgs args)
    {
        if (textmeshDisplay != null)
        {
            DialogueNode[] nodes = { new DialogueNode(new string[] { "fuck line 1", "fuck Line 2" }, 1),
                new DialogueNode(new string[] { "<b>Entering dialogue node 2.</b>" }) };
            DialogueTree testTree = new DialogueTree(nodes);
            StartCoroutine(RunTree(testTree, testTree.GetNode(0)));
            textmeshDisplay.transform.position = args.activatedNPC.transform.position + Vector3.up;
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
                    BroadcastDialogueIndicatorEvent(indications.Item1);
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
            DialogueEventIndicated(this, new IndicatorArgs { indicatorType = relevantInfo.Item2, infoToParse = relevantInfo.Item1 }) ;
        }
    }

    public void GenerateDialogue(TextAsset txt)
    {
        dr.activeDialogue = new Dialogue();
    }

    private void OnDisable()
    {
        NpcBehavior.NpcDialogueTriggered -= OnNpcDialogueTriggered;
    }
}

public class DialogueTriggerEventArgs : EventArgs
{
    public string sampleDialogue;
    public NpcBehavior activatedNPC;
}
