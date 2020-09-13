using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Dialogue
{
    [SerializeField] string active_tree_id;
    public string ActiveTreeID
    {
        get => active_tree_id;
        set => active_tree_id = value;
    }

    Dictionary<string, DialogueTree> dialogue_lookup;
    bool validId => dialogue_lookup.ContainsKey(active_tree_id);

    public DialogueTree activeTree => validId ? dialogue_lookup[active_tree_id] : null;
    
    public Dialogue() { }

    public Dialogue(string path) {
        SubscribeToEvents();
        TreeLoader trees = path.InitializeFromJSON<TreeLoader>();
        dialogue_lookup = trees.Trees.ToDictionary(k => k.treeID);
        Debug.LogFormat("Created Dialogue Lookup with {0} entries", dialogue_lookup.Count);
    }

    public DialogueTree GetDialogueTree(string id)
    {
        try
        {
            return dialogue_lookup[id];
        }
        catch (KeyNotFoundException){
            Debug.LogWarning("Error getting dialogue tree by ID");
            return new DialogueTree();
        }
    }

    public DialogueTree GetDialogueTree(int index)
    {
        try
        {
            return dialogue_lookup.ElementAt(index).Value;
        }catch (ArgumentOutOfRangeException)
        {
            Debug.LogWarning("Error getting dialogue tree by index");
            return new DialogueTree();
        }
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
    public string treeID;
    public List<DialogueNode> nodesInTree;
    public bool validTree => nodesInTree != null && nodesInTree.Count > 0;
    public int startIndex { get; set; }
    bool initialized;
    public TreeSpecs treeSpecs;

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

    public void SetSpecification(TreeSpecs specs)
    {
        initialized = false;
        treeSpecs = specs;
        InitializeDialogueTree();
    }

    public void InitializeDialogueTree()
    {
        if (treeSpecs.treeConditions.ValidArray() && !initialized)
        {
            initialized = true;
            for(int i = 0; i < treeSpecs.treeConditions.Length; i++)
            {
                string sep = treeSpecs.treeConditions[i].GetIndicationInfo().Item1;
                string[] arr = sep.Split(':');
                this.SetValue(arr[0], arr[1]);
            }
        }
    }

    public DialogueNode startNode
    {
        get => GetNode(startIndex);
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

[Serializable]
public struct TreeSpecs
{
    public string[] treeConditions;
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

[Serializable]
public struct TreeLoader
{
    public List<DialogueTree> Trees;
}