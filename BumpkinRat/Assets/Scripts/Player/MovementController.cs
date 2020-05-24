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

    private void FixedUpdate()
    {
       if(moveInfluence == MovementInfluence.Keyboard)
        {
            OverworldMovement();
        } else
        {
            MouseOverworldMovement();
        }
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

    bool atRest => body.velocity == Vector3.zero;

    void MouseOverworldMovement()
    {
        if (UIManager.menuActive) { return; }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            StopCoroutine("SlowDown");
            Vector3 look = MouseManager.mousePosOffset.normalized.Swizzle(GridLayout.CellSwizzle.XZY); 
            body.AddForce(look * forceMultiplier, ForceMode.Force);
            body.AddForce(MouseManager.delta.Swizzle(GridLayout.CellSwizzle.XZY) * 0.01f, ForceMode.Impulse);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(look), Time.deltaTime * rotationSpeed);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && !atRest)
        {
            StartCoroutine(SlowDown(3));
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
}
