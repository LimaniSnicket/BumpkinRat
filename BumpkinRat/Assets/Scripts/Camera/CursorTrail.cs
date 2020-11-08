using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTrail : MonoBehaviour
{
    public Color trailColor = new Color(1, 0, 0.38f);
    public float distanceFromCamera = 5;
    public float startWidth = 0.1f;
    public float endWidth = 0f;
    public float trailTime = 0.24f;

    Transform trailTransform;
    Camera thisCamera;

    public Material trailMaterial;

    public GameObject mouseTrail;

    // Start is called before the first frame update
    void Start()
    {
        thisCamera = Camera.main;//GetComponent<Camera>();

        /*    GameObject trailObj = new GameObject("Mouse Trail");
            trailTransform = trailObj.transform;
            TrailRenderer trail = trailObj.AddComponent<TrailRenderer>();
            trail.time = -1f;*/
        MoveTrailToCursor(Input.mousePosition);
    /*    trail.time = trailTime;
        trail.startWidth = startWidth;
        trail.endWidth = endWidth;
        trail.numCapVertices = 3;
        trail.sharedMaterial = trailMaterial ?? new Material(Shader.Find("Unlit/Color"));
        trail.sharedMaterial.color = trailColor;*/
    }

    // Update is called once per frame
    void Update()
    {
        MoveTrailToCursor(Input.mousePosition);
    }

    void MoveTrailToCursor(Vector3 screenPosition)
    {
        mouseTrail.transform.position = thisCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distanceFromCamera));
    }
}
