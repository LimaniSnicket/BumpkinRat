using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [Range(10,20)]
    public float forceMultiplier;
    public float rotationSpeed = 1;
    public Vector3 movementVector
    {
        get
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float forward = Input.GetAxisRaw("Vertical");
            return new Vector3(horizontal, 0, forward);
        }
    }
    Rigidbody body => GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if(movementVector != Vector3.zero)
        {
            body.AddForce(movementVector.normalized * forceMultiplier, ForceMode.Force);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), Time.deltaTime * rotationSpeed);
        }
        Debug.DrawLine(transform.position, transform.forward, Color.red);
    }

}
