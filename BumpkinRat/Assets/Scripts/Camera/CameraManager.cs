using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager camManager;

    private static FollowBehaviorBasic basicCameraFollow;

    public Transform craftingViewPoint;

    private void Awake()
    {
        if(camManager == null)
        {
            camManager = this;
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

    private void OnUiEvent(object source, UiEventArgs args)
    {
        basicCameraFollow.enabled = !args.load;

        if (!args.load)
        {
            basicCameraFollow.SetRotationToOriginal();
        } else
        {
            if (args.menuLoaded == MenuType.Crafting)
            {
                ChangeViewTo();
            }
        }

     
    }

    public static void ChangeViewTo()
    {
        camManager.transform.position = camManager.craftingViewPoint.position;
        camManager.transform.rotation = camManager.craftingViewPoint.rotation;
    }

    private void OnDestroy()
    {
        UiMenu.UiEvent -= OnUiEvent;   
    }
}
