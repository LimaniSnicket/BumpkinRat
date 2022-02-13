using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UiElementContainer : MonoBehaviour
{
    public Vector2 startPosition;
    public float spacing;
    public int ElementsInContainer => transform.childCount;

    public bool HasElements => transform.childCount > 0;

    public void DetachAndClearChildren()
    {
        if (transform.childCount <= 0)
        {
            return;
        }

        var children = transform.GetChildren();

        foreach (var child in children)
        {
            child.transform.SetParent(null);
            Destroy(child);
        }
    }

    public void SpawnAllHorizontally(GameObject spawning, float spacing)
    {
        RectTransform rect = SpawnRectTransformObject(spawning);

        float width = rect.rect.width;

        rect.localPosition = startPosition + Vector2.right * (width + spacing) * (ElementsInContainer - 1);

    }

    public void SpawnAllVertically(GameObject spawning, float spacing)
    {
        RectTransform rect = SpawnRectTransformObject(spawning);

        float height = rect.rect.height;

        rect.localPosition = startPosition + Vector2.up * (height + spacing) * (ElementsInContainer - 1);
    }

    public void SpawnAtAlternatingVerticalPositions(GameObject spawning, float spacing, float verticalOffset, int everyOther = 2, int? position = null)
    {
        RectTransform rect = SpawnRectTransformObject(spawning);

        int pos = this.PositionInfluence(position);

        Vector2 widthSpacing = Vector2.right * (rect.rect.width + spacing) * (pos - 1);

        Vector2 verticalSpacing = Vector2.up * -verticalOffset * ((pos + 1) % everyOther);

        rect.localPosition = startPosition + widthSpacing + verticalSpacing;
    }

    public T GetLastChildComponent<T>()
    {
        return this.GetLastChild().GetComponent<T>();
    }

    private GameObject GetLastChild()
    {
        return transform.GetChild(ElementsInContainer - 1).gameObject;
    }
    private RectTransform SpawnRectTransformObject(GameObject prefab)
    {
        GameObject inst = Instantiate(prefab, transform);
        RectTransform tr = inst.GetOrAddComponent<RectTransform>();
        tr.localPosition = Vector2.zero;
        return tr;
    }

    private int PositionInfluence(int? passedValue)
    {
        return passedValue ?? ElementsInContainer;
    }
}
