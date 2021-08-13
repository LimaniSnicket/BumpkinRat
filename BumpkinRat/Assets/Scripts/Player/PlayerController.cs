using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlayerController : MonoBehaviour
{
    Rigidbody body => GetComponent<Rigidbody>();
    SphereCollider sphereColl => GetComponent<SphereCollider>();

    [Range(10, 20)] public float forceMultiplier = 10;
    [Range(1, 5)] public float rotationSpeed = 1;
    Vector3 kInput
    {
        get
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float forward = Input.GetAxisRaw("Vertical");
            return new Vector3(horizontal, 0, forward);
        }
    }

    public Vector3 contactNormal;
    Ray rayDown, rayForward;
    RaycastHit downHit;

    public bool onSurface;
    public bool freezeXZ;

    private void FixedUpdate()
    {
        /*if (freezeXZ)
        {
            body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        } else
        {
            body.constraints = RigidbodyConstraints.None;
        }*/

        MouseControlledOverworldMovement();
       
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(downHit.point, (transform.position + contactNormal) * 3f, Color.red);
    }

    void MouseControlledOverworldMovement()
    {
        if (UIManager.MenuActive) { return; }
        freezeXZ = true;
        Vector3 look = MouseManager.mousePosOffset.normalized.Swizzle(GridLayout.CellSwizzle.XZY);
        if (kInput.z > 0)
        {
            body.AddForce(look * forceMultiplier, ForceMode.Force);
            body.AddTorque(MouseManager.delta.Swizzle(GridLayout.CellSwizzle.XZY) * 0.25f, ForceMode.Impulse);
        }
    }
}
