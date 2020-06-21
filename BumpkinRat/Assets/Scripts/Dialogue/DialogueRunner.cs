using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class DialogueRunner : MonoBehaviour
{
    private static DialogueRunner dr;
    public TextMeshPro textmeshDisplay;
    public Dialogue activeDialogue;

    string displayLine;

    private void OnEnable()
    {
        if(dr == null) { dr = this; } else { Destroy(gameObject); }
        NpcBehavior.NpcDialogueTriggered += OnNpcDialogueTriggered;
    }

    private void Update()
    {
        textmeshDisplay.text = reading ? displayLine : "";

        //test reading dialogue nodes here
        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Read a sample tree of dialogue");
            DialogueNode node = new DialogueNode(new string[] { "args.sampleDialogue", "line 2 now" }, 1);
            DialogueNode node2 = new DialogueNode(new string[] { "Node 2"});
            DialogueTree testTree = new DialogueTree(new DialogueNode[] { node, node2});
            StartCoroutine(RunNode(testTree, testTree.GetNode(0)));
        }
        
    }

    void OnNpcDialogueTriggered(object source, DialogueTriggerEventArgs args)
    {
        if(textmeshDisplay != null)
        {
            //StartCoroutine(RunLine(args.sampleDialogue));
            DialogueNode node = new DialogueNode(new string[] { args.sampleDialogue, args.sampleDialogue });
            StartCoroutine(RunNode(node));
            textmeshDisplay.transform.position = args.activatedNPC.transform.position + Vector3.up;
        }
    }

    bool writingLine;
    public static bool reading { get; private set; }

    IEnumerator RunNode(DialogueTree t, DialogueNode n)
    {
        reading = true;
        yield return StartCoroutine(RunNode(n));

        if(n.pointer > 0)
        {
            Debug.Log(t.ValidNode(n.pointer));
            if (t.ValidNode(n.pointer))
            {
                StartCoroutine(RunNode(t, t.GetNode(n.pointer)));
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
        string l = line;
        while (l.Length > 0)
        {
            char c = l.First();
            displayLine += c;
            l = l.TrimStart(c);
            yield return new WaitForSeconds(0.01f);
        }
        writingLine = false;
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
