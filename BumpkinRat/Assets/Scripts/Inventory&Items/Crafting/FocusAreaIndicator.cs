using TMPro;
using UnityEngine;

public struct FocusAreaIndicator
{
    public GameObject gameObject;

    private TextMeshProUGUI tmproDisplay;

    private RectTransform rectTransform;

    public FocusAreaIndicator(GameObject obj)
    {
        gameObject = obj;
        tmproDisplay = gameObject.GetComponentInChildren<TextMeshProUGUI>();
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    public void SetTMPMessage(string message)
    {
        tmproDisplay.text = message;
    }

    public void SetPosition(Vector2 position)
    {
        rectTransform.position = position;
    }
}
