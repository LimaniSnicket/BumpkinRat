using System.Collections.Generic;
using UnityEngine;

public class OccupiablePositionContainer
{
    private readonly List<OccupiablePosition> occupiablePositions;

    private readonly List<Vector3> positionVectors;

    public OccupiablePositionContainer(params Vector3[] positions)
    {
        positionVectors = new List<Vector3>(positions);
        occupiablePositions = this.SetPlacementPositions(positions);
    }

    public OccupiablePositionContainer(params Vector2[] positions)
    {
        positionVectors = new List<Vector3>();

        foreach(Vector2 v in positions)
        {
            positionVectors.Add(v);
        }

        occupiablePositions =  this.SetPlacementPositions(positions);
    }

    public bool TryPlaceObjectInOccupiablePosition<T>(IOccupyPositions<T> occupier) where T: Transform
    {
        var nextPosition = this.GetNext();

        if(nextPosition != null)
        {
           nextPosition.Occupy(occupier);
           return true;
        }

        return false;
    }

    public OccupiablePosition GetNext()
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

    private List<OccupiablePosition> SetPlacementPositions(params Vector3[] vects)
    {
        var occupiablePositions = new List<OccupiablePosition>();

        if(vects != null)
        {
            for (int i = 0; i < vects.Length; i++)
            {
                var position = new OccupiablePosition(vects[i], Vector3.zero, i);
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
                var position = new OccupiablePosition(vects[i], Vector3.zero, i);
                occupiablePositions.Add(position);
            }
        }

        return occupiablePositions;
    }
}
