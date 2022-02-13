using System;
using UnityEngine;
using DG.Tweening;

public class ItemObjectWorldElement : ItemObjectBehaviour, IOccupyPositions
{  
    public bool MouseHoveringOnItemObject { get; private set; }

    public Transform OccupierTransform => transform;

    public Vector3 PositionOffset { get; private set; }

    public event EventHandler<ReleaseOccupierEventArgs> ReleaseOccupier;

    public string description;

    public WorldDragoutSnippet dragoutSnippet;

    private void Start()
    {
        dragoutSnippet = GetComponentInChildren<WorldDragoutSnippet>();
        dragoutSnippet.SetDescription(description);

        PositionOffset = Vector3.zero; //GetComponent<MeshFilter>().mesh.bounds.extents.y * Vector3.up;
    }

    private void Update()
    {
        if(FocusAreaHandler.NumberOfInFocusAreas() > 0)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Put item back into inventory!");
                this.BroadcastPlaceBack();

                this.BroadcastRelease(destroyOnRelease: true);

                // TODO: Broadcast Release

                // Occupied.ReleaseOccupier(this);
                // Destroy(gameObject);
            }
        }
    }
    private void OnMouseEnter()
    {
        Debug.Log("Hover Over Item Object");

        gameObject.BroadcastMessage("OnItemObjectFocusChange", true);

        if (!ItemCrafter.CraftingSequenceActive)
        {
            return;
        }

        MouseHoveringOnItemObject = true;
    }

    private void OnMouseExit()
    {
        if (FocusAreaHandler.NumberOfInFocusAreas() <= 0)
        {
            gameObject.BroadcastMessage("OnItemObjectFocusChange", false);

            MouseHoveringOnItemObject = false;
        }
    }

    public override void ForceDestroy()
    {
        this.BroadcastRelease(true);
    }

    private void BroadcastRelease(bool destroyOnRelease)
    {
        var args = new ReleaseOccupierEventArgs
        {
            DestroyOnRelease = destroyOnRelease,
            Occupier = this
        };

        this.ReleaseOccupier.BroadcastEvent(this, args);
    }

    bool rotating = false;
    private void RotateViaMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Rotate(Vector3.up * -1);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Rotate(Vector3.up);
        }

        if(Input.mouseScrollDelta != Vector2.zero)
        {
            Rotate(Vector3.right * Input.mouseScrollDelta);
        }
    }

    void Rotate(Vector3 direction)
    {
        if (rotating)
        {
            return;
        }

        transform.DOBlendableLocalRotateBy(direction * 45, .3f);
    }

    internal void BroadcastInteractionWithFocusArea(FocusAreaObject focus)
    {
        ItemCrafter.BeginCraftingSequence(focus, this);
        this.BroadcastInteractedWith(focus);
    }

/*    public override void ForceDestroy()
    {
        this.ForceDestroy(this);
    }*/
}

public class ItemObjectEventArgs: EventArgs
{
    public bool PutBack { get; set; }
    public ItemObjectBehaviour InteractedWith { get; set; }
    public IFocusArea AtFocusArea { get; set; }

}




