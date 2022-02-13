using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBehaviorBasic: MonoBehaviour
{
    public GameObject toFollow;

    public Vector3 axisInfluence;

    public Vector3 offset;

    Quaternion originalRotation;

    public Quaternion OriginalRotation => originalRotation;

    private bool canFollow = true;

    private void Start()
    {
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        if (!canFollow)
        {
            return;
        }
        transform.position = GetFollowPositionWithInfluences();
    }

    public void SuspendFollow()
    {
        canFollow = false;
    }

    public void ResumeFollow()
    {
        canFollow = true;
        SetRotationToOriginal();
    }

    public Vector3 GetFollowPositionWithInfluences()
    {
        if(toFollow == null)
        {
            return Vector3.zero;
        }

        float x = offset.x + toFollow.transform.position.x * axisInfluence.x;
        float y = offset.y + toFollow.transform.position.y * axisInfluence.y;
        float z = offset.z + toFollow.transform.position.z * axisInfluence.z;

        return new Vector3(x, y, z);
    }

    public void SetRotationToOriginal()
    {
        transform.rotation = originalRotation;
    }

}
