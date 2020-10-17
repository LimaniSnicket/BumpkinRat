using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ItemObjectUiElement
{
    GameObject gameObject;
    public Image ObjectImage { get; private set; }
    public Button ObjectButton { get; private set; }
    public TextMeshProUGUI LabelTMPro { get; private set; }

    private ItemCrafter itemCrafter;

    static int id;

    public ItemObjectUiElement(ItemCrafter crafter, Transform parent, Vector2 rectPosition)
    {
        itemCrafter = crafter;

        gameObject = new GameObject($"ItemObjectUiElement_{id}", typeof(RectTransform), typeof(Image), typeof(Button));
        ObjectImage = gameObject.GetComponent<Image>();
        ObjectButton = gameObject.GetComponent<Button>();
        gameObject.transform.SetParent(parent);
        SetPositionAndUnitScale(rectPosition);

        ObjectButton.onClick.AddListener(OnClickDebug);
        id++;
    }

    void SetPositionAndUnitScale(Vector2 rectPosition)
    {
        gameObject.GetComponent<RectTransform>().localPosition = rectPosition;
        gameObject.GetComponent<RectTransform>().localScale = Vector2.one;
    }

    void OnClickDebug()
    {
    }
}
