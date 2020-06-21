using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGravitySurface : MonoBehaviour
{
    public float gravity = -50;

    public void Attract(Transform t)
    {
        Vector3 upGrav = (t.position - transform.position).normalized;
        Vector3 bodyUp = t.up;
        t.GetComponent<Rigidbody>().AddForce(upGrav * gravity);
        Quaternion targetROtation = Quaternion.FromToRotation(bodyUp, upGrav) * t.rotation;
        t.rotation = Quaternion.Slerp(t.rotation, targetROtation, Time.deltaTime * 5);
    }
}
