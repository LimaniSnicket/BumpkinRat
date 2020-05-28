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
    public MovementInfluence moveInfluence;
    bool atRest => body.velocity == Vector3.zero;
    bool inAir => body.velocity.y != 0;

    public ActionState currentActionState;

   public GameObject forwardObjectLook;

    private void OnEnable()
    {
        currentActionState = ActionState.Scurry;
    }

    private void FixedUpdate()
    {
       if(moveInfluence == MovementInfluence.Keyboard)
        {
            OverworldMovement();
        } else
        {
            if (!climbing) { MouseOverworldMovement(); }
            Climb();
        }

        Ray r = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if(Physics.Raycast(r, out hit, 2f))
        {
            forwardObjectLook = hit.transform.gameObject;
        } else { forwardObjectLook = null; }
        //Debug.DrawLine(transform.position, transform.position + transform.forward, Color.red);
        Debug.DrawRay(r.origin, r.direction, Color.blue);
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
        if (UIManager.menuActive) { return; }
        Vector3 look = MouseManager.mousePosOffset.normalized.Swizzle(GridLayout.CellSwizzle.XZY);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * rotationSpeed);
        if (Input.GetKey(KeyCode.W))
        {
            StopCoroutine("SlowDown");
            body.AddForce(look * forceMultiplier, ForceMode.Force);
            body.AddTorque(MouseManager.delta.Swizzle(GridLayout.CellSwizzle.XZY) * 0.025f, ForceMode.Impulse);
        }
        if (Input.GetKeyUp(KeyCode.W) && !atRest && !inAir)
        {
            StartCoroutine(SlowDown(3));
        }
        if (Input.GetMouseButtonDown(0) && !inAir)
        {
            body.AddForce(Vector3.up * forceMultiplier / 2, ForceMode.Impulse);
        }

        if (forwardObjectLook == null) { return; }
        if (!forwardObjectLook.CompareTag("ClimbingSurface")) { return; }
        if (Input.GetKeyDown(KeyCode.A) && !climbing) { StartCoroutine(ClimbingPosition()); }
    }

    void Climb()
    {
        Ray below = new Ray(transform.position, transform.up * -1);
        RaycastHit rh;
        Transform hit = Physics.Raycast(below, out rh, 1f) ? rh.transform : null;
        if(hit != null)
        {
            //print("Hit is Valid: " + hit.tag);
        }
    }

    IEnumerator SlowDown(float speed)
    {
        while (!body.velocity.Squeeze(Vector3.zero))
        {
            body.velocity = Vector3.Lerp(body.velocity, Vector3.zero, Time.deltaTime * speed);
            yield return null;
        }
        body.velocity = Vector3.zero;
    }
    bool climbing;
    IEnumerator ClimbingPosition()
    {
        transform.DORotate(new Vector3(-90, transform.eulerAngles.y, transform.eulerAngles.z), 3);
        yield return new WaitForSeconds(3);
        climbing = true;
    }
}

