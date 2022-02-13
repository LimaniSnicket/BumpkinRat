using UnityEngine;
using DG.Tweening;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    private static CameraManager camManager;

    private static FollowBehaviorBasic basicCameraFollow;

    public Transform craftingViewPoint, craftingFocusViewPoint;

    static string controller;

    const string FollowBehaviorString = "FollowBehaviorBasic";

    public static bool IsAvailableForControl => controller == FollowBehaviorString;

    private void Awake()
    {
        if(camManager == null)
        {
            camManager = this;
            controller = FollowBehaviorString;
        } else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        basicCameraFollow = gameObject.GetOrAddComponent<FollowBehaviorBasic>();
        UiMenu.UiEvent += OnUiEvent;
    }

    private void Update()
    {
        print(controller);
    }

    private void OnUiEvent(object source, UiEventArgs args)
    {
        if (UIManager.MenuActive)
        {
            TryTakeControlOfCamera(args.UiMenuName);
        } else
        {
            basicCameraFollow.SetRotationToOriginal();
        }

        if (args.MenuTypeLoaded == MenuType.Crafting)
        {
            if (args.Load)
            {
                ChangeViewTo();
            }
        }
    }

    public static Vector3 GetEulersOfAxes(string axes)
    {
        return camManager.transform.eulerAngles.GetAxesOfVector3(axes);
    }

    public static void LookAt(Transform t)
    {
        camManager.transform.DOLookAt(t.position, 1f);
    }

    public static bool TryTakeControlOfCamera(string newController)
    {
        if (IsAvailableForControl)
        {
            basicCameraFollow.SuspendFollow();
            controller = newController;
            return true;
        }
        return false;
    }

    public static bool TryTakeControlOfCamera<T>(T newController)
    {
        string typeName = newController.GetType().ToString();

        return TryTakeControlOfCamera(typeName);
    }

    public static void ReleaseCameraControlBackToFollowBehavior()
    {
        camManager.StartCoroutine(ReturnControlToCameraFollow());
    }

    static IEnumerator ReturnControlToCameraFollow()
    {
        Vector3 position = basicCameraFollow.GetFollowPositionWithInfluences();
        Vector3 euler = basicCameraFollow.OriginalRotation.eulerAngles;
        camManager.transform.DOMove(position, 0.8f);
        camManager.transform.DORotate(euler, 1f);

        yield return new WaitForSeconds(1);

        controller = FollowBehaviorString;
        basicCameraFollow.ResumeFollow();
    } 

    public static void ChangeViewTo(bool doMove = false)
    {
        if (!doMove)
        {
            camManager.transform.position = camManager.craftingViewPoint.position;
            camManager.transform.rotation = camManager.craftingViewPoint.rotation;
        } else
        {
            DoCameraMoveAndRotate(camManager.craftingFocusViewPoint.position, camManager.craftingFocusViewPoint.rotation.eulerAngles, 1.2f, 1.4f);
        }
    }

    public static void CraftingFocusView()
    {
        camManager.transform.DOMove(camManager.craftingFocusViewPoint.position, 1.2f);
        camManager.transform.DORotate(camManager.craftingFocusViewPoint.rotation.eulerAngles, 1.4f);
    }

    static void DoCameraMoveAndRotate(Vector3 pos, Vector3 euler, float moveTime, float rotateTime)
    {
        camManager.transform.DOMove(pos, moveTime);
        camManager.transform.DORotate(euler, rotateTime);
    }

    private void OnDestroy()
    {
        UiMenu.UiEvent -= OnUiEvent;   
    }
}
