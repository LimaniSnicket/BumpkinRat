using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    private static MouseManager staticMouse;
    Vect2Delta mousePosition;
    public Vector2 screenDimensions;
    public static Vector2 delta { get; private set; }
    public static Vector2 mousePosOffset => Input.mousePosition - staticMouse.offset;
    public static Vector2 mousePosInfluence => mousePosOffset.PercentOfVector2(staticMouse.screenDimensions);
    private Vector3 offset;
    private void OnEnable()
    {
        if(staticMouse == null) { staticMouse = this; } else { Destroy(this); }
        screenDimensions = new Vector2(Screen.width, Screen.height);
        offset = new Vector2(screenDimensions.x / 2, screenDimensions.y/2);
    }

    private void LateUpdate()
    {
        delta = mousePosition.GetDelta(Input.mousePosition);
    }
}
