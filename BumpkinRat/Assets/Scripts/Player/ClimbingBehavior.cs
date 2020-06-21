using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingBehavior : MonoBehaviour
{
    public CustomGravitySurface gravitySurface;
    public Rigidbody body => GetComponent<Rigidbody>();

    private void Start()
    {
        body.useGravity = false;
    }

    private void Update()
    {
        gravitySurface.Attract(transform);
    }
}
