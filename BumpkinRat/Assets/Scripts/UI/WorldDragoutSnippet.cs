using System.Collections;
using TMPro;
using UnityEngine;

public class WorldDragoutSnippet : DraggableFoldout<Transform>, ITrackDistanceToPlayer
{
    public RangeChangeTracker DistanceTracker { get; set; }
    private string description = string.Empty;

    private void Start()
    {
        Enable();
    }

    void Update()
    {
        if (FoldoutStatus.Equals(FoldoutStatus.FOLDED_OUT))
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartCoroutine(CollapseBack());
            }
        }
    }

     void Enable()
    {
        draggerTransform = transform;
        foldoutTransform = transform.GetChild(0).transform;
        DistanceTracker = new RangeChangeTracker(foldoutTransform, 2f);
        SetOriginalPosition();
        textDisplay = GetComponentInChildren<TextMeshPro>();

        SetMessage(string.Empty);
    }

    private void OnMouseDrag()
    {
        if (CanDragOut() && ValidMouseDirectionDrag(pullDirection, out pullDirectionalVector))
        {
            StartCoroutine(DragOut());
        }
    }

    public void SetDescription(string desc)
    {
        description = desc;
    }

    public override IEnumerator DragOut()
    {
        bool isCameraController = CameraManager.TryTakeControlOfCamera(this);

        if (isCameraController)
        {
            Vector3 x = originalPosition - pullDirectionalVector * 2.5f;
            yield return StartCoroutine(MovingOut(x, Vector2.one, .3f, 0.07f));
            SetMessage(description);
            Interactable = false;

            //Vector3 cameraOffset = Vector3.back * 2.5f + Vector3.up * -1f;

            CameraManager.LookAt(textDisplay.transform);
        }
    }

    public override IEnumerator CollapseBack()
    {
        Vector2 x = originalPosition;
        SetMessage(string.Empty);
        yield return StartCoroutine(MovingOut(x, Vector2.zero, 0.5f, .07f));
        Interactable = true;

        CameraManager.ReleaseCameraControlBackToFollowBehavior();
    }
}
