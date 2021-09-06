using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccupiablePositionContainer
{
    private readonly List<OccupiablePosition> occupiablePositions;

    private readonly List<Vector3> positionVectors;

    private OccupiablePosition hasNextResult;

    public List<IOccupyPositions> Occupiers { get; private set; }

    public OccupiablePositionContainer(params Vector3[] positions)
    {
        positionVectors = new List<Vector3>(positions);
        occupiablePositions = this.SetPlacementPositions(positions);
        Occupiers = new List<IOccupyPositions>();
    }

    public OccupiablePositionContainer(params Vector2[] positions)
    {
        positionVectors = new List<Vector3>();

        foreach(Vector2 v in positions)
        {
            positionVectors.Add(v);
        }

        occupiablePositions =  this.SetPlacementPositions(positions);
        Occupiers = new List<IOccupyPositions>();
    }

    public void ReleaseAll(MonoBehaviour coroutineStarter, float delayTime)
    {
        coroutineStarter.StartCoroutine(ReleaseAllOccupied(delayTime));
    }

    public void Print()
    {
        foreach (var o in occupiablePositions)
        {
            Debug.Log(o.ToString());
        }
    }

    public bool TryPlaceObjectInOccupiablePosition(IOccupyPositions occupier)
    {
        var nextPosition = this.GetNext();

        if (nextPosition != null)
        {
           nextPosition.Occupy(occupier);

            Occupiers.Add(occupier);

           return true;
        }

        return false;
    }

    public bool HasNext()
    {
        if(hasNextResult == null || !hasNextResult.Available)
        {
            var next = this.GetNextInternal();

            if(next == null)
            {
                return false;
            }

            hasNextResult = next;
        }

        return true;
    }

    private IEnumerator ReleaseAllOccupied(float waitFor)
    {
        yield return new WaitForSeconds(waitFor);

        foreach (var occupier in Occupiers)
        {
            if (this.TryGetOccupiablePosition(occupier, out OccupiablePosition pos))
            {
                OccupierReleaser.Release(occupier, pos, Object.Destroy);
            }
        }

        Occupiers.Clear();
    }

    public OccupiablePosition GetNext()
    {
        return this.HasNext() ? hasNextResult : null;
    }

    private OccupiablePosition GetNextInternal()
    {
        if (occupiablePositions.ValidList())
        {
            foreach (var pos in occupiablePositions)
            {
                if (pos.Available)
                {
                    return pos;
                }
            }
        }

        return null;
    }

    private bool TryGetOccupiablePosition(IOccupyPositions occupier, out OccupiablePosition position)
    {
        position = occupier.Occupied;

        return position != null;
    }

    private List<OccupiablePosition> SetPlacementPositions(params Vector3[] vects)
    {
        var occupiablePositions = new List<OccupiablePosition>();

        if(vects != null)
        {
            for (int i = 0; i < vects.Length; i++)
            {
                var position = new OccupiablePosition(this, vects[i], Vector3.zero, i);
                occupiablePositions.Add(position);
            }
        }

        return occupiablePositions;
    }

    private List<OccupiablePosition> SetPlacementPositions(params Vector2[] vects)
    {
        var occupiablePositions = new List<OccupiablePosition>();

        if (vects != null)
        {
            for (int i = 0; i < vects.Length; i++)
            {
                var position = new OccupiablePosition(this, vects[i], Vector3.zero, i);
                occupiablePositions.Add(position);
            }
        }
        return occupiablePositions;
    }
}
