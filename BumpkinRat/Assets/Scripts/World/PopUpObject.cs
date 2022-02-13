using UnityEngine;

public class PopUpObject : MonoBehaviour
{
    public MeshFilter frontFacing, sideFacing;

    public bool adjustWithView;
    static Transform CameraTransform => Camera.main.transform;

    private void Update()
    {
        if (!adjustWithView)
        {
            return;
        }
    }
}

public enum AppearanceRelativeToCamera
{
    NOT_VISIBLE = 0,
    FRONT_FACING = 1,
    BACK_FACING = 2,
    SIDE_FACING = 3
}
