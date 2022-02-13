using System;
using UnityEngine;

public class OccupiablePosition : IOccupiablePosition
{
    private OccupiablePositionContainer positionContainer;

    private IOccupyPositions currentOccupant;

    private Vector3 position;

    private Vector3 eulers;

    private readonly int positionIndex;

    public bool Available => currentOccupant == null;

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
        return currentOccupant != null && currentOccupant == occupier;
    }

    public void Occupy(IOccupyPositions occupier)
    {
        occupier.OccupierTransform.localPosition = position + occupier.PositionOffset;

        occupier.OccupierTransform.rotation = Quaternion.Euler(eulers);

        currentOccupant = occupier;

        occupier.ReleaseOccupier += OnReleaseOccupierSetThatShitFree;
    }

    public void ReleasePosition(Action<GameObject> destructionMethod = null)
    {
        this.ReleaseOccupier(currentOccupant, destructionMethod);
    }

    private void ReleaseOccupier(IOccupyPositions occupier, Action<GameObject> destructionMethod = null) 
    {
        if (this.IsOccupiedBy(occupier))
        {
            currentOccupant = null;
        }

        if (destructionMethod != null)
        {
            destructionMethod(occupier.OccupierTransform.gameObject);
        } 
        else
        {
            occupier.ReleaseOccupier -= OnReleaseOccupierSetThatShitFree;
        }
    }

    private void OnReleaseOccupierSetThatShitFree(object source, ReleaseOccupierEventArgs args)
    {
        if (this.IsOccupiedBy(args.Occupier))
        {
            if (args.DestroyOnRelease)
            {
                this.ReleasePosition(GameObject.Destroy);
            } else
            {
                this.ReleasePosition();
            }
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
    Vector3 PositionOffset { get; }

    event EventHandler<ReleaseOccupierEventArgs> ReleaseOccupier;
}

public interface IOccupiablePosition
{
    bool IsOccupiedBy(IOccupyPositions occupier);

    void ReleasePosition(Action<GameObject> destructionMethod = null);
}

public class ReleaseOccupierEventArgs : EventArgs
{
    public IOccupyPositions Occupier { get; set; }

    public bool DestroyOnRelease { get; set; }
}
