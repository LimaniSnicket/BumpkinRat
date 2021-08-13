using UnityEngine;

public class OccupiablePosition
{
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

    public OccupiablePosition(Vector2 position, Vector3 eulers, int positionIndex) : this((Vector3)position, eulers, positionIndex) { }

    public void Occupy<T>(IOccupyPositions<T> occupier) where T: Transform
    {
        isOccupied = true;

        occupier.OccupierTransform.position = position + occupier.PositionOffset;

        occupier.OccupierTransform.rotation = Quaternion.Euler(eulers);

        occupier.Occupied = this;
    }

    public static void Release<T>(IOccupyPositions<T> occupying) where T: Transform
    {
        if(occupying.Occupied != null)
        {
            occupying.Occupied.isOccupied = false;
        }
    }
}

public interface IOccupyPositions<T> where T : Transform
{
    T OccupierTransform { get; }
    OccupiablePosition Occupied { get; set; }
    Vector3 PositionOffset { get; }
}