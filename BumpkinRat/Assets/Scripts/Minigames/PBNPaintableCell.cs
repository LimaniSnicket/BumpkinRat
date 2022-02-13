using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PBNPaintableCell : ColorCellBehaviour
{
    Texture2D texture;
    public Sprite secondaryMap;

    SpriteRenderer spriteRenderer;
    TextMeshPro numberDisplay;

    public Color debugHit;

    int paintedNumber;
    public bool Valid;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        secondaryMap = pbn.interactionMap;
    }

    public override void InitializeColorCell(PaintByNumbers reference, int num)
    {
        base.InitializeColorCell(reference, num);
        GetNumberToDisplay();
    }

    protected override void SetColor(SpriteRenderer renderer)
    {
        base.SetColor(renderer);
        numberDisplay.gameObject.SetActive(false);
    }

    private void OnMouseEnter()
    {
        texture = secondaryMap.CreateTexture2D();
    }

    private void OnMouseOver()
    {
        if (!pbn.PbnSequenceActive)
        {
            return;
        }

        Ray ray = Camera.main.ViewportPointToRay(Camera.main.ScreenToViewportPoint(Input.mousePosition));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000))
        {
            Color c = GetColorAtPoint(hit.point);
            debugHit = c;


            if(ColorX.Approximately(c, Color.green))
            {
                paintedNumber = ColorCell.ActiveNumber;
                Valid = paintedNumber == colorCell.CellNumber;

                SetColor(spriteRenderer);
            }
        }
    }

    Color GetColorAtPoint(Vector3 v)
    {
        return texture.GetPixelBilinear(v.x, v.y);
    }

    void GetNumberToDisplay()
    {
        GameObject textObj = Instantiate(pbn.numberPrefab);
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = Vector3.zero;
        TextMeshPro tmp = textObj.GetComponent<TextMeshPro>();
        tmp.text = colorCell.CellNumber.ToString();
        numberDisplay = tmp;
    }

}
