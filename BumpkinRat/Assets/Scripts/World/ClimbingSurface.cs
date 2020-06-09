using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ClimbingSurface : MonoBehaviour
{
    public bool climbable;
    MeshFilter meshF => GetComponent<MeshFilter>();


    public Vector3 GetStartingClimbPosition(GameObject player)
    {
        return Vector3.zero;
    }
}
