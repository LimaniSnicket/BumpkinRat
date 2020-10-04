using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    int itemId;

    public int numberOfPositions;

    public string[] positionOccupiedBy;

    private void Start()
    {
        positionOccupiedBy = Enumerable.Repeat("empty", Math.Max(numberOfPositions, 1)).ToArray();
    }

}
