﻿using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ItemObjectUiElement : ItemObjectBehaviour, IContainFocusArea, IOccupyPositions
{
    private Image itemObjectImage;

    private  RectTransform rectTransform;

    public Transform OccupierTransform => rectTransform;

    public Vector3 PositionOffset { get; private set; } = Vector3.zero;

    public event EventHandler<ReleaseOccupierEventArgs> ReleaseOccupier;

    void Awake()
    {
        itemObjectImage = gameObject.GetOrAddComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            return;
        }

        Vector2 localMousePosition = rectTransform.InverseTransformPoint(Input.mousePosition);

        if (!rectTransform.rect.Contains(localMousePosition))
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("To-do: Put item back in inventory, destroy Item Object Ui Element");
        }
    }

    public void SetItemObjectSprite(Sprite s)
    {
        itemObjectImage.sprite = s;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void SetPositionAndUnitScale(Vector2 rectPosition)
    {
        rectTransform.localPosition = rectPosition;
        rectTransform.localScale = Vector2.one;
    }
    public void RegisterFocusHandlerAreas()
    {
        FocusAreaHandler.RegisterFocusAreaUiInChildren(transform, this);
    }

    public override void ForceDestroy()
    {
        this.BroadcastRelease(true);
    }

    private void BroadcastRelease(bool destroyOnRelease)
    {
        var args = new ReleaseOccupierEventArgs
        {
            DestroyOnRelease = destroyOnRelease,
            Occupier = this
        };

        this.ReleaseOccupier.BroadcastEvent(this, args);
    }

    /*    public override void ForceDestroy()
        {
            this.ForceDestroy(this);
        }*/
    /*
        public void OnReleaseAllDestroyObject(object source, EventArgs args)
        {
            if (Occupied != null)
            {
                Occupied.ReleaseOccupier(this);
                Destroy(gameObject);
            }
        }*/
}
