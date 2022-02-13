using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBehavior : MonoBehaviour
{
    public NpcMood npcMood;
    public NpcDialogueTest npcDialogue;
    public string dialoguePath;
    public Dialogue dialogueStorage { get; set; }
    MaterialPropertyBlock propBlock;
    public MaterialPropertyBlock getPropBlock
    {
        get
        {
            if (propBlock == null) { propBlock = new MaterialPropertyBlock(); }
             return propBlock;
        }
    }

    static readonly int colorProperty = Shader.PropertyToID("_Color");
    MeshRenderer meshR => GetComponent<MeshRenderer>();

    public static event EventHandler<DialogueTriggerEventArgs> NpcDialogueTriggered;

    private void Start()
    {
        npcMood = new NpcMood();
        npcDialogue = new NpcDialogueTest();
        dialogueStorage = new Dialogue(dialoguePath);
        dialogueStorage.ActiveTreeID = "tree_1";
        dialogueStorage.activeTree.InitializeDialogueTree();
        DialogueRunner.DialogueEventIndicated += OnDialogueIndicatorEvent;
    }

    private void Update()
    {
        Ray r = new Ray(transform.position, transform.forward);
        Debug.DrawRay(r.origin, r.direction, Color.green);
        getPropBlock.SetColor(colorProperty, npcMood.opinionColor);
        meshR.SetPropertyBlock(getPropBlock);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MovementController>())
        {
            float impactVelocity = collision.relativeVelocity.magnitude;
            npcMood.AdjustFavor(impactVelocity * -0.25f);
            BroadcastNpcDialogue();
        }
    }

    void OnDialogueIndicatorEvent(object source, IndicatorArgs args)
    {
        Debug.Log(args.SeperatedInfo.Length);
    }

    void BroadcastNpcDialogue()
    {
        if (NpcDialogueTriggered != null)
        {
            NpcDialogueTriggered(this, new DialogueTriggerEventArgs { sampleDialogue = npcDialogue.defaultLine,
                triggeredTree = npcDialogue.nextTree,
                activatedNPC = this});
        }
    }

    private void OnDisable()
    {
        DialogueRunner.DialogueEventIndicated -= OnDialogueIndicatorEvent;
        dialogueStorage.UnsubscribeToEvents();
    }
}

[Serializable]
public class NpcDialogueTest
{
    public bool readyForDialogue;
    public string defaultLine = "Hey, do you need something?";
    public string annoyedLine = "Hey, watch where you're going!";
    public DialogueTree nextTree;

    public NpcDialogueTest()
    {
        InstantiateInitialDialogue();
    }

    public void InstantiateInitialDialogue()
    {
        DialogueNode[] nodes = { new DialogueNode(new string[] { defaultLine }),
            new DialogueNode(new string[]{ annoyedLine }) };
        nextTree = new DialogueTree(nodes);
        nextTree.SetSpecification(new TreeSpecs { treeConditions = new string[] { "<set>startIndex:1</set>" } });
    }
}

[Serializable]
public class NpcMood
{
    Dictionary<float, Color> favorColors = new Dictionary<float, Color> { { 0, Color.red }, { 5, Color.white }, { 10, Color.cyan } };
    [Range(0, 10)] public float opinionOfPlayer = 5;
    public void AdjustFavor(float value) { opinionOfPlayer += value; }
    public Color opinionColor => opinionOfPlayer.ColorByRange(favorColors);
}
