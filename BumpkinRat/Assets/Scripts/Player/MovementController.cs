using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class MovementController : MonoBehaviour
{
    [Range(10,20)]
    public float forceMultiplier;
    public float rotationSpeed = 1;
    [Range(2,5)] public float forwardRayDistance = 2;
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
    SphereCollider collisionSphere => GetComponent<SphereCollider>();
    public MovementInfluence moveInfluence;
    bool atRest => body.velocity == Vector3.zero;
    bool inAir => body.velocity.y != 0;

    public Vector3 defaultGravity => new Vector3(0, -9.81f, 0);

    Vector3 upAxis;

    Ray rayDown; RaycastHit downHit;
    Transform climbT;

    public ActionState currentActionState;

   public GameObject forwardObjectLook;
   public ClimbingSurface toClimb;
    CustomGravitySurface gravSurface;

    public bool onSurface;
    public Vector3 contactNormal;

    Quaternion gravityAlignment = Quaternion.identity;

    private void OnEnable()
    {
        currentActionState = ActionState.Scurry;
        climbT = transform;
        OnValidate();
    }

    int physicsStepsSinceGrounded;
    private void FixedUpdate()
    {
        physicsStepsSinceGrounded++;
        if (onSurface) { contactNormal.Normalize();  physicsStepsSinceGrounded = 0; } else { contactNormal = Vector3.up; }
        Ray r = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        rayDown = new Ray(transform.position, transform.up * -1);
        if (Physics.Raycast(rayDown, out downHit, 1f))
        {

        }

        Debug.DrawRay(rayDown.origin, rayDown.direction);

        if (Physics.Raycast(r, out hit, 2f))
        {
            forwardObjectLook = hit.transform.gameObject;
            if (Input.GetKeyDown(KeyCode.W) && !climbing) { BeginClimbing(forwardObjectLook.GetComponent<ClimbingSurface>(), hit.point, hit.triangleIndex); }
        }
        else { forwardObjectLook = null; }

        Debug.DrawRay(r.origin, r.direction, Color.blue);
        if (moveInfluence == MovementInfluence.Keyboard)
        {
            OverworldMovement();
        } else
        {
            if (!climbing)
            {
                MouseOverworldMovement();
            }
            else
            {
                Climb();
            }
        }
        ClearState();
    }

    private void LateUpdate()
    {
        gravityAlignment =
            Quaternion.FromToRotation(
                gravityAlignment * Vector3.up, CustomGravity.GetUpAxis(transform.position)
            ) * gravityAlignment;

    }

    void OverworldMovement()
    {
        if (UIManager.menuActive) { return; }
        if (movementVector != Vector3.zero)
        {
            body.AddForce(movementVector.normalized * forceMultiplier, ForceMode.Force);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementVector), Time.deltaTime * rotationSpeed);
        }
    }

    void MouseOverworldMovement()
    {
        if (UIManager.menuActive || yieldPlayerControl) { return; }
        upAxis = -Physics.gravity.normalized;
        body.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        Vector3 look = MouseManager.mousePosOffset.normalized.Swizzle(GridLayout.CellSwizzle.XZY);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * rotationSpeed);
        if (Input.GetKey(KeyCode.W))
        {
            body.AddForce(look * forceMultiplier, ForceMode.Force);
            body.AddTorque(MouseManager.delta.Swizzle(GridLayout.CellSwizzle.XZY) * 0.025f, ForceMode.Impulse);
        }
        if (Input.GetMouseButtonDown(0) && !inAir)
        {
            body.AddForce(upAxis * forceMultiplier / 2, ForceMode.Impulse);
        }

        if (forwardObjectLook == null) { return; }
        if (!forwardObjectLook.CompareTag("ClimbingSurface")) { return; }
    }

    bool climbing, yieldPlayerControl;
    public Vector3 climbingLook;
    void BeginClimbing(ClimbingSurface c, Vector3 startAt, int triIndex)
    {
        Debug.Log("Begin Climb");
        body.useGravity = false;
        toClimb = c;
        climbing = true;
        transform.position = startAt + (contactNormal * collisionSphere.bounds.extents.y);
        SetGravitySurface(c.GetComponent<CustomGravitySurface>());
    }

    void SetGravitySurface(CustomGravitySurface surf)
    {
        gravSurface = surf;
    }

    void Climb()
    {
        Vector3 lookDir = MouseManager.mousePosOffset.normalized.Swizzle(GridLayout.CellSwizzle.XZY);
        Vector3 moveDir = lookDir * InputX.InputRawVect3.z;
        
        gravSurface.Attract(transform);
        body.MovePosition(body.position + transform.TransformDirection(moveDir) * forceMultiplier * 0.5f * Time.deltaTime);
    }

    [SerializeField, Range(0,90)] float maxAngle = 90;
    float minGroundDotProduct;

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
    }

    Vector3 ProjectDirectionOnPlane(Vector3 dir, Vector3 normal)
    {
        return (dir - normal * Vector3.Dot(dir, normal)).normalized;
    }

    Vector3 ProjectOnContactPlane(Vector3 vect)
    {
        return ProjectDirectionOnPlane(vect, contactNormal);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            if (normal.y >= minGroundDotProduct)
            {
                onSurface = true;
                contactNormal += normal;
                Debug.DrawLine(collision.GetContact(i).point, contactNormal, Color.green);
            }
        }

    }

    void ClearState()
    {
        onSurface = false;
        contactNormal = Vector3.zero;
    }
}

