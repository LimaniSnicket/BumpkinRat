using System;
using UnityEngine;

public class OccupiablePosition : IOccupiablePosition
{
    private OccupiablePositionContainer positionContainer;

    private IOccupyPositions currentOccupant;

    private bool isOccupied;

    private Vector3 position;

    private Vector3 eulers;

    private readonly int positionIndex;

    public bool Available => !isOccupied;

    public int PositionIndex => positionIndex;

    public OccupiablePosition(Vector3 position, Vector3 eulers, int positionIndex)
    {
        this.position = position;

        this.eulers = eulers;

        this.positionIndex = positionIndex;
    }

    public OccupiablePosition(OccupiablePositionContainer container, Vector2 position, Vector3 eulers, int positionIndex) : this((Vector3)position, eulers, positionIndex) 
    {
        this.positionContainer = container;
    }

    public bool IsOccupiedBy(IOccupyPositions occupier)
    {
        return occupier.Occupied != null && occupier.Occupied == this;
    }

    public void Occupy(IOccupyPositions occupier)
    {
        isOccupied = true;

        occupier.OccupierTransform.localPosition = position + occupier.PositionOffset;

        occupier.OccupierTransform.rotation = Quaternion.Euler(eulers);

        currentOccupant = occupier;

        occupier.Occupied = this;
    }

    public void ReleasePosition()
    {
        isOccupied = false;
    }

    internal void ReleaseOccupier(IOccupyPositions occupier, Action<GameObject> destructionMethod = null) 
    {
        if (this.IsOccupiedBy(occupier))
        {
            isOccupied = false;

            occupier.Occupied = null;
        }

        if(destructionMethod != null)
        {
            destructionMethod(occupier.OccupierTransform.gameObject);
        }
    }

    public override string ToString()
    {
        return $"Positon at {position}. Index {positionIndex}. Available: {Available}";
    }
}

public interface IOccupyPositions 
{
    Transform OccupierTransform { get; }
    // OccupiablePosition Occupied { get; set; }
    Vector3 PositionOffset { get; }
    void ForceDestroy();
}

public interface IOccupiablePosition
{
    bool IsOccupiedBy(IOccupyPositions occupier);

    void ReleasePosition();
}

public struct OccupierReleaser
{
    public static void Release(IOccupyPositions occupier, IOccupiablePosition position, Action<GameObject> destructionMethod = null)
    {
        if (occupier.Occupied != null && occupier.Occupied == position)
        {
            occupier.Occupied = null;
            position.ReleasePosition();
        }

        if (destructionMethod != null)
        {
            destructionMethod(occupier.OccupierTransform.gameObject);
        }
    }
}