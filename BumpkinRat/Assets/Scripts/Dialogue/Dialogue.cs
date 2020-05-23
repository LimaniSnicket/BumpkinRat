using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue
{
    public DialogueTree currentTree;
    [SerializeField] DialogueNode activeNode;
    public bool dialogueActive { get => activeNode != null; }
    public bool onLastDialogueNode => dialogueActive && activeNode != null && activeNode.pointer == -1;

    public void RunDialogueTree(MonoBehaviour host)
    {
        
    }

    IEnumerator ReadDialogue(MonoBehaviour host, int node)
    {
        if(currentTree == null && !currentTree.validTree) { yield return null; }
        activeNode = currentTree.GetNode(node); int l = activeNode.numberOfLines;
        int t = 0; 
        while (t < l)
        {
            yield return host.StartCoroutine(activeNode.RunDialogueLine(host, activeNode.lines, t));
            t++;
        }
        if (onLastDialogueNode)
        {
            Debug.Log("Exiting Dialogue");
            activeNode = null;
        } else
        {
            host.StartCoroutine(ReadDialogue(host, activeNode.pointer));
        }
    }
}

[Serializable]
public class DialogueTree
{
    public string treeTitle;
    public List<DialogueNode> nodesInTree;

    public bool validTree => nodesInTree != null && nodesInTree.Count > 0;

    public DialogueNode GetNode(int i)
    {
        if (!ValidNode(i)) { return new DialogueNode(); }
        return nodesInTree[i];
    }

    public int NodePointer(int node)
    {
        if(nodesInTree == null || nodesInTree.Count <= 0) { return -1; }
        if(node >= nodesInTree.Count) { return nodesInTree[nodesInTree.Count - 1].pointer; }
        return nodesInTree[node].pointer;
    }

    public bool ValidNode(int node)
    {
        int next = NodePointer(node);
        return next > -1;
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
    public string displayLine { get; private set; }
    public DialogueNode() { }
    public DialogueNode(string[] l)
    {
        lines = l;
    }
    public DialogueNode(string[] l, int p)
    {
        lines = l;
        pointer = p;
    }

    public bool writingLine { get; private set; }
    public IEnumerator RunDialogueLine(MonoBehaviour host, string[] arr, int index)
    {
        displayLine = ""; writingLine = true;
        if(index < 0 || arr == null || index >= arr.Length || arr.Length <= 0)
        {
            writingLine = false;
            yield return null;
        }
        while (writingLine)
        {
           for(int i = 0; i< arr[index].Length + 1; i++)
            {
                if(i >= arr.Length) { writingLine = false; break; } else
                {
                    displayLine.Insert(i, arr[i]);
                    if (arr[i] != " ") { yield return new WaitForSeconds(0.005f); }
                }
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                writingLine = false;
                break;
            }
            yield return null;
        }
        int nIndex = index++;
        yield return host.StartCoroutine(RunDialogueLine(host, arr, nIndex));
    }
}