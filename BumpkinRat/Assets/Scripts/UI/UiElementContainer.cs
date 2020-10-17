using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiElementContainer : MonoBehaviour
{
    public RectTransform RectTransform => GetComponent<RectTransform>();

    public Vector2 startPosition;
    public float spacing;

    public bool dynamicallySpace;

    private int elements;
    public int ElementsInContainer
    {
        get
        {
            if(elements != transform.childCount)
            {
                OnElementCountChange();
            }

            elements = transform.childCount;
            return elements;
        }
    }

    void OnElementCountChange()
    {

    }

    RectTransform InstantiateAndGetRectTransform(GameObject prefab)
    {
        GameObject inst = Instantiate(prefab, transform);
        RectTransform tr = inst.GetOrAddComponent<RectTransform>();
        tr.localPosition = Vector2.zero;
        return tr;
    }

    public void SpawnAllHorizontally(GameObject spawning, float spacing)
    {
        RectTransform rect = InstantiateAndGetRectTransform(spawning);

        float width = rect.rect.width;

        rect.localPosition = startPosition + Vector2.right * (width + spacing) * (ElementsInContainer - 1);

    }

    public void SpawnAllVertically(GameObject spawning, float spacing)
    {
        RectTransform rect = InstantiateAndGetRectTransform(spawning);

        float height = rect.rect.height;

        rect.localPosition = startPosition + Vector2.up * (height + spacing) * (ElementsInContainer - 1);
    }

    public void SpawnAtAlternatingVerticalPositions(GameObject spawning, float spacing, float verticalOffset, int everyOther = 2)
    {
        RectTransform rect = InstantiateAndGetRectTransform(spawning);

        Vector2 widthSpacing = Vector2.right * (rect.rect.width + spacing) * (ElementsInContainer - 1);

        Vector2 verticalSpacing = Vector2.up * -verticalOffset * ((ElementsInContainer + 1) % everyOther);

        rect.localPosition = startPosition + widthSpacing + verticalSpacing;
    }

    public GameObject GetLastChild()
    {
        return transform.GetChild(ElementsInContainer - 1).gameObject;
    }

}
