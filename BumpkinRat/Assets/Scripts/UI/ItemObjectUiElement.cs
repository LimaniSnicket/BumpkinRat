using System;
using UnityEngine;
using UnityEngine.UI;
using Items;

[RequireComponent(typeof(Image))]
public class ItemObjectUiElement : ItemObjectBehaviour, IContainFocusArea, IOccupyPositions<RectTransform>
{
    private Image itemObjectImage;

    private  RectTransform rectTransform;

    private bool mouseOver;

    private readonly static GameObject  focusAreaCache;

    public RectTransform OccupierTransform => rectTransform;

    public OccupiablePosition Occupied { get; set; }

    public Vector3 PositionOffset { get; private set; } = Vector3.zero;

    void OnEnable()
    {
        itemObjectImage = gameObject.GetOrAddComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (!mouseOver)
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

    private void SetFromItem(Item item)
    {
        itemObject = new ItemObject(item.itemId);

        Sprite itemSprite = ItemDataManager.GetDisplaySprite(item.itemName);

        this.itemObjectImage.sprite = itemSprite;
        int focusAreas = item.FocusAreaCount;
        if(focusAreas > 0)
        {
            for(int i = 0; i < focusAreas; i++)
            {
                FocusAreaUI uiElement = Instantiate(focusAreaCache).GetOrAddComponent<FocusAreaUI>();
               
                uiElement.transform.SetParent(transform);

                FocusAreaUiDetails details = item.GetFocusAreaUiDetailsAtIndex(i);

                uiElement.SetDetails(details);
            }
        }

        FocusAreaHandler.RegisterFocusAreaUiInChildren(transform, this);
    }

    public void RegisterFocusHandlerAreas()
    {
        FocusAreaHandler.RegisterFocusAreaUiInChildren(transform, this);
    }

    public override void ForceDestroy()
    {
        this.ForceDestroy(this);
    }
}
