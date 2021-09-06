using System;
using UnityEngine;

public class OverworldNpc : MonoBehaviour, ITrackDistanceToPlayer
{
    public int npcIndex;

    public string npcName;

    protected static NpcTextureCache npcTextureCache;

    public static event EventHandler<EvaluateEventArgs<(bool, OverworldNpc)>> OverworldNpcEvent; 

    public DialogueResponse[] testResponses;

    public OverworldDialogueTracker tracker;

    bool interactable;
    public bool CanInteractWith {
        get => interactable;
        private set { 
            if(value != interactable)
            {
                OnRangeChange(value);
            }
            interactable = value;
        } 
    }

    public RangeChangeTracker DistanceTracker { get; set; }

    protected MeshRenderer Renderer => GetComponent<MeshRenderer>();

    protected MaterialPropertyBlock propertyBlock;

    protected MaterialPropertyBlock GetPropertyBlock
    {
        get
        {
            if (propertyBlock == null)
            {
                propertyBlock = new MaterialPropertyBlock();
            }

            return propertyBlock;
        }
    }

    protected static readonly int npcTexture = Shader.PropertyToID("_MainTex");

    void Awake()
    {
        if(npcTextureCache == null)
        {
            npcTextureCache = new NpcTextureCache();
        }
    }

    private void Start()
    {
        InitializeNpc();
    }

    private void Update()
    {
        CanInteractWith = DistanceTracker.PlayerInRange;
    }

    private void InitializeNpc()
    {
        this.SetNpcDataFromDatabaseEntry();
        this.GetNpcDialogue();
        DistanceTracker = new RangeChangeTracker(transform, 2f);
    }

    private void SetNpcDataFromDatabaseEntry()
    {
        NpcDatabaseEntry npc = NpcData.GetDatabaseEntry(npcIndex);
        npcName = npc.NpcName;
    }

    private void GetNpcDialogue()
    {
        tracker.GetDialogueFromLevelData(npcIndex);
    }

    public string GetDialogueToDisplay()
    {
        string[] strs = testResponses[0].displayDialogue; //tracker.GetDisplayDialogue();
        return $"{npcName} says: \n {string.Join(" ", strs)}";
    }

    public void UpdateOverworldDialogueTrackerForNpc()
    {
        tracker.Advance();
    }

    public static int CloserNpc(OverworldNpc a, OverworldNpc b)
    {
        if(a == null && b == null)
        {
            return 0;
        }

        if(a == null && b != null)
        {
            return 1;
        }

        if(a != null && b == null)
        {
            return -1;
        }

        return Mathf.Abs(a.DistanceTracker.DistanceFromPlayer) <= Mathf.Abs(b.DistanceTracker.DistanceFromPlayer) ? -1 : 1;
    }

    private void OnRangeChange(bool withinRange)
    {
        OverworldNpcEvent.BroadcastEvent(this, new EvaluateEventArgs<(bool, OverworldNpc)> 
        { 
            Evaluate = (withinRange, this) 
        }
        );
        Debug.Log("Within Range: " + withinRange);
    }

    protected void SetNpcTexture(Texture2D texture)
    {
        if(texture == null)
        {
            Debug.Log("Texture is null");
            return;
        }

        GetPropertyBlock.SetTexture(npcTexture, texture);
        Renderer.SetPropertyBlock(GetPropertyBlock);
    }
}

public interface ITrackDistanceToPlayer
{
    RangeChangeTracker DistanceTracker { get; }
}

