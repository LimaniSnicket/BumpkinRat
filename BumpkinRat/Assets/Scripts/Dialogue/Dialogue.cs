using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public DialogueTree currentTree;

    public Dialogue() {
        SubscribeToEvents();
    }
    public Dialogue(DialogueTree tree)
    {
        currentTree = tree; 
    }

    public void SubscribeToEvents()
    {
        
    }

    public void UnsubscribeToEvents()
    {
        
    }
}

[Serializable]
public class DialogueTree
{
    public List<DialogueNode> nodesInTree;
    public bool validTree => nodesInTree != null && nodesInTree.Count > 0;

    public DialogueTree() { }
    public DialogueTree(string[] lines)
    {
        nodesInTree = new List<DialogueNode>();
        nodesInTree.Add(new DialogueNode(lines));
    }

    public DialogueTree(DialogueNode[] nodes)
    {
        nodesInTree = new List<DialogueNode>(nodes);
    }

    public DialogueNode GetNode(int i)
    {
        if (!ValidNode(i)) { return new DialogueNode(); }
        return nodesInTree[i];
    }

    public bool ValidNode(int node)
    {
        if (!validTree) { return false; }
        return node > -1 && nodesInTree.Count > node;
    }

    public bool HasNext(int node)
    {
        return GetNode(node).pointer > -1;
    }
}

[Serializable]
public class DialogueNode
{
    public string[] lines;
    public int pointer = -1;
    public int numberOfLines { get {
            if(lines == null) { return -1; }
            return lines.Length;
        } }

    public NodeSpecs specs;
    public bool validChoices {
        get
        {
            if (specs == null) { specs = new NodeSpecs(); }
            return specs.waitForPlayerInput && specs.dialogueChoices.ValidList();
        }
    }

    public DialogueNode() { }

    public DialogueNode(string[] l)
    {
        lines = new string[l.Length];
        Array.Copy(l, lines, lines.Length);
        specs = new NodeSpecs();
    }
    public DialogueNode(string[] l, int p)
    {
        lines = new string[l.Length];
        Array.Copy(l, lines, lines.Length);
        pointer = p;
        specs = new NodeSpecs();
    }

    public DialogueNode(string[] l, int p, NodeSpecs s)
    { 
        lines = new string[l.Length];
        Array.Copy(l, lines, lines.Length);
        pointer = p;
        specs = s;
    }

    public void ChangePointer(int np)
    {
        pointer = np;
    }
}


/// <summary>
/// <para> NodeSpecs class has different options to tailor each dialogue node to specific uses. Not everything needs to be filled in. </para>
/// </summary>

[Serializable]
public class NodeSpecs
{
    public string nodeType;
    public bool waitForPlayerInput;
    public List<DialogueChoice> dialogueChoices;
    public NodeSpecs() { }
    public NodeSpecs(bool wait) { waitForPlayerInput = wait; }
}

[Serializable]
public class DialogueChoice
{
    public string choiceSnippet;
    public int pointer = -1; //default end dialogue
}