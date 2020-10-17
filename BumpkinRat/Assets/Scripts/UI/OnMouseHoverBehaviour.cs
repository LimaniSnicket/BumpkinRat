using UnityEngine;
using UnityEngine.EventSystems;

public class OnMouseHoverBehaviour : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log(eventData.pointerCurrentRaycast.gameObject.name);
    }

    private void OnMouseEnter()
    {
        Debug.Log("Mouse has entered object space");
    }

}
