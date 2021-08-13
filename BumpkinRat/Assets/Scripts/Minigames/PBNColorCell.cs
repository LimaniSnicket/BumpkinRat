using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class PBNColorCell : ColorCellBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    Image cellImage => GetComponent<Image>();
    RectTransform rectTransform => GetComponent<RectTransform>();


    bool inRaycast;

    public static bool ReferenceInFocus { get; private set; }


    void Start()
    {
    }

    void Update()
    {
        if (!inRaycast)
        {
            return;
        }

        pbn.CheckGraphicsRaycastFromMouse();
    }

    public void SetColor(Color color)
    {
        cellImage.color = color;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        inRaycast = true;
        ReferenceInFocus = true;
        if (pbn.PbnSequenceActive)
        {
            SetReferenceColorAsActive();
            pbn.RefillMeter();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inRaycast = false;
        ReferenceInFocus = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pbn.ActivateSequence();
        SetReferenceColorAsActive();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
    }

    void SetReferenceColorAsActive()
    {
        int num = colorCell.CellNumber;
        SetActiveNumber(num);
    }

    static void SetActiveNumber(int number)
    {
        ColorCell.ActiveNumber = number;
    }

    public static PBNColorCell InstantiatePBNColorCell(int number, PaintByNumbers pbn)
    {
        PBNColorCell newCell = new GameObject("PBN", typeof(PBNColorCell)).GetComponent<PBNColorCell>();
        newCell.InitializeColorCell(pbn, number);
        newCell.cellImage.sprite = pbn.referenceSprite;
        return newCell;
    }

    public static void GenerateGenericPBNColorCellRow(int numCells, PaintByNumbers pbn, Vector2 startPos)
    {
        if(numCells > 0)
        {
            for(int i = 0; i < numCells; i++)
            {
                PBNColorCell newCell = InstantiatePBNColorCell(i, pbn);
                newCell.SetColor(Color.white);
                newCell.transform.SetParent(pbn.transform);
                newCell.rectTransform.position = new Vector2(startPos.x + 100 * i, startPos.y);
            }
        }
    }
}



