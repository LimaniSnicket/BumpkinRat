using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public DialogueTree currentTree;
    [SerializeField] DialogueNode activeNode;
    public bool dialogueActive { get => activeNode != null; }
    public bool onLastDialogueNode => dialogueActive && activeNode != null && activeNode.pointer == -1;

    public string dialogueLine { get; private set; } public bool writingLine { get; private set; }

    public Dialogue() { }
    public Dialogue(DialogueTree tree)
    {
        currentTree = tree;
        activeNode = new DialogueNode();
    }

    public IEnumerator RunDialogue(MonoBehaviour host, int nPointer)
    {
        Debug.Log(nPointer);
        if (nPointer < 0) { Debug.Log("Dialogue Complete. Exiting..."); }
        else
        {
            if (currentTree == null || !currentTree.validTree) { yield return null; }
            activeNode = currentTree.GetNode(nPointer);
            yield return host.StartCoroutine(RunLine(host, activeNode.lines, 0));
            while (writingLine)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return host.StartCoroutine(RunDialogue(host, activeNode.pointer));
        }
    }

    IEnumerator RunLine(MonoBehaviour host, string[] lines, int index)
    {
        dialogueLine = ""; writingLine = true;
        if (index < 0 || lines == null || index >= lines.Length || lines.Length <= 0)
        {
            writingLine = false; yield return null;
        }
        else
        {

            string line = lines[index];
            while (line.Length > 0)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    dialogueLine = lines[index];
                    break;
                }

                char c = line.First();
                dialogueLine += c;
                line.Remove(0);
                yield return new WaitForSeconds(0.001f);
            }
            while (!Input.GetKeyDown(KeyCode.Space)) { yield return new WaitForEndOfFrame(); }
            int increment = index++;
            yield return host.StartCoroutine(RunLine(host, lines, increment));
        }
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
    public DialogueNode() { }

    public DialogueNode(string[] l)
    {
        lines = new string[l.Length];
        Array.Copy(l, lines, lines.Length);
    }
    public DialogueNode(string[] l, int p)
    {
        lines = new string[l.Length];
        Array.Copy(l, lines, lines.Length);
        pointer = p;
    }    
}